using System.ComponentModel.DataAnnotations;

namespace CampusMatch.Shared.DTOs;

// Auth DTOs
public record RegisterRequest(
    [Required, EmailAddress, StringLength(100)] string Email, 
    [Required, StringLength(100, MinimumLength = 6)] string Password, 
    [Required, StringLength(50, MinimumLength = 2)] string Name, 
    [Required, Phone] string PhoneNumber, 
    [StringLength(30)] string? InstagramHandle = null
);

public record LoginRequest(
    [Required, EmailAddress] string Email, 
    [Required] string Password
);

public record AuthResponse(string Token, string? RefreshToken, StudentDto Student, bool IsAdmin = false);
public record RefreshTokenRequest([Required] string RefreshToken);

public record ChangePasswordRequest(
    [Required] string CurrentPassword, 
    [Required, StringLength(100, MinimumLength = 6)] string NewPassword
);

public record DeleteAccountRequest([Required] string Password);


// Student Profile DTOs
public record StudentDto(
    int Id,
    string Email,
    string Name,
    int? Age,
    string? Major,
    string? Year,
    string? Bio,
    string? PhotoUrl,
    string? University,
    string? Gender,
    string? PreferredGender,
    string? PhoneNumber,
    string? InstagramHandle,
    double? Latitude = null,
    double? Longitude = null,
    List<InterestDto>? Interests = null,
    List<PhotoDto>? Photos = null,
    List<StudentPromptDto>? Prompts = null
);

public record ProfileUpdateRequest(
    string Name,
    int? Age,
    string? Major,
    string? Year,
    string? Bio,
    string? PhotoUrl,
    string? University,
    string? Gender,
    string? PreferredGender,
    string? PhoneNumber,
    string? InstagramHandle,
    double? Latitude = null,
    double? Longitude = null
);

// Photo DTOs
public record PhotoDto(int Id, string Url, bool IsPrimary, int DisplayOrder);

// Interest DTOs
public record InterestDto(int Id, string Name, string Emoji, string Category);
public record UpdateInterestsRequest(List<int> InterestIds);

// Prompt DTOs
public record PromptDto(int Id, string Question, string Category);
public record StudentPromptDto(int Id, int PromptId, string Question, string Answer, int DisplayOrder);
public record UpdatePromptRequest(int PromptId, string Answer);
public record UpdatePromptsRequest(List<UpdatePromptRequest> Prompts);

// Swipe DTOs
public record SwipeRequest(int SwipedId, bool IsLike, bool IsSuperLike = false);
public record SwipeResponse(bool IsMatch, MatchDto? Match);
public record UndoSwipeResponse(bool Success, string? Message);

// Likes DTOs (Who Liked You)
public record LikePreviewDto(int Id, string? BlurredPhotoUrl, string FirstLetter, bool IsSuperLike, DateTime LikedAt);

// Match DTOs
public record MatchDto(
    int Id,
    int OtherStudentId,
    string OtherStudentName,
    string? OtherStudentPhotoUrl,
    string? OtherStudentMajor,
    DateTime MatchedAt,
    bool IsActive = true
);

// Message DTOs with read receipts
public record MessageDto(
    int Id,
    int SenderId,
    string SenderName,
    string Content,
    DateTime SentAt,
    bool IsRead,
    DateTime? DeliveredAt = null,
    DateTime? ReadAt = null
);

public record SendMessageRequest(
    [Range(1, int.MaxValue)] int MatchId, 
    [Required, StringLength(5000, MinimumLength = 1)] string Content
);

// Settings DTOs
public record SettingsDto(
    int MinAgePreference,
    int MaxAgePreference,
    int MaxDistancePreference,
    bool ShowOnlineStatus,
    bool NotifyOnMatch,
    bool NotifyOnMessage,
    bool NotifyOnSuperLike,
    bool IsProfileHidden
);

public record UpdateSettingsRequest(
    int? MinAgePreference = null,
    int? MaxAgePreference = null,
    int? MaxDistancePreference = null,
    bool? ShowOnlineStatus = null,
    bool? NotifyOnMatch = null,
    bool? NotifyOnMessage = null,
    bool? NotifyOnSuperLike = null,
    bool? IsProfileHidden = null
);

// Session DTOs
public record SessionDto(int Id, string? DeviceInfo, string? IpAddress, DateTime CreatedAt, DateTime LastActiveAt, bool IsCurrent);

// Report & Block DTOs
public record ReportRequest(
    [Range(1, int.MaxValue)] int ReportedId, 
    [Required, StringLength(200, MinimumLength = 3)] string Reason, 
    [StringLength(2000)] string? Details = null,
    string? Source = null  // "chat" or "discover" - determines auto-unmatch or auto-dislike
);
public record BlockRequest([Range(1, int.MaxValue)] int BlockedId);
public record BlockedUserDto(int Id, string Name, string? PhotoUrl, DateTime BlockedAt);

// Admin DTOs
public record AdminStatsDto(
    int TotalUsers,
    int ActiveUsers,
    int BannedUsers,
    int TotalMatches,
    int TotalMessages,
    int PendingReports,
    int TotalReports,
    int TotalInterests,
    int TotalPrompts
);

public record AdminUserDto(
    int Id,
    string Email,
    string Name,
    int? Age,
    string? Major,
    string? PhotoUrl,
    DateTime CreatedAt,
    DateTime LastActiveAt,
    bool IsAdmin,
    bool IsBanned,
    string? BanReason,
    int MatchCount,
    int ReportCount
);

public record BanUserRequest(string Reason);

public record AdminReportDto(
    int Id,
    int ReporterId,
    string ReporterName,
    int ReportedId,
    string ReportedName,
    string? ReportedPhotoUrl,
    string Reason,
    string? Details,
    DateTime CreatedAt,
    bool IsReviewed
);

public record ActivityLogDto(
    int Id,
    string? AdminName,
    string? TargetUserName,
    string Action,
    string? Details,
    DateTime CreatedAt
);

public record CreateInterestRequest(
    [Required, StringLength(50, MinimumLength = 2)] string Name, 
    [Required, StringLength(10)] string Emoji, 
    [Required, StringLength(50)] string Category
);
public record UpdateInterestAdminRequest(
    [Required, StringLength(50, MinimumLength = 2)] string Name, 
    [Required, StringLength(10)] string Emoji, 
    [Required, StringLength(50)] string Category
);
public record CreatePromptRequest(
    [Required, StringLength(500, MinimumLength = 10)] string Question, 
    [Required, StringLength(50)] string Category
);
public record UpdatePromptAdminRequest(
    [Required, StringLength(500, MinimumLength = 10)] string Question, 
    [Required, StringLength(50)] string Category, 
    bool IsActive
);

