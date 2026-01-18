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
public class PromptsController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    
    public PromptsController(CampusMatchDbContext db)
    {
        _db = db;
    }
    
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    // Get all available prompts
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<PromptDto>>> GetAllPrompts()
    {
        var prompts = await _db.Prompts
            .Where(p => p.IsActive)
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Question)
            .Select(p => new PromptDto(p.Id, p.Question, p.Category))
            .ToListAsync();
        
        return Ok(prompts);
    }
    
    // Get current user's prompt answers
    [HttpGet("me")]
    public async Task<ActionResult<List<StudentPromptDto>>> GetMyPrompts()
    {
        var userId = GetUserId();
        
        var prompts = await _db.StudentPrompts
            .Include(sp => sp.Prompt)
            .Where(sp => sp.StudentId == userId)
            .OrderBy(sp => sp.DisplayOrder)
            .Select(sp => new StudentPromptDto(
                sp.Id,
                sp.PromptId,
                sp.Prompt.Question,
                sp.Answer,
                sp.DisplayOrder
            ))
            .ToListAsync();
        
        return Ok(prompts);
    }
    
    // Add or update a prompt answer
    [HttpPost]
    public async Task<ActionResult<StudentPromptDto>> AddOrUpdatePrompt(UpdatePromptRequest request)
    {
        var userId = GetUserId();
        
        // Verify prompt exists
        var prompt = await _db.Prompts.FindAsync(request.PromptId);
        if (prompt == null || !prompt.IsActive)
            return BadRequest("Invalid prompt.");
        
        // Check if already answered
        var existing = await _db.StudentPrompts
            .FirstOrDefaultAsync(sp => sp.StudentId == userId && sp.PromptId == request.PromptId);
        
        if (existing != null)
        {
            // Update existing
            existing.Answer = request.Answer;
            existing.CreatedAt = DateTime.UtcNow;
        }
        else
        {
            // Check limit (max 3 prompts)
            var count = await _db.StudentPrompts.CountAsync(sp => sp.StudentId == userId);
            if (count >= 3)
                return BadRequest("Maximum 3 prompts allowed. Delete one first.");
            
            existing = new StudentPrompt
            {
                StudentId = userId,
                PromptId = request.PromptId,
                Answer = request.Answer,
                DisplayOrder = count
            };
            _db.StudentPrompts.Add(existing);
        }
        
        await _db.SaveChangesAsync();
        
        return Ok(new StudentPromptDto(
            existing.Id,
            existing.PromptId,
            prompt.Question,
            existing.Answer,
            existing.DisplayOrder
        ));
    }
    
    // Update multiple prompts at once
    [HttpPut]
    public async Task<ActionResult<List<StudentPromptDto>>> UpdatePrompts(UpdatePromptsRequest request)
    {
        var userId = GetUserId();
        
        if (request.Prompts.Count > 3)
            return BadRequest("Maximum 3 prompts allowed.");
        
        // Remove existing prompts
        var existing = await _db.StudentPrompts.Where(sp => sp.StudentId == userId).ToListAsync();
        _db.StudentPrompts.RemoveRange(existing);
        
        // Add new prompts
        var order = 0;
        foreach (var promptReq in request.Prompts)
        {
            var prompt = await _db.Prompts.FindAsync(promptReq.PromptId);
            if (prompt == null || !prompt.IsActive)
                continue;
            
            _db.StudentPrompts.Add(new StudentPrompt
            {
                StudentId = userId,
                PromptId = promptReq.PromptId,
                Answer = promptReq.Answer,
                DisplayOrder = order++
            });
        }
        
        await _db.SaveChangesAsync();
        
        // Return updated list
        var prompts = await _db.StudentPrompts
            .Include(sp => sp.Prompt)
            .Where(sp => sp.StudentId == userId)
            .OrderBy(sp => sp.DisplayOrder)
            .Select(sp => new StudentPromptDto(
                sp.Id,
                sp.PromptId,
                sp.Prompt.Question,
                sp.Answer,
                sp.DisplayOrder
            ))
            .ToListAsync();
        
        return Ok(prompts);
    }
    
    // Delete a prompt answer
    [HttpDelete("{promptId}")]
    public async Task<IActionResult> DeletePrompt(int promptId)
    {
        var userId = GetUserId();
        
        var studentPrompt = await _db.StudentPrompts
            .FirstOrDefaultAsync(sp => sp.StudentId == userId && sp.PromptId == promptId);
        
        if (studentPrompt == null)
            return NotFound("Prompt not found.");
        
        _db.StudentPrompts.Remove(studentPrompt);
        await _db.SaveChangesAsync();
        
        return Ok(new { message = "Prompt removed." });
    }
}
