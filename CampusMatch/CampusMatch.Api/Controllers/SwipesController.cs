using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CampusMatch.Api.Data;
using CampusMatch.Api.Hubs;
using CampusMatch.Api.Models;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SwipesController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    private readonly IHubContext<ChatHub> _hubContext;
    
    public SwipesController(CampusMatchDbContext db, IHubContext<ChatHub> hubContext)
    {
        _db = db;
        _hubContext = hubContext;
    }
    
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpPost]
    [EnableRateLimiting("swipe")]
    public async Task<ActionResult<SwipeResponse>> Swipe(SwipeRequest request)
    {
        var userId = GetUserId();
        
        // Check if blocked
        var isBlocked = await _db.Blocks.AnyAsync(b =>
            (b.BlockerId == userId && b.BlockedId == request.SwipedId) ||
            (b.BlockerId == request.SwipedId && b.BlockedId == userId));
            
        if (isBlocked)
            return BadRequest("Cannot swipe on this profile.");
        
        // Check if already swiped
        var existingSwipe = await _db.Swipes
            .FirstOrDefaultAsync(s => s.SwiperId == userId && s.SwipedId == request.SwipedId);
            
        if (existingSwipe != null)
        {
            return BadRequest("Already swiped on this profile.");
        }
        
        // Handle super like limit
        if (request.IsSuperLike)
        {
            var user = await _db.Students.FindAsync(userId);
            if (user == null) return NotFound();
            
            // Reset if needed
            if (DateTime.UtcNow >= user.SuperLikesResetAt)
            {
                user.SuperLikesRemaining = 3;
                user.SuperLikesResetAt = DateTime.UtcNow.Date.AddDays(1);
            }
            
            if (user.SuperLikesRemaining <= 0)
            {
                return BadRequest("No super likes remaining. Resets at midnight.");
            }
            
            user.SuperLikesRemaining--;
        }
        
        // Create swipe
        var swipe = new Swipe
        {
            SwiperId = userId,
            SwipedId = request.SwipedId,
            IsLike = request.IsLike,
            IsSuperLike = request.IsSuperLike
        };
        
        _db.Swipes.Add(swipe);
        await _db.SaveChangesAsync();
        
        // If like, check for mutual match
        if (request.IsLike)
        {
            var mutualLike = await _db.Swipes
                .FirstOrDefaultAsync(s => s.SwiperId == request.SwipedId && s.SwipedId == userId && s.IsLike);
                
            if (mutualLike != null)
            {
                // Create match
                var match = new Match
                {
                    Student1Id = Math.Min(userId, request.SwipedId),
                    Student2Id = Math.Max(userId, request.SwipedId)
                };
                
                _db.Matches.Add(match);
                await _db.SaveChangesAsync();
                
                // Get both students for notifications
                var currentUser = await _db.Students.FindAsync(userId);
                var otherUser = await _db.Students.FindAsync(request.SwipedId);
                
                var matchDtoForCurrent = new MatchDto(
                    match.Id, otherUser!.Id, otherUser.Name, otherUser.PhotoUrl, otherUser.Major, match.CreatedAt
                );
                
                var matchDtoForOther = new MatchDto(
                    match.Id, currentUser!.Id, currentUser.Name, currentUser.PhotoUrl, currentUser.Major, match.CreatedAt
                );
                
                // Notify via SignalR if online
                var otherConnectionId = ChatHub.GetConnectionId(request.SwipedId);
                if (otherConnectionId != null)
                {
                    await _hubContext.Clients.Client(otherConnectionId).SendAsync("NewMatch", matchDtoForOther);
                }
                
                return Ok(new SwipeResponse(true, matchDtoForCurrent));
            }
        }
        
        return Ok(new SwipeResponse(false, null));
    }
    
    // Undo/Rewind last swipe
    [HttpDelete("last")]
    public async Task<ActionResult<UndoSwipeResponse>> UndoLastSwipe()
    {
        var userId = GetUserId();
        var user = await _db.Students.FindAsync(userId);
        if (user == null) return NotFound();
        
        // Reset rewinds if needed
        if (DateTime.UtcNow >= user.RewindsResetAt)
        {
            user.RewindsRemaining = 1;
            user.RewindsResetAt = DateTime.UtcNow.Date.AddDays(1);
        }
        
        // Check if rewinds available
        if (user.RewindsRemaining <= 0)
        {
            return Ok(new UndoSwipeResponse(false, "No rewinds remaining. Resets at midnight."));
        }
        
        // Find the most recent swipe (within 30 seconds)
        var recentCutoff = DateTime.UtcNow.AddSeconds(-30);
        var lastSwipe = await _db.Swipes
            .Where(s => s.SwiperId == userId && s.CreatedAt >= recentCutoff)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();
        
        if (lastSwipe == null)
        {
            return Ok(new UndoSwipeResponse(false, "No recent swipe to undo. You can only undo swipes made within the last 30 seconds."));
        }
        
        // Check if a match was created from this swipe
        var match = await _db.Matches
            .FirstOrDefaultAsync(m => 
                (m.Student1Id == userId && m.Student2Id == lastSwipe.SwipedId) ||
                (m.Student1Id == lastSwipe.SwipedId && m.Student2Id == userId));
        
        if (match != null)
        {
            // Delete messages first
            var messages = await _db.Messages.Where(m => m.MatchId == match.Id).ToListAsync();
            _db.Messages.RemoveRange(messages);
            
            // Delete match
            _db.Matches.Remove(match);
        }
        
        // Delete the swipe
        _db.Swipes.Remove(lastSwipe);
        
        // Refund super like if applicable
        if (lastSwipe.IsSuperLike)
        {
            user.SuperLikesRemaining++;
        }
        
        // Use rewind
        user.RewindsRemaining--;
        
        await _db.SaveChangesAsync();
        
        return Ok(new UndoSwipeResponse(true, "Swipe undone successfully."));
    }
    
    // Get remaining rewinds count
    [HttpGet("rewinds")]
    public async Task<ActionResult<object>> GetRewindsRemaining()
    {
        var userId = GetUserId();
        var user = await _db.Students.FindAsync(userId);
        if (user == null) return NotFound();
        
        // Reset if needed
        if (DateTime.UtcNow >= user.RewindsResetAt)
        {
            user.RewindsRemaining = 1;
            user.RewindsResetAt = DateTime.UtcNow.Date.AddDays(1);
            await _db.SaveChangesAsync();
        }
        
        return Ok(new { 
            remaining = user.RewindsRemaining,
            resetsAt = user.RewindsResetAt
        });
    }
}
