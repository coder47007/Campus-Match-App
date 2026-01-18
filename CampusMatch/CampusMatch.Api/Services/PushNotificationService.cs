// Push Notification Service for Expo
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using CampusMatch.Api.Data;
using CampusMatch.Api.Models;

namespace CampusMatch.Api.Services;

public interface IPushNotificationService
{
    Task SendMatchNotification(int userId, string matchName, int matchId);
    Task SendMessageNotification(int userId, string senderName, string messagePreview, int matchId);
    Task SendSuperLikeNotification(int userId, string likerName);
    Task SendEventReminderNotification(int userId, string eventTitle, int eventId);
}

public class PushNotificationService : IPushNotificationService
{
    private readonly CampusMatchDbContext _db;
    private readonly HttpClient _httpClient;
    private readonly ILogger<PushNotificationService> _logger;
    private const string EXPO_PUSH_URL = "https://exp.host/--/api/v2/push/send";

    public PushNotificationService(CampusMatchDbContext db, HttpClient httpClient, ILogger<PushNotificationService> logger)
    {
        _db = db;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task SendMatchNotification(int userId, string matchName, int matchId)
    {
        await SendNotification(userId, new ExpoPushMessage
        {
            Title = "New Match! 💖",
            Body = $"You matched with {matchName}! Say hi!",
            Data = new { type = "match", matchId }
        });
    }

    public async Task SendMessageNotification(int userId, string senderName, string messagePreview, int matchId)
    {
        var preview = messagePreview.Length > 50 
            ? messagePreview.Substring(0, 47) + "..." 
            : messagePreview;

        await SendNotification(userId, new ExpoPushMessage
        {
            Title = senderName,
            Body = preview,
            Data = new { type = "message", matchId }
        });
    }

    public async Task SendSuperLikeNotification(int userId, string likerName)
    {
        await SendNotification(userId, new ExpoPushMessage
        {
            Title = "Someone Super Liked you! ⭐",
            Body = $"{likerName} really likes your profile!",
            Data = new { type = "superlike" }
        });
    }

    public async Task SendEventReminderNotification(int userId, string eventTitle, int eventId)
    {
        await SendNotification(userId, new ExpoPushMessage
        {
            Title = "Event Starting Soon! 📅",
            Body = $"{eventTitle} is about to begin",
            Data = new { type = "event", eventId }
        });
    }

    private async Task SendNotification(int userId, ExpoPushMessage message)
    {
        try
        {
            // Get user's push token
            var student = await _db.Students.FindAsync(userId);
            if (student?.PushNotificationToken == null || !student.PushNotificationToken.StartsWith("ExponentPushToken"))
            {
                _logger.LogDebug("No valid push token for user {UserId}", userId);
                return;
            }

            message.To = student.PushNotificationToken;
            message.Sound = "default";
            message.Priority = "high";

            var response = await _httpClient.PostAsJsonAsync(EXPO_PUSH_URL, message);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to send push notification to user {UserId}: {Error}", userId, errorContent);
            }
            else
            {
                _logger.LogInformation("Push notification sent to user {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification to user {UserId}", userId);
        }
    }

    // Send to multiple users
    public async Task SendBulkNotifications(IEnumerable<int> userIds, ExpoPushMessage baseMessage)
    {
        var tokens = await _db.Students
            .Where(s => userIds.Contains(s.Id) && s.PushNotificationToken != null)
            .Select(s => s.PushNotificationToken!)
            .ToListAsync();

        if (!tokens.Any()) return;

        var messages = tokens.Select(token => new ExpoPushMessage
        {
            To = token,
            Title = baseMessage.Title,
            Body = baseMessage.Body,
            Data = baseMessage.Data,
            Sound = "default",
            Priority = "high"
        }).ToList();

        try
        {
            var response = await _httpClient.PostAsJsonAsync(EXPO_PUSH_URL, messages);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to send bulk push notifications: {Error}", errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending bulk push notifications");
        }
    }
}

public class ExpoPushMessage
{
    [JsonPropertyName("to")]
    public string To { get; set; } = string.Empty;
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;
    
    [JsonPropertyName("data")]
    public object? Data { get; set; }
    
    [JsonPropertyName("sound")]
    public string? Sound { get; set; }
    
    [JsonPropertyName("priority")]
    public string? Priority { get; set; }
    
    [JsonPropertyName("badge")]
    public int? Badge { get; set; }
}
