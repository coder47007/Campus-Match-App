namespace CampusMatch.Api.Services;

/// <summary>
/// Cache key constants for consistent key naming
/// </summary>
public static class CacheKeys
{
    // Prefixes
    private const string StudentPrefix = "student:";
    private const string DiscoverPrefix = "discover:";
    private const string MatchesPrefix = "matches:";
    private const string ProfilePrefix = "profile:";
    private const string LikesPrefix = "likes:";
    
    // Student keys
    public static string Student(int studentId) => $"{StudentPrefix}{studentId}";
    public static string StudentProfile(int studentId) => $"{ProfilePrefix}{studentId}";
    
    // Discovery keys
    public static string DiscoverProfiles(int studentId) => $"{DiscoverPrefix}{studentId}";
    public static string DiscoverCount(int studentId) => $"{DiscoverPrefix}{studentId}:count";
    
    // Matches keys
    public static string StudentMatches(int studentId) => $"{MatchesPrefix}{studentId}";
    public static string Match(int matchId) => $"match:{matchId}";
    
    // Likes keys
    public static string LikesReceived(int studentId) => $"{LikesPrefix}{studentId}:received";
    public static string LikesCount(int studentId) => $"{LikesPrefix}{studentId}:count";
    
    // Patterns for bulk invalidation
    public static string StudentPattern(int studentId) => $"*{studentId}*";
    
    // Expiration times
    public static class Expiration
    {
        public static readonly TimeSpan Short = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan Medium = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan Long = TimeSpan.FromHours(1);
        public static readonly TimeSpan VeryLong = TimeSpan.FromHours(24);
    }
}
