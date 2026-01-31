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
        [FromQuery] int? maxDistance = null,  // km - only for premium/gold users
        [FromQuery] string? academicYears = null,  // comma-separated
        [FromQuery] string? majors = null,  // comma-separated
        [FromQuery] string? interests = null)  // comma-separated interest IDs
    {
        var userId = GetUserId();
        
        // Get user's subscription to check features
        var subscription = await _db.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.StudentId == userId);
        var plan = subscription?.Plan ?? "free";
        var planFeatures = SubscriptionPlans.Plans.GetValueOrDefault(plan) ?? SubscriptionPlans.Plans["free"];
        
        // OPTIMIZED: Get current user with only needed fields
        var currentUserData = await _db.Students
            .Where(s => s.Id == userId)
            .Select(s => new {
                s.MinAgePreference,
                s.MaxAgePreference,
                s.PreferredGender,
                s.University,
                s.Latitude,
                s.Longitude,
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
        
        // CAMPUS/DISTANCE FILTERING based on subscription
        // Free users: Same campus only
        // Premium/Gold: Cross-campus matching with distance filter
        if (!planFeatures.CrossCampusMatching)
        {
            // Free users - same university only
            if (!string.IsNullOrEmpty(currentUserData.University))
            {
                candidatesQuery = candidatesQuery.Where(s => 
                    s.University == currentUserData.University || string.IsNullOrEmpty(s.University)
                );
            }
        }
        // Note: Distance filtering is done in C# after fetch since SQL doesn't have Haversine
        
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
        
        // ADVANCED FILTERS - Only for Premium/Gold users
        if (planFeatures.AdvancedFilters)
        {
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
            
            // Apply interest filter
            if (!string.IsNullOrEmpty(interests))
            {
                var interestIdsList = interests.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => int.TryParse(i.Trim(), out var id) ? id : 0)
                    .Where(id => id > 0)
                    .ToList();
                    
                if (interestIdsList.Any())
                {
                    candidatesQuery = candidatesQuery.Where(s =>
                        s.Interests.Any(si => interestIdsList.Contains(si.InterestId))
                    );
                }
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
                s.IsBoosted,
                s.BoostExpiresAt,
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
        
        // DISTANCE FILTERING (for Premium/Gold users with location data)
        var effectiveMaxDistance = planFeatures.CrossCampusMatching 
            ? Math.Min(maxDistance ?? planFeatures.MaxDistanceKm, planFeatures.MaxDistanceKm)
            : 0;
            
        if (effectiveMaxDistance > 0 && currentUserData.Latitude.HasValue && currentUserData.Longitude.HasValue)
        {
            candidates = candidates.Where(c =>
            {
                if (!c.Latitude.HasValue || !c.Longitude.HasValue) return true; // Include users without location
                
                var distance = CalculateDistanceKm(
                    currentUserData.Latitude.Value, currentUserData.Longitude.Value,
                    c.Latitude.Value, c.Longitude.Value
                );
                
                return distance <= effectiveMaxDistance;
            }).ToList();
        }
        
        // Score and rank candidates (done in C# after fetching minimal data)
        var now = DateTime.UtcNow;
        var scored = candidates.Select(c =>
        {
            var theirInterestIds = c.InterestIds.ToHashSet();
            var sharedInterests = myInterestIds.Intersect(theirInterestIds).Count();
            
            int score = 0;
            
            // Boosted profiles appear first (if boost is active)
            if (c.IsBoosted && c.BoostExpiresAt.HasValue && c.BoostExpiresAt > now)
            {
                score += 1000;  // Boosted priority
            }
            
            score += sharedInterests * 10;  // 10 points per shared interest
            score += c.University == currentUserData.University ? 20 : 0;  // Same university bonus
            score += c.LastActiveAt > now.AddDays(-1) ? 15 : 0;  // Recently active bonus
            score += c.LastActiveAt > now.AddDays(-7) ? 5 : 0;  // Active in week bonus
            
            // Calculate distance for display (if both have location)
            double? distanceKm = null;
            if (currentUserData.Latitude.HasValue && currentUserData.Longitude.HasValue &&
                c.Latitude.HasValue && c.Longitude.HasValue)
            {
                distanceKm = CalculateDistanceKm(
                    currentUserData.Latitude.Value, currentUserData.Longitude.Value,
                    c.Latitude.Value, c.Longitude.Value
                );
            }
            
            return (Candidate: c, Score: score, DistanceKm: distanceKm);
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
    
    // Haversine formula to calculate distance between two coordinates
    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in km
        
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        return R * c;
    }
    
    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
    
    
    
    
    
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
