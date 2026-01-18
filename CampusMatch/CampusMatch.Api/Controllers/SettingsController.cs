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
public class SettingsController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    
    public SettingsController(CampusMatchDbContext db)
    {
        _db = db;
    }
    
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpGet]
    public async Task<ActionResult<SettingsDto>> GetSettings()
    {
        var userId = GetUserId();
        var student = await _db.Students.FindAsync(userId);
        if (student == null) return NotFound();
        
        return Ok(new SettingsDto(
            student.MinAgePreference,
            student.MaxAgePreference,
            student.MaxDistancePreference,
            student.ShowOnlineStatus,
            student.NotifyOnMatch,
            student.NotifyOnMessage,
            student.NotifyOnSuperLike,
            student.IsProfileHidden
        ));
    }
    
    [HttpPut]
    public async Task<ActionResult<SettingsDto>> UpdateSettings(UpdateSettingsRequest request)
    {
        var userId = GetUserId();
        var student = await _db.Students.FindAsync(userId);
        if (student == null) return NotFound();
        
        // Update only provided fields
        if (request.MinAgePreference.HasValue)
            student.MinAgePreference = Math.Clamp(request.MinAgePreference.Value, 18, 100);
        if (request.MaxAgePreference.HasValue)
            student.MaxAgePreference = Math.Clamp(request.MaxAgePreference.Value, 18, 100);
        if (request.MaxDistancePreference.HasValue)
            student.MaxDistancePreference = Math.Clamp(request.MaxDistancePreference.Value, 1, 100);
        if (request.ShowOnlineStatus.HasValue)
            student.ShowOnlineStatus = request.ShowOnlineStatus.Value;
        if (request.NotifyOnMatch.HasValue)
            student.NotifyOnMatch = request.NotifyOnMatch.Value;
        if (request.NotifyOnMessage.HasValue)
            student.NotifyOnMessage = request.NotifyOnMessage.Value;
        if (request.NotifyOnSuperLike.HasValue)
            student.NotifyOnSuperLike = request.NotifyOnSuperLike.Value;
        if (request.IsProfileHidden.HasValue)
            student.IsProfileHidden = request.IsProfileHidden.Value;
        
        // Ensure min <= max age
        if (student.MinAgePreference > student.MaxAgePreference)
        {
            student.MaxAgePreference = student.MinAgePreference;
        }
        
        await _db.SaveChangesAsync();
        
        return Ok(new SettingsDto(
            student.MinAgePreference,
            student.MaxAgePreference,
            student.MaxDistancePreference,
            student.ShowOnlineStatus,
            student.NotifyOnMatch,
            student.NotifyOnMessage,
            student.NotifyOnSuperLike,
            student.IsProfileHidden
        ));
    }
}
