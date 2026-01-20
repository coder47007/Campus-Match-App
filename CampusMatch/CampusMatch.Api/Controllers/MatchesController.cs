using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CampusMatch.Api.Data;
using CampusMatch.Api.Services;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    private readonly ICacheService _cache;
    
    public MatchesController(CampusMatchDbContext db, ICacheService cache)
    {
        _db = db;
        _cache = cache;
    }
    
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpGet]
    public async Task<ActionResult<List<MatchDto>>> GetMatches()
    {
        var userId = GetUserId();
        var cacheKey = CacheKeys.StudentMatches(userId);
        
        // Try cache first
        var cachedMatches = await _cache.GetAsync<List<MatchDto>>(cacheKey);
        if (cachedMatches != null)
        {
            return Ok(cachedMatches);
        }
        
        var matches = await _db.Matches
            .Include(m => m.Student1)
            .Include(m => m.Student2)
            .Where(m => (m.Student1Id == userId || m.Student2Id == userId) && m.IsActive)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
        
        var result = matches.Select(m =>
        {
            var other = m.Student1Id == userId ? m.Student2 : m.Student1;
            return new MatchDto(m.Id, other.Id, other.Name, other.PhotoUrl, other.Major, m.CreatedAt);
        }).ToList();
        
        // Cache for 5 minutes (matches change when new matches occur)
        await _cache.SetAsync(cacheKey, result, CacheKeys.Expiration.Short);
        
        return Ok(result);
    }
    
    [HttpDelete("{matchId}")]
    public async Task<ActionResult> UnmatchUser(int matchId)
    {
        var userId = GetUserId();
        
        var match = await _db.Matches
            .FirstOrDefaultAsync(m => m.Id == matchId && 
                (m.Student1Id == userId || m.Student2Id == userId));
        
        if (match == null)
            return NotFound("Match not found");
        
        // Determine the other user's ID for cache invalidation
        var otherUserId = match.Student1Id == userId ? match.Student2Id : match.Student1Id;
        
        // Set match as inactive instead of deleting (keeps message history)
        match.IsActive = false;
        await _db.SaveChangesAsync();
        
        // Invalidate cache for both users
        await _cache.RemoveAsync(CacheKeys.StudentMatches(userId));
        await _cache.RemoveAsync(CacheKeys.StudentMatches(otherUserId));
        await _cache.RemoveAsync(CacheKeys.Match(matchId));
        
        return Ok(new { message = "Successfully unmatched" });
    }
}
