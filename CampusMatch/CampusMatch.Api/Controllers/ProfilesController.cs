using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CampusMatch.Api.Data;
using CampusMatch.Api.Models;
using CampusMatch.Api.Services;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    private readonly ICacheService _cache;
    
    public ProfilesController(CampusMatchDbContext db, ICacheService cache)
    {
        _db = db;
        _cache = cache;
    }
    
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpGet("me")]
    public async Task<ActionResult<StudentDto>> GetMyProfile()
    {
        var userId = GetUserId();
        var cacheKey = CacheKeys.StudentProfile(userId);
        
        // Try cache first
        var cachedProfile = await _cache.GetAsync<StudentDto>(cacheKey);
        if (cachedProfile != null)
        {
            return Ok(cachedProfile);
        }
        
        var student = await _db.Students
            .Include(s => s.Interests)
                .ThenInclude(si => si.Interest)
            .Include(s => s.Photos.OrderBy(p => p.DisplayOrder))
            .Include(s => s.Prompts)
                .ThenInclude(sp => sp.Prompt)
            .FirstOrDefaultAsync(s => s.Id == userId);
            
        if (student == null) return NotFound();
        
        var dto = MapToDto(student);
        await _cache.SetAsync(cacheKey, dto, CacheKeys.Expiration.Medium);
        
        return Ok(dto);
    }
    
    [HttpPut("me")]
    public async Task<ActionResult<StudentDto>> UpdateMyProfile(ProfileUpdateRequest request)
    {
        var userId = GetUserId();
        var student = await _db.Students
            .Include(s => s.Interests)
                .ThenInclude(si => si.Interest)
            .Include(s => s.Photos)
            .Include(s => s.Prompts)
                .ThenInclude(sp => sp.Prompt)
            .FirstOrDefaultAsync(s => s.Id == userId);
        if (student == null) return NotFound();
        
        student.Name = request.Name;
        student.Age = request.Age;
        student.Major = request.Major;
        student.Year = request.Year;
        student.Bio = request.Bio;
        student.PhotoUrl = request.PhotoUrl;
        student.University = request.University;
        student.Gender = request.Gender;
        student.PreferredGender = request.PreferredGender;
        student.PhoneNumber = request.PhoneNumber ?? student.PhoneNumber;
        student.InstagramHandle = request.InstagramHandle;
        student.Latitude = request.Latitude;
        student.Longitude = request.Longitude;
        student.LastActiveAt = DateTime.UtcNow;
        
        await _db.SaveChangesAsync();
        
        // Invalidate cache after update
        await _cache.RemoveAsync(CacheKeys.StudentProfile(userId));
        await _cache.RemoveAsync(CacheKeys.Student(userId));
        
        var dto = MapToDto(student);
        return Ok(dto);
    }
    
    // PHASE 3: Added to replace direct database access from client
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMyAccount()
    {
        var userId = GetUserId();
        var student = await _db.Students
            .Include(s => s.Photos)
            .Include(s => s.Prompts)
            .Include(s => s.Interests)
            .FirstOrDefaultAsync(s => s.Id == userId);
            
        if (student == null) return NotFound();
        
        // Business logic: Delete related data (cascade will handle most, but explicit for clarity)
        // Note: Matches, Swipes, Messages will be cascade deleted by database constraints
        
        _db.Students.Remove(student);
        await _db.SaveChangesAsync();
        
        // Invalidate all cached data for this user
        await _cache.RemoveAsync(CacheKeys.StudentProfile(userId));
        await _cache.RemoveAsync(CacheKeys.Student(userId));
        await _cache.RemoveAsync(CacheKeys.StudentMatches(userId));
        
        return NoContent();
    }
    
    [HttpGet("discover")]
    public async Task<ActionResult<List<StudentDto>>> DiscoverProfiles(
        [FromQuery] int? minAge = null,
        [FromQuery] int? maxAge = null,
        [FromQuery] string? gender = null,
        [FromQuery] int? maxDistance = null,
        [FromQuery] string? academicYears = null,  // comma-separated
        [FromQuery] string? majors = null)  // comma-separated
    {
        var userId = GetUserId();
        
        // OPTIMIZED: Get current user with only needed fields
        var currentUserData = await _db.Students
            .Where(s => s.Id == userId)
            .Select(s => new {
                s.MinAgePreference,
                s.MaxAgePreference,
                s.PreferredGender,
                s.University,
                InterestIds = s.Interests.Select(i => i.InterestId).ToList()
            })
            .FirstOrDefaultAsync();
            
        if (currentUserData == null) return NotFound();
        
        var myInterestIds = currentUserData.InterestIds.ToHashSet();
        
        // OPTIMIZED: Single query to get all excluded IDs (swipes + blocks)
        var excludedIds = await _db.Swipes
            .Where(s => s.SwiperId == userId)
            .Select(s => s.SwipedId)
            .Union(_db.Blocks
                .Where(b => b.BlockerId == userId || b.BlockedId == userId)
                .Select(b => b.BlockerId == userId ? b.BlockedId : b.BlockerId))
            .ToListAsync();
        
        var excludedIdsSet = excludedIds.ToHashSet();
        
        // OPTIMIZED: Build WHERE clause in database, not C#
        var candidatesQuery = _db.Students
            .Where(s => s.Id != userId && !excludedIdsSet.Contains(s.Id) && !s.IsAdmin)
            .Where(s => !s.IsProfileHidden && !s.IsBanned);
        
        // Apply age filter (use provided params or user preferences)
        var filterMinAge = minAge ?? currentUserData.MinAgePreference;
        var filterMaxAge = maxAge ?? currentUserData.MaxAgePreference;
        
        if (filterMinAge > 0 || filterMaxAge > 0)
        {
            candidatesQuery = candidatesQuery.Where(s => 
                !s.Age.HasValue || // Include users without age set
                (s.Age >= filterMinAge && s.Age <= filterMaxAge)
            );
        }
        
        // Apply gender preference filter (use provided param or user preference)
        var filterGender = gender ?? currentUserData.PreferredGender;
        if (!string.IsNullOrEmpty(filterGender) && filterGender != "Everyone" && filterGender != "everyone")
        {
            // Normalize gender values
            var normalizedGender = filterGender.ToLower() == "men" ? "Male" : 
                                   filterGender.ToLower() == "women" ? "Female" : 
                                   filterGender;
            
            candidatesQuery = candidatesQuery.Where(s => 
                s.Gender == normalizedGender || string.IsNullOrEmpty(s.Gender)
            );
        }
        
        // Apply academic year filter
        if (!string.IsNullOrEmpty(academicYears))
        {
            var yearsList = academicYears.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(y => y.Trim())
                .ToList();
                
            if (yearsList.Any())
            {
                candidatesQuery = candidatesQuery.Where(s => 
                    string.IsNullOrEmpty(s.Year) || yearsList.Contains(s.Year)
                );
            }
        }
        
        // Apply major filter
        if (!string.IsNullOrEmpty(majors))
        {
            var majorsList = majors.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(m => m.Trim().ToLower())
                .ToList();
                
            if (majorsList.Any())
            {
                candidatesQuery = candidatesQuery.Where(s => 
                    string.IsNullOrEmpty(s.Major) || 
                    majorsList.Any(m => s.Major.ToLower().Contains(m))
                );
            }
        }
        
        // OPTIMIZED: Use projection to fetch only needed data in single query
        var candidates = await candidatesQuery
            .Select(s => new {
                s.Id,
                s.Email,
                s.Name,
                s.Age,
                s.Major,
                s.Year,
                s.Bio,
                s.PhotoUrl,
                s.University,
                s.Gender,
                s.PreferredGender,
                s.PhoneNumber,
                s.InstagramHandle,
                s.Latitude,
                s.Longitude,
                s.LastActiveAt,
                InterestIds = s.Interests.Select(i => i.InterestId).ToList(),
                Interests = s.Interests.Select(si => new { 
                    si.Interest.Id, 
                    si.Interest.Name, 
                    si.Interest.Emoji, 
                    si.Interest.Category 
                }).ToList(),
                Photos = s.Photos.OrderBy(p => p.DisplayOrder).Select(p => new {
                    p.Id,
                    p.Url,
                    p.IsPrimary,
                    p.DisplayOrder
                }).ToList(),
                Prompts = s.Prompts.OrderBy(sp => sp.DisplayOrder).Select(sp => new {
                    sp.Id,
                    sp.PromptId,
                    sp.Prompt.Question,
                    sp.Answer,
                    sp.DisplayOrder
                }).ToList()
            })
            .ToListAsync();
        
        // Score and rank candidates (done in C# after fetching minimal data)
        var scored = candidates.Select(c =>
        {
            var theirInterestIds = c.InterestIds.ToHashSet();
            var sharedInterests = myInterestIds.Intersect(theirInterestIds).Count();
            
            int score = 0;
            score += sharedInterests * 10;  // 10 points per shared interest
            score += c.University == currentUserData.University ? 20 : 0;  // Same university bonus
            score += c.LastActiveAt > DateTime.UtcNow.AddDays(-1) ? 15 : 0;  // Recently active bonus
            score += c.LastActiveAt > DateTime.UtcNow.AddDays(-7) ? 5 : 0;  // Active in week bonus
            
            return (Candidate: c, Score: score);
        })
        .OrderByDescending(x => x.Score)
        .ThenBy(_ => Guid.NewGuid())  // Randomize within same score
        .Take(20)
        .Select(x => new StudentDto(
            x.Candidate.Id,
            x.Candidate.Email,
            x.Candidate.Name,
            x.Candidate.Age,
            x.Candidate.Major,
            x.Candidate.Year,
            x.Candidate.Bio,
            x.Candidate.PhotoUrl,
            x.Candidate.University,
            x.Candidate.Gender,
            x.Candidate.PreferredGender,
            x.Candidate.PhoneNumber,
            x.Candidate.InstagramHandle,
            x.Candidate.Latitude,
            x.Candidate.Longitude,
            x.Candidate.Interests.Select(i => new InterestDto(i.Id, i.Name, i.Emoji, i.Category)).ToList(),
            x.Candidate.Photos.Select(p => new PhotoDto(p.Id, p.Url, p.IsPrimary, p.DisplayOrder)).ToList(),
            x.Candidate.Prompts.Select(p => new StudentPromptDto(p.Id, p.PromptId, p.Question, p.Answer, p.DisplayOrder)).ToList()
        ))
        .ToList();
        
        return Ok(scored);
    }
    
    
    
    
    
    [HttpGet("interests")]
    [AllowAnonymous]
    public async Task<ActionResult<List<InterestDto>>> GetAllInterests()
    {
        var interests = await _db.Interests
            .OrderBy(i => i.Category)
            .ThenBy(i => i.Name)
            .Select(i => new InterestDto(i.Id, i.Name, i.Emoji, i.Category))
            .ToListAsync();
            
        return Ok(interests);
    }
    
    [HttpPut("interests")]
    public async Task<ActionResult<StudentDto>> UpdateInterests([FromBody] UpdateInterestsRequest request)
    {
        var userId = GetUserId();
        
        // Remove existing interests
        var existing = await _db.StudentInterests.Where(si => si.StudentId == userId).ToListAsync();
        _db.StudentInterests.RemoveRange(existing);
        
        // Add new interests (max 10)
        var interestIds = request.InterestIds.Take(10).ToList();
        foreach (var interestId in interestIds)
        {
            _db.StudentInterests.Add(new StudentInterest
            {
                StudentId = userId,
                InterestId = interestId
            });
        }
        
        await _db.SaveChangesAsync();
        
        var student = await _db.Students
            .Include(s => s.Interests)
                .ThenInclude(si => si.Interest)
            .FirstOrDefaultAsync(s => s.Id == userId);
            
        return Ok(MapToDto(student!));
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<StudentDto>> GetProfile(int id)
    {
        var userId = GetUserId();
        
        // Check if blocked
        var isBlocked = await _db.Blocks.AnyAsync(b =>
            (b.BlockerId == userId && b.BlockedId == id) ||
            (b.BlockerId == id && b.BlockedId == userId));
            
        if (isBlocked)
            return NotFound("Profile not found.");
        
        var student = await _db.Students
            .Include(s => s.Interests)
                .ThenInclude(si => si.Interest)
            .Include(s => s.Prompts)
                .ThenInclude(sp => sp.Prompt)
            .Include(s => s.Photos)
            .FirstOrDefaultAsync(s => s.Id == id);
            
        if (student == null) return NotFound();
        return Ok(MapToDto(student));
    }
    
    private static StudentDto MapToDto(Student s) => new(
        s.Id, s.Email, s.Name, s.Age, s.Major, s.Year, s.Bio, s.PhotoUrl, s.University, s.Gender, s.PreferredGender,
        s.PhoneNumber, s.InstagramHandle,
        s.Latitude, s.Longitude,
        s.Interests?.Select(si => new InterestDto(si.Interest.Id, si.Interest.Name, si.Interest.Emoji, si.Interest.Category)).ToList(),
        s.Photos?.OrderBy(p => p.DisplayOrder).Select(p => new PhotoDto(p.Id, p.Url, p.IsPrimary, p.DisplayOrder)).ToList(),
        s.Prompts?.OrderBy(sp => sp.DisplayOrder).Select(sp => new StudentPromptDto(sp.Id, sp.PromptId, sp.Prompt.Question, sp.Answer, sp.DisplayOrder)).ToList()
    );
}
