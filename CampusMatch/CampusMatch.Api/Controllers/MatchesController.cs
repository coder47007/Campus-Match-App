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
public class MatchesController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    
    public MatchesController(CampusMatchDbContext db)
    {
        _db = db;
    }
    
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpGet]
    public async Task<ActionResult<List<MatchDto>>> GetMatches()
    {
        var userId = GetUserId();
        
        var matches = await _db.Matches
            .Include(m => m.Student1)
            .Include(m => m.Student2)
            .Where(m => m.Student1Id == userId || m.Student2Id == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
        
        var result = matches.Select(m =>
        {
            var other = m.Student1Id == userId ? m.Student2 : m.Student1;
            return new MatchDto(m.Id, other.Id, other.Name, other.PhotoUrl, other.Major, m.CreatedAt);
        }).ToList();
        
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
        
        // Set match as inactive instead of deleting (keeps message history)
        match.IsActive = false;
        await _db.SaveChangesAsync();
        
        return Ok(new { message = "Successfully unmatched" });
    }
}
