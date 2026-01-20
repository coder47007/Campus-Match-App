using CampusMatch.Api.Data;
using CampusMatch.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CampusMatch.Api.Services.BackgroundJobs;

/// <summary>
/// Background job for handling match-related async tasks
/// </summary>
public class MatchJobService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MatchJobService> _logger;

    public MatchJobService(IServiceProvider serviceProvider, ILogger<MatchJobService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Calculate compatibility score between two users after they match
    /// Returns the compatibility score (0-100)
    /// </summary>
    public async Task<int> CalculateCompatibilityScoreAsync(int matchId)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CampusMatchDbContext>();
        
        var match = await db.Matches
            .Include(m => m.Student1)
                .ThenInclude(s => s.Interests)
            .Include(m => m.Student2)
                .ThenInclude(s => s.Interests)
            .FirstOrDefaultAsync(m => m.Id == matchId);
            
        if (match == null)
        {
            _logger.LogWarning("Match {MatchId} not found for compatibility calculation", matchId);
            return 0;
        }
        
        // Calculate compatibility based on shared interests
        var student1InterestIds = match.Student1.Interests.Select(i => i.InterestId).ToHashSet();
        var student2InterestIds = match.Student2.Interests.Select(i => i.InterestId).ToHashSet();
        
        var sharedInterests = student1InterestIds.Intersect(student2InterestIds).Count();
        var totalUniqueInterests = student1InterestIds.Union(student2InterestIds).Count();
        
        // Jaccard similarity coefficient
        var compatibilityScore = totalUniqueInterests > 0 
            ? (double)sharedInterests / totalUniqueInterests * 100 
            : 50; // Default if no interests
            
        var score = (int)Math.Round(compatibilityScore);
        
        _logger.LogInformation("Calculated compatibility score {Score}% for match {MatchId}", 
            score, matchId);
            
        return score;
    }

    /// <summary>
    /// Send notification to both users about a new match
    /// </summary>
    public async Task SendMatchNotificationAsync(int matchId, int student1Id, int student2Id)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CampusMatchDbContext>();
        
        var students = await db.Students
            .Where(s => s.Id == student1Id || s.Id == student2Id)
            .ToListAsync();
            
        foreach (var student in students)
        {
            // TODO: Integrate with Firebase Cloud Messaging when FcmToken is added to Student entity
            // For now, just log that we would send a notification
            _logger.LogInformation("Would send push notification to student {StudentId} ({Name}) about match {MatchId}", 
                student.Id, student.Name, matchId);
        }
    }
}
