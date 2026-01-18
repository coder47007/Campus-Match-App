using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CampusMatch.Api.Data;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LikesController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    
    public LikesController(CampusMatchDbContext db)
    {
        _db = db;
    }
    
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    // Get blurred preview of who liked you (users who swiped right on you but you haven't swiped on them)
    [HttpGet("received")]
    public async Task<ActionResult<List<LikePreviewDto>>> GetReceivedLikes()
    {
        var userId = GetUserId();
        
        // Get IDs of users you've already swiped on
        var swipedIds = await _db.Swipes
            .Where(s => s.SwiperId == userId)
            .Select(s => s.SwipedId)
            .ToListAsync();
        
        // Get IDs of blocked users (both directions)
        var blockedIds = await _db.Blocks
            .Where(b => b.BlockerId == userId || b.BlockedId == userId)
            .Select(b => b.BlockerId == userId ? b.BlockedId : b.BlockerId)
            .ToListAsync();
        
        var excludedIds = swipedIds.Concat(blockedIds).Distinct().ToHashSet();
        
        // Get users who liked you but you haven't swiped on
        var likes = await _db.Swipes
            .Include(s => s.Swiper)
            .Where(s => s.SwipedId == userId && s.IsLike && !excludedIds.Contains(s.SwiperId))
            .OrderByDescending(s => s.IsSuperLike)
            .ThenByDescending(s => s.CreatedAt)
            .Take(20)
            .Select(s => new LikePreviewDto(
                s.SwiperId,
                null,  // Blurred photo URL (client can blur the actual photo)
                s.Swiper.Name.Length > 0 ? s.Swiper.Name[0].ToString() : "?",
                s.IsSuperLike,
                s.CreatedAt
            ))
            .ToListAsync();
        
        return Ok(likes);
    }
    
    // Get count of likes received
    [HttpGet("count")]
    public async Task<ActionResult<object>> GetLikesCount()
    {
        var userId = GetUserId();
        
        // Get IDs of users you've already swiped on or matched with
        var swipedIds = await _db.Swipes
            .Where(s => s.SwiperId == userId)
            .Select(s => s.SwipedId)
            .ToListAsync();
        
        var blockedIds = await _db.Blocks
            .Where(b => b.BlockerId == userId || b.BlockedId == userId)
            .Select(b => b.BlockerId == userId ? b.BlockedId : b.BlockerId)
            .ToListAsync();
        
        var excludedIds = swipedIds.Concat(blockedIds).Distinct().ToHashSet();
        
        var count = await _db.Swipes
            .CountAsync(s => s.SwipedId == userId && s.IsLike && !excludedIds.Contains(s.SwiperId));
        
        var superLikeCount = await _db.Swipes
            .CountAsync(s => s.SwipedId == userId && s.IsSuperLike && !excludedIds.Contains(s.SwiperId));
        
        return Ok(new { 
            total = count, 
            superLikes = superLikeCount,
            hasNew = count > 0
        });
    }
}
