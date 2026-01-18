using System.ComponentModel.DataAnnotations;

namespace CampusMatch.Api.Models;

public class Student
{
    public int Id { get; set; }
    
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public int? Age { get; set; }
    
    [MaxLength(100)]
    public string? Major { get; set; }
    
    [MaxLength(50)]
    public string? Year { get; set; }  // Freshman, Sophomore, Junior, Senior, Graduate
    
    [MaxLength(500)]
    public string? Bio { get; set; }
    
    public string? PhotoUrl { get; set; }
    
    [MaxLength(200)]
    public string? University { get; set; }
    
    [MaxLength(20)]
    public string? Gender { get; set; }
    
    [MaxLength(20)]
    public string? PreferredGender { get; set; }
    
    [Required, MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? InstagramHandle { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;
    
    public int SuperLikesRemaining { get; set; } = 3;
    public DateTime SuperLikesResetAt { get; set; } = DateTime.UtcNow.Date.AddDays(1);
    
    // Refresh token for security
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    
    // Rewind feature - undo last swipe
    public int RewindsRemaining { get; set; } = 1;
    public DateTime RewindsResetAt { get; set; } = DateTime.UtcNow.Date.AddDays(1);
    
    // Location-based matching
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    // Discovery preferences (persisted settings)
    public int MinAgePreference { get; set; } = 18;
    public int MaxAgePreference { get; set; } = 30;
    public int MaxDistancePreference { get; set; } = 25;  // miles
    public bool ShowOnlineStatus { get; set; } = true;
    
    // Notification preferences
    public bool NotifyOnMatch { get; set; } = true;
    public bool NotifyOnMessage { get; set; } = true;
    public bool NotifyOnSuperLike { get; set; } = true;
    public string? PushNotificationToken { get; set; }  // Expo push token
    
    // Profile visibility
    public bool IsProfileHidden { get; set; } = false;
    
    // Admin and moderation
    public bool IsAdmin { get; set; } = false;
    public bool IsBanned { get; set; } = false;
    public DateTime? BannedAt { get; set; }
    public string? BanReason { get; set; }
    
    // Premium boost feature
    public bool IsBoosted { get; set; } = false;
    public DateTime? BoostExpiresAt { get; set; }

    
    // Navigation properties
    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    public ICollection<Swipe> SwipesMade { get; set; } = new List<Swipe>();
    public ICollection<Swipe> SwipesReceived { get; set; } = new List<Swipe>();
    public ICollection<StudentInterest> Interests { get; set; } = new List<StudentInterest>();
    public ICollection<Block> BlockedUsers { get; set; } = new List<Block>();
    public ICollection<Block> BlockedByUsers { get; set; } = new List<Block>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<StudentPrompt> Prompts { get; set; } = new List<StudentPrompt>();
}

public class Photo
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    
    [Required]
    public string Url { get; set; } = string.Empty;
    
    [Required]
    public string BlobName { get; set; } = string.Empty;  // For cloud storage
    
    public bool IsPrimary { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public Student Student { get; set; } = null!;
}

public class Interest
{
    public int Id { get; set; }
    
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(10)]
    public string Emoji { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;  // Sports, Music, Arts, etc.
    
    // Navigation
    public ICollection<StudentInterest> Students { get; set; } = new List<StudentInterest>();
}

public class StudentInterest
{
    public int StudentId { get; set; }
    public int InterestId { get; set; }
    
    // Navigation
    public Student Student { get; set; } = null!;
    public Interest Interest { get; set; } = null!;
}

public class Swipe
{
    public int Id { get; set; }
    public int SwiperId { get; set; }
    public int SwipedId { get; set; }
    public bool IsLike { get; set; }
    public bool IsSuperLike { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Student Swiper { get; set; } = null!;
    public Student Swiped { get; set; } = null!;
}

public class Match
{
    public int Id { get; set; }
    public int Student1Id { get; set; }
    public int Student2Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;  // For unmatch functionality
    
    // Navigation properties
    public Student Student1 { get; set; } = null!;
    public Student Student2 { get; set; } = null!;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

public class Message
{
    public int Id { get; set; }
    public int MatchId { get; set; }
    public int SenderId { get; set; }
    
    [Required, MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
    
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    
    // Read receipts & delivery status
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsRead { get; set; } = false;
    
    // Navigation properties
    public Match Match { get; set; } = null!;
    public Student Sender { get; set; } = null!;
}

public class Report
{
    public int Id { get; set; }
    public int ReporterId { get; set; }
    public int ReportedId { get; set; }
    
    [Required, MaxLength(50)]
    public string Reason { get; set; } = string.Empty;  // Inappropriate, Spam, Harassment, Fake, Other
    
    [MaxLength(500)]
    public string? Details { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsReviewed { get; set; } = false;
    
    // Navigation
    public Student Reporter { get; set; } = null!;
    public Student Reported { get; set; } = null!;
}

public class Block
{
    public int Id { get; set; }
    public int BlockerId { get; set; }
    public int BlockedId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public Student Blocker { get; set; } = null!;
    public Student Blocked { get; set; } = null!;
}

// Session management - track active sessions
public class Session
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? DeviceInfo { get; set; }
    
    [MaxLength(50)]
    public string? IpAddress { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    
    // Navigation
    public Student Student { get; set; } = null!;
}

// Profile prompts - Tinder/Hinge style questions
public class Prompt
{
    public int Id { get; set; }
    
    [Required, MaxLength(200)]
    public string Question { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;  // About Me, Dating, Fun, etc.
    
    public bool IsActive { get; set; } = true;
    
    // Navigation
    public ICollection<StudentPrompt> StudentPrompts { get; set; } = new List<StudentPrompt>();
}

// Student's answers to prompts
public class StudentPrompt
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int PromptId { get; set; }
    
    [Required, MaxLength(500)]
    public string Answer { get; set; } = string.Empty;
    
    public int DisplayOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public Student Student { get; set; } = null!;
    public Prompt Prompt { get; set; } = null!;
}

// Activity log for admin actions
public class ActivityLog
{
    public int Id { get; set; }
    public int? AdminId { get; set; }
    public int? TargetUserId { get; set; }
    
    [Required, MaxLength(50)]
    public string Action { get; set; } = string.Empty;  // BanUser, UnbanUser, DeleteUser, AddInterest, etc.
    
    [MaxLength(500)]
    public string? Details { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public Student? Admin { get; set; }
    public Student? TargetUser { get; set; }
}
