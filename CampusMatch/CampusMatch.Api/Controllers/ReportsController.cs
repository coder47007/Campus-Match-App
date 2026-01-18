using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CampusMatch.Api.Data;
using CampusMatch.Api.Models;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    
    public ReportsController(CampusMatchDbContext db)
    {
        _db = db;
    }
    
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpPost]
    public async Task<IActionResult> ReportUser([FromBody] ReportRequest request)
    {
        var userId = GetUserId();
        
        if (userId == request.ReportedId)
            return BadRequest("Cannot report yourself.");
        
        // Check if already reported
        var existing = await _db.Reports.FirstOrDefaultAsync(r => 
            r.ReporterId == userId && r.ReportedId == request.ReportedId);
            
        if (existing != null)
            return BadRequest("You have already reported this user.");
        
        var report = new Report
        {
            ReporterId = userId,
            ReportedId = request.ReportedId,
            Reason = request.Reason,
            Details = request.Details
        };
        
        _db.Reports.Add(report);
        
        // Handle source-based actions
        if (request.Source?.ToLower() == "chat")
        {
            // Unmatch when reporting from chat
            var match = await _db.Matches.FirstOrDefaultAsync(m =>
                (m.Student1Id == userId && m.Student2Id == request.ReportedId) ||
                (m.Student1Id == request.ReportedId && m.Student2Id == userId));
            
            if (match != null)
            {
                match.IsActive = false;
            }
        }
        else if (request.Source?.ToLower() == "discover")
        {
            // Auto-dislike when reporting from discover view
            var existingSwipe = await _db.Swipes.FirstOrDefaultAsync(s => 
                s.SwiperId == userId && s.SwipedId == request.ReportedId);
            
            if (existingSwipe == null)
            {
                var swipe = new Swipe
                {
                    SwiperId = userId,
                    SwipedId = request.ReportedId,
                    IsLike = false,
                    IsSuperLike = false
                };
                _db.Swipes.Add(swipe);
            }
        }
        
        await _db.SaveChangesAsync();
        
        var message = request.Source?.ToLower() == "chat" 
            ? "Report submitted and user unmatched. Thank you for helping keep our community safe."
            : "Report submitted. Thank you for helping keep our community safe.";
        
        return Ok(new { message });
    }
    
    [HttpPost("block")]
    public async Task<IActionResult> BlockUser([FromBody] BlockRequest request)
    {
        var userId = GetUserId();
        
        if (userId == request.BlockedId)
            return BadRequest("Cannot block yourself.");
        
        // Check if already blocked
        var existing = await _db.Blocks.FirstOrDefaultAsync(b => 
            b.BlockerId == userId && b.BlockedId == request.BlockedId);
            
        if (existing != null)
            return BadRequest("User is already blocked.");
        
        var block = new Block
        {
            BlockerId = userId,
            BlockedId = request.BlockedId
        };
        
        _db.Blocks.Add(block);
        
        // Remove any existing match between users
        var match = await _db.Matches.FirstOrDefaultAsync(m =>
            (m.Student1Id == userId && m.Student2Id == request.BlockedId) ||
            (m.Student1Id == request.BlockedId && m.Student2Id == userId));
            
        if (match != null)
        {
            match.IsActive = false;
        }
        
        await _db.SaveChangesAsync();
        
        return Ok(new { message = "User blocked successfully." });
    }
    
    [HttpDelete("block/{blockedId}")]
    public async Task<IActionResult> UnblockUser(int blockedId)
    {
        var userId = GetUserId();
        
        var block = await _db.Blocks.FirstOrDefaultAsync(b => 
            b.BlockerId == userId && b.BlockedId == blockedId);
            
        if (block == null)
            return NotFound("Block not found.");
        
        _db.Blocks.Remove(block);
        await _db.SaveChangesAsync();
        
        return Ok(new { message = "User unblocked." });
    }
    
    [HttpGet("blocked")]
    public async Task<ActionResult<List<BlockedUserDto>>> GetBlockedUsers()
    {
        var userId = GetUserId();
        
        var blocked = await _db.Blocks
            .Where(b => b.BlockerId == userId)
            .Include(b => b.Blocked)
            .Select(b => new BlockedUserDto(
                b.BlockedId, 
                b.Blocked.Name, 
                b.Blocked.PhotoUrl,
                b.CreatedAt))
            .ToListAsync();
            
        return Ok(blocked);
    }
}
