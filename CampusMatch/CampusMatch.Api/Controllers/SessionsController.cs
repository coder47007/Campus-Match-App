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
public class SessionsController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    
    public SessionsController(CampusMatchDbContext db)
    {
        _db = db;
    }
    
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    private string? GetCurrentRefreshToken()
    {
        // Extract from Authorization header or query - this is simplified
        // In production you'd track session ID in JWT claims
        return null;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<SessionDto>>> GetSessions()
    {
        var userId = GetUserId();
        var currentToken = GetCurrentRefreshToken();
        
        var sessions = await _db.Sessions
            .Where(s => s.StudentId == userId && !s.IsRevoked && s.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(s => s.LastActiveAt)
            .Select(s => new SessionDto(
                s.Id,
                s.DeviceInfo,
                s.IpAddress,
                s.CreatedAt,
                s.LastActiveAt,
                s.RefreshToken == currentToken  // IsCurrent
            ))
            .ToListAsync();
        
        return Ok(sessions);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> RevokeSession(int id)
    {
        var userId = GetUserId();
        var session = await _db.Sessions.FirstOrDefaultAsync(s => s.Id == id && s.StudentId == userId);
        
        if (session == null)
            return NotFound("Session not found.");
        
        session.IsRevoked = true;
        await _db.SaveChangesAsync();
        
        return Ok(new { message = "Session revoked successfully." });
    }
    
    [HttpDelete]
    public async Task<IActionResult> RevokeAllSessions()
    {
        var userId = GetUserId();
        var currentToken = GetCurrentRefreshToken();
        
        // Revoke all sessions except current one
        var sessions = await _db.Sessions
            .Where(s => s.StudentId == userId && !s.IsRevoked && s.RefreshToken != currentToken)
            .ToListAsync();
        
        foreach (var session in sessions)
        {
            session.IsRevoked = true;
        }
        
        await _db.SaveChangesAsync();
        
        return Ok(new { message = $"Revoked {sessions.Count} sessions." });
    }
}
