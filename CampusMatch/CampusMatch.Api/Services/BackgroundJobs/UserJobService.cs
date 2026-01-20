using CampusMatch.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusMatch.Api.Services.BackgroundJobs;

/// <summary>
/// Background job for user-related async tasks
/// </summary>
public class UserJobService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserJobService> _logger;

    public UserJobService(IServiceProvider serviceProvider, ILogger<UserJobService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Update user's last active status
    /// </summary>
    public async Task UpdateLastActiveAsync(int studentId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CampusMatchDbContext>();
        
        var student = await db.Students.FindAsync(studentId);
        if (student != null)
        {
            student.LastActiveAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            _logger.LogDebug("Updated last active for student {StudentId}", studentId);
        }
    }

    /// <summary>
    /// Clean up expired sessions
    /// </summary>
    public async Task CleanupExpiredSessionsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CampusMatchDbContext>();
        
        var expiredCutoff = DateTime.UtcNow.AddDays(-30);
        var expiredSessions = await db.Sessions
            .Where(s => s.LastActiveAt < expiredCutoff || s.IsRevoked || s.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();
            
        if (expiredSessions.Any())
        {
            db.Sessions.RemoveRange(expiredSessions);
            await db.SaveChangesAsync();
            _logger.LogInformation("Cleaned up {Count} expired sessions", expiredSessions.Count);
        }
    }

    /// <summary>
    /// Reset daily limits (super likes, rewinds) for all users
    /// </summary>
    public async Task ResetDailyLimitsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CampusMatchDbContext>();
        
        var now = DateTime.UtcNow;
        var usersToReset = await db.Students
            .Where(s => s.SuperLikesResetAt <= now || s.RewindsResetAt <= now)
            .ToListAsync();
            
        foreach (var user in usersToReset)
        {
            if (user.SuperLikesResetAt <= now)
            {
                // Default values - TODO: Check subscription status for premium users
                user.SuperLikesRemaining = 3;
                user.SuperLikesResetAt = now.Date.AddDays(1);
            }
            
            if (user.RewindsResetAt <= now)
            {
                // Default values - TODO: Check subscription status for premium users
                user.RewindsRemaining = 1;
                user.RewindsResetAt = now.Date.AddDays(1);
            }
        }
        
        if (usersToReset.Any())
        {
            await db.SaveChangesAsync();
            _logger.LogInformation("Reset daily limits for {Count} users", usersToReset.Count);
        }
    }

    /// <summary>
    /// Calculate profile completion percentage
    /// Returns the completion percentage (0-100)
    /// </summary>
    public async Task<int> CalculateProfileCompletionAsync(int studentId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CampusMatchDbContext>();
        
        var student = await db.Students
            .Include(s => s.Photos)
            .Include(s => s.Interests)
            .Include(s => s.Prompts)
            .FirstOrDefaultAsync(s => s.Id == studentId);
            
        if (student == null) return 0;
        
        int completedFields = 0;
        int totalFields = 10;
        
        if (!string.IsNullOrEmpty(student.Name)) completedFields++;
        if (student.Age.HasValue) completedFields++;
        if (!string.IsNullOrEmpty(student.Bio)) completedFields++;
        if (!string.IsNullOrEmpty(student.Major)) completedFields++;
        if (!string.IsNullOrEmpty(student.Year)) completedFields++;
        if (!string.IsNullOrEmpty(student.Gender)) completedFields++;
        if (!string.IsNullOrEmpty(student.PreferredGender)) completedFields++;
        if (student.Photos.Count >= 1) completedFields++;
        if (student.Interests.Count >= 3) completedFields++;
        if (student.Prompts.Count >= 1) completedFields++;
        
        var percentage = (completedFields * 100) / totalFields;
        
        _logger.LogDebug("Calculated profile completion {Percentage}% for student {StudentId}", 
            percentage, studentId);
            
        return percentage;
    }
}
