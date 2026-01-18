using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CampusMatch.Api.Data;
using CampusMatch.Api.Models;
using CampusMatch.Api.Attributes;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Api.Controllers;

/// <summary>
/// Admin-only controller for managing users, reports, interests, prompts, and viewing activity logs.
/// All endpoints require admin authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[AdminOnly]
public class AdminController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    
    public AdminController(CampusMatchDbContext db)
    {
        _db = db;
    }
    
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    
    private async Task LogActivity(string action, int? targetUserId = null, string? details = null)
    {
        var log = new ActivityLog
        {
            AdminId = GetUserId(),
            TargetUserId = targetUserId,
            Action = action,
            Details = details
        };
        _db.ActivityLogs.Add(log);
        await _db.SaveChangesAsync();
    }
    
    // GET: api/admin/stats
    [HttpGet("stats")]
    public async Task<ActionResult<AdminStatsDto>> GetStats()
    {
        var now = DateTime.UtcNow;
        var thirtyDaysAgo = now.AddDays(-30);
        
        var stats = new AdminStatsDto(
            TotalUsers: await _db.Students.CountAsync(s => !s.IsAdmin),
            ActiveUsers: await _db.Students.CountAsync(s => !s.IsAdmin && s.LastActiveAt > thirtyDaysAgo),
            BannedUsers: await _db.Students.CountAsync(s => s.IsBanned),
            TotalMatches: await _db.Matches.CountAsync(),
            TotalMessages: await _db.Messages.CountAsync(),
            PendingReports: await _db.Reports.CountAsync(r => !r.IsReviewed),
            TotalReports: await _db.Reports.CountAsync(),
            TotalInterests: await _db.Interests.CountAsync(),
            TotalPrompts: await _db.Prompts.CountAsync()
        );
        
        return Ok(stats);
    }
    
    // GET: api/admin/users
    [HttpGet("users")]
    public async Task<ActionResult<List<AdminUserDto>>> GetUsers([FromQuery] string? search = null, [FromQuery] bool? banned = null)
    {

        
        var query = _db.Students.Where(s => !s.IsAdmin).AsQueryable();
        
        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(search) || s.Email.ToLower().Contains(search));
        }
        
        if (banned.HasValue)
        {
            query = query.Where(s => s.IsBanned == banned.Value);
        }
        
        var users = await query
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new AdminUserDto(
                s.Id,
                s.Email,
                s.Name,
                s.Age,
                s.Major,
                s.PhotoUrl,
                s.CreatedAt,
                s.LastActiveAt,
                s.IsAdmin,
                s.IsBanned,
                s.BanReason,
                _db.Matches.Count(m => (m.Student1Id == s.Id || m.Student2Id == s.Id) && m.IsActive),
                _db.Reports.Count(r => r.ReportedId == s.Id)
            ))
            .ToListAsync();
            
        return Ok(users);
    }
    
    // GET: api/admin/users/{id}
    [HttpGet("users/{id}")]
    public async Task<ActionResult<AdminUserDto>> GetUser(int id)
    {

        
        var user = await _db.Students.FindAsync(id);
        if (user == null) return NotFound();
        
        var dto = new AdminUserDto(
            user.Id,
            user.Email,
            user.Name,
            user.Age,
            user.Major,
            user.PhotoUrl,
            user.CreatedAt,
            user.LastActiveAt,
            user.IsAdmin,
            user.IsBanned,
            user.BanReason,
            await _db.Matches.CountAsync(m => (m.Student1Id == user.Id || m.Student2Id == user.Id) && m.IsActive),
            await _db.Reports.CountAsync(r => r.ReportedId == user.Id)
        );
        
        return Ok(dto);
    }
    
    // POST: api/admin/users/{id}/ban
    [HttpPost("users/{id}/ban")]
    public async Task<IActionResult> BanUser(int id, [FromBody] BanUserRequest request)
    {

        
        var user = await _db.Students.FindAsync(id);
        if (user == null) return NotFound();
        if (user.IsAdmin) return BadRequest("Cannot ban admin users.");
        
        user.IsBanned = true;
        user.BannedAt = DateTime.UtcNow;
        user.BanReason = request.Reason;
        
        // Revoke all sessions
        var sessions = await _db.Sessions.Where(s => s.StudentId == id).ToListAsync();
        foreach (var session in sessions)
        {
            session.IsRevoked = true;
        }
        
        await _db.SaveChangesAsync();
        await LogActivity("BanUser", id, $"Reason: {request.Reason}");
        
        return Ok(new { message = "User banned successfully." });
    }
    
    // POST: api/admin/users/{id}/unban
    [HttpPost("users/{id}/unban")]
    public async Task<IActionResult> UnbanUser(int id)
    {

        
        var user = await _db.Students.FindAsync(id);
        if (user == null) return NotFound();
        
        user.IsBanned = false;
        user.BannedAt = null;
        user.BanReason = null;
        
        await _db.SaveChangesAsync();
        await LogActivity("UnbanUser", id);
        
        return Ok(new { message = "User unbanned successfully." });
    }
    
    // DELETE: api/admin/users/{id}
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {

        
        var user = await _db.Students.FindAsync(id);
        if (user == null) return NotFound();
        if (user.IsAdmin) return BadRequest("Cannot delete admin users.");
        
        var userName = user.Name;
        
        _db.Students.Remove(user);
        await _db.SaveChangesAsync();
        await LogActivity("DeleteUser", null, $"Deleted user: {userName} (ID: {id})");
        
        return Ok(new { message = "User deleted successfully." });
    }
    
    // GET: api/admin/reports
    [HttpGet("reports")]
    public async Task<ActionResult<List<AdminReportDto>>> GetReports([FromQuery] bool pendingOnly = true)
    {

        
        var query = _db.Reports
            .Include(r => r.Reporter)
            .Include(r => r.Reported)
            .AsQueryable();
            
        if (pendingOnly)
        {
            query = query.Where(r => !r.IsReviewed);
        }
        
        var reports = await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new AdminReportDto(
                r.Id,
                r.ReporterId,
                r.Reporter.Name,
                r.ReportedId,
                r.Reported.Name,
                r.Reported.PhotoUrl,
                r.Reason,
                r.Details,
                r.CreatedAt,
                r.IsReviewed
            ))
            .ToListAsync();
            
        return Ok(reports);
    }
    
    // POST: api/admin/reports/{id}/resolve
    [HttpPost("reports/{id}/resolve")]
    public async Task<IActionResult> ResolveReport(int id)
    {

        
        var report = await _db.Reports.FindAsync(id);
        if (report == null) return NotFound();
        
        report.IsReviewed = true;
        await _db.SaveChangesAsync();
        await LogActivity("ResolveReport", report.ReportedId, $"Report ID: {id}");
        
        return Ok(new { message = "Report marked as resolved." });
    }
    
    // DELETE: api/admin/reports/{id}
    [HttpDelete("reports/{id}")]
    public async Task<IActionResult> DeleteReport(int id)
    {

        
        var report = await _db.Reports.FindAsync(id);
        if (report == null) return NotFound();
        
        _db.Reports.Remove(report);
        await _db.SaveChangesAsync();
        await LogActivity("DeleteReport", null, $"Deleted report ID: {id}");
        
        return Ok(new { message = "Report deleted." });
    }
    
    // GET: api/admin/interests
    [HttpGet("interests")]
    public async Task<ActionResult<List<InterestDto>>> GetInterests()
    {

        
        var interests = await _db.Interests
            .OrderBy(i => i.Category)
            .ThenBy(i => i.Name)
            .Select(i => new InterestDto(i.Id, i.Name, i.Emoji, i.Category))
            .ToListAsync();
            
        return Ok(interests);
    }
    
    // POST: api/admin/interests
    [HttpPost("interests")]
    public async Task<ActionResult<InterestDto>> CreateInterest([FromBody] CreateInterestRequest request)
    {

        
        var interest = new Interest
        {
            Name = request.Name,
            Emoji = request.Emoji,
            Category = request.Category
        };
        
        _db.Interests.Add(interest);
        await _db.SaveChangesAsync();
        await LogActivity("CreateInterest", null, $"Created: {request.Name}");
        
        return Ok(new InterestDto(interest.Id, interest.Name, interest.Emoji, interest.Category));
    }
    
    // PUT: api/admin/interests/{id}
    [HttpPut("interests/{id}")]
    public async Task<IActionResult> UpdateInterest(int id, [FromBody] UpdateInterestAdminRequest request)
    {

        
        var interest = await _db.Interests.FindAsync(id);
        if (interest == null) return NotFound();
        
        interest.Name = request.Name;
        interest.Emoji = request.Emoji;
        interest.Category = request.Category;
        
        await _db.SaveChangesAsync();
        await LogActivity("UpdateInterest", null, $"Updated: {request.Name}");
        
        return Ok(new { message = "Interest updated." });
    }
    
    // DELETE: api/admin/interests/{id}
    [HttpDelete("interests/{id}")]
    public async Task<IActionResult> DeleteInterest(int id)
    {

        
        var interest = await _db.Interests.FindAsync(id);
        if (interest == null) return NotFound();
        
        var name = interest.Name;
        _db.Interests.Remove(interest);
        await _db.SaveChangesAsync();
        await LogActivity("DeleteInterest", null, $"Deleted: {name}");
        
        return Ok(new { message = "Interest deleted." });
    }
    
    // GET: api/admin/prompts
    [HttpGet("prompts")]
    public async Task<ActionResult<List<PromptDto>>> GetPrompts()
    {

        
        var prompts = await _db.Prompts
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Question)
            .Select(p => new PromptDto(p.Id, p.Question, p.Category))
            .ToListAsync();
            
        return Ok(prompts);
    }
    
    // POST: api/admin/prompts
    [HttpPost("prompts")]
    public async Task<ActionResult<PromptDto>> CreatePrompt([FromBody] CreatePromptRequest request)
    {

        
        var prompt = new Prompt
        {
            Question = request.Question,
            Category = request.Category,
            IsActive = true
        };
        
        _db.Prompts.Add(prompt);
        await _db.SaveChangesAsync();
        await LogActivity("CreatePrompt", null, $"Created: {request.Question}");
        
        return Ok(new PromptDto(prompt.Id, prompt.Question, prompt.Category));
    }
    
    // PUT: api/admin/prompts/{id}
    [HttpPut("prompts/{id}")]
    public async Task<IActionResult> UpdatePrompt(int id, [FromBody] UpdatePromptAdminRequest request)
    {

        
        var prompt = await _db.Prompts.FindAsync(id);
        if (prompt == null) return NotFound();
        
        prompt.Question = request.Question;
        prompt.Category = request.Category;
        prompt.IsActive = request.IsActive;
        
        await _db.SaveChangesAsync();
        await LogActivity("UpdatePrompt", null, $"Updated: {request.Question}");
        
        return Ok(new { message = "Prompt updated." });
    }
    
    // DELETE: api/admin/prompts/{id}
    [HttpDelete("prompts/{id}")]
    public async Task<IActionResult> DeletePrompt(int id)
    {

        
        var prompt = await _db.Prompts.FindAsync(id);
        if (prompt == null) return NotFound();
        
        var question = prompt.Question;
        _db.Prompts.Remove(prompt);
        await _db.SaveChangesAsync();
        await LogActivity("DeletePrompt", null, $"Deleted: {question}");
        
        return Ok(new { message = "Prompt deleted." });
    }
    
    // GET: api/admin/logs
    [HttpGet("logs")]
    public async Task<ActionResult<List<ActivityLogDto>>> GetActivityLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {

        
        var logs = await _db.ActivityLogs
            .Include(l => l.Admin)
            .Include(l => l.TargetUser)
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new ActivityLogDto(
                l.Id,
                l.Admin != null ? l.Admin.Name : null,
                l.TargetUser != null ? l.TargetUser.Name : null,
                l.Action,
                l.Details,
                l.CreatedAt
            ))
            .ToListAsync();
            
        return Ok(logs);
    }
}
