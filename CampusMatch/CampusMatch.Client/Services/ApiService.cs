using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Client.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private string? _token;
    private string? _refreshToken;
    private StudentDto? _currentStudent;
    private const int MaxRetries = 3;
    private const int RetryDelayMs = 1000;
    
    public StudentDto? CurrentStudent => _currentStudent;
    public bool IsLoggedIn => !string.IsNullOrEmpty(_token);
    public bool IsOffline { get; private set; }
    
    public event Action<string>? OnError;
    public event Action<bool>? OnConnectionStatusChanged;
    
    public ApiService()
    {
        _http = new HttpClient 
        { 
            BaseAddress = new Uri("http://localhost:5229"),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }
    
    public void SetToken(string token, string? refreshToken, StudentDto student)
    {
        _token = token;
        _refreshToken = refreshToken;
        _currentStudent = student;
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    
    public string? GetToken() => _token;
    
    public void Logout()
    {
        _token = null;
        _refreshToken = null;
        _currentStudent = null;
        _http.DefaultRequestHeaders.Authorization = null;
    }
    
    private async Task<HttpResponseMessage?> ExecuteWithRetryAsync(Func<Task<HttpResponseMessage>> request, string operation)
    {
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                var response = await request();
                
                if (IsOffline)
                {
                    IsOffline = false;
                    OnConnectionStatusChanged?.Invoke(true);
                }
                
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                
                // Don't retry client errors (4xx)
                if (response.StatusCode >= HttpStatusCode.BadRequest && 
                    response.StatusCode < HttpStatusCode.InternalServerError)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    OnError?.Invoke($"{operation} failed: {error}");
                    return response;
                }
                
                // Retry server errors (5xx)
                if (attempt < MaxRetries)
                {
                    await Task.Delay(RetryDelayMs * attempt);
                }
            }
            catch (HttpRequestException ex)
            {
                if (attempt == MaxRetries)
                {
                    IsOffline = true;
                    OnConnectionStatusChanged?.Invoke(false);
                    OnError?.Invoke($"Connection error: {ex.Message}");
                    return null;
                }
                await Task.Delay(RetryDelayMs * attempt);
            }
            catch (TaskCanceledException)
            {
                if (attempt == MaxRetries)
                {
                    OnError?.Invoke($"{operation} timed out");
                    return null;
                }
                await Task.Delay(RetryDelayMs * attempt);
            }
        }
        
        return null;
    }
    
    // Auth
    public async Task<AuthResponse?> RegisterAsync(string email, string password, string name, string phoneNumber, string? instagramHandle = null)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, password, name, phoneNumber, instagramHandle)),
            "Registration");
            
        if (response == null || !response.IsSuccessStatusCode) return null;
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (auth != null) SetToken(auth.Token, auth.RefreshToken, auth.Student);
        return auth;
    }
    
    public async Task<AuthResponse?> LoginAsync(string email, string password)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, password)),
            "Login");
            
        if (response == null || !response.IsSuccessStatusCode) return null;
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (auth != null) SetToken(auth.Token, auth.RefreshToken, auth.Student);
        return auth;
    }
    
    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsJsonAsync("/api/auth/forgot-password", new { Email = email }),
            "Forgot password");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsJsonAsync("/api/auth/reset-password", new { Token = token, NewPassword = newPassword }),
            "Reset password");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    // Profiles
    public async Task<StudentDto?> GetMyProfileAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/profiles/me"),
            "Get profile");
            
        if (response == null || !response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<StudentDto>();
    }
    
    public async Task<StudentDto?> UpdateProfileAsync(ProfileUpdateRequest request)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PutAsJsonAsync("/api/profiles/me", request),
            "Update profile");
            
        if (response == null || !response.IsSuccessStatusCode) return null;
        _currentStudent = await response.Content.ReadFromJsonAsync<StudentDto>();
        return _currentStudent;
    }
    
    public async Task<List<StudentDto>> DiscoverProfilesAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/profiles/discover"),
            "Discover profiles");
            
        if (response == null || !response.IsSuccessStatusCode) return new List<StudentDto>();
        return await response.Content.ReadFromJsonAsync<List<StudentDto>>() ?? new List<StudentDto>();
    }
    
    // Interests
    public async Task<List<InterestDto>> GetAllInterestsAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/profiles/interests"),
            "Get interests");
            
        if (response == null || !response.IsSuccessStatusCode) return new List<InterestDto>();
        return await response.Content.ReadFromJsonAsync<List<InterestDto>>() ?? new List<InterestDto>();
    }
    
    public async Task<StudentDto?> UpdateInterestsAsync(List<int> interestIds)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PutAsJsonAsync("/api/profiles/interests", new UpdateInterestsRequest(interestIds)),
            "Update interests");
            
        if (response == null || !response.IsSuccessStatusCode) return null;
        _currentStudent = await response.Content.ReadFromJsonAsync<StudentDto>();
        return _currentStudent;
    }
    
    // Photos
    public async Task<List<PhotoDto>> GetMyPhotosAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/photos"),
            "Get photos");
            
        if (response == null || !response.IsSuccessStatusCode) return new List<PhotoDto>();
        return await response.Content.ReadFromJsonAsync<List<PhotoDto>>() ?? new List<PhotoDto>();
    }
    
    public async Task<PhotoDto?> UploadPhotoAsync(Stream fileStream, string fileName)
    {
        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        content.Add(streamContent, "file", fileName);
        
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsync("/api/photos/upload", content),
            "Upload photo");
            
        if (response == null || !response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<PhotoDto>();
    }
    
    public async Task<bool> DeletePhotoAsync(int photoId)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.DeleteAsync($"/api/photos/{photoId}"),
            "Delete photo");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<bool> SetPrimaryPhotoAsync(int photoId)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PutAsync($"/api/photos/{photoId}/primary", null),
            "Set primary photo");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    // Swipes
    public async Task<SwipeResponse?> SwipeAsync(int swipedId, bool isLike, bool isSuperLike = false)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsJsonAsync("/api/swipes", new SwipeRequest(swipedId, isLike, isSuperLike)),
            "Swipe");
            
        if (response == null || !response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<SwipeResponse>();
    }
    
    // Matches
    public async Task<List<MatchDto>> GetMatchesAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/matches"),
            "Get matches");
            
        if (response == null || !response.IsSuccessStatusCode) return new List<MatchDto>();
        return await response.Content.ReadFromJsonAsync<List<MatchDto>>() ?? new List<MatchDto>();
    }
    
    public async Task<bool> UnmatchAsync(int matchId)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.DeleteAsync($"/api/matches/{matchId}"),
            "Unmatch");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    // Messages
    public async Task<List<MessageDto>> GetMessagesAsync(int matchId)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync($"/api/messages/{matchId}"),
            "Get messages");
            
        if (response == null || !response.IsSuccessStatusCode) return new List<MessageDto>();
        return await response.Content.ReadFromJsonAsync<List<MessageDto>>() ?? new List<MessageDto>();
    }
    
    public async Task<MessageDto?> SendMessageAsync(int matchId, string content)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsJsonAsync("/api/messages", new SendMessageRequest(matchId, content)),
            "Send message");
            
        if (response == null || !response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<MessageDto>();
    }
    
    public async Task<bool> MarkMessagesAsReadAsync(int matchId)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsync($"/api/messages/{matchId}/read", null),
            "Mark messages read");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    // Reports & Blocks
    public async Task<bool> ReportUserAsync(int userId, string reason, string? details = null, string? source = null)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsJsonAsync("/api/reports", new ReportRequest(userId, reason, details, source)),
            "Report user");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<bool> BlockUserAsync(int userId)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsJsonAsync("/api/reports/block", new BlockRequest(userId)),
            "Block user");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<bool> UnblockUserAsync(int userId)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.DeleteAsync($"/api/reports/block/{userId}"),
            "Unblock user");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<List<BlockedUserDto>> GetBlockedUsersAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/reports/blocked"),
            "Get blocked users");
            
        if (response == null || !response.IsSuccessStatusCode) return new List<BlockedUserDto>();
        return await response.Content.ReadFromJsonAsync<List<BlockedUserDto>>() ?? new List<BlockedUserDto>();
    }
    
    // Settings
    public async Task<SettingsDto?> GetSettingsAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/settings"),
            "Get settings");
            
        if (response == null || !response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<SettingsDto>();
    }
    
    public async Task<SettingsDto?> UpdateSettingsAsync(UpdateSettingsRequest request)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PutAsJsonAsync("/api/settings", request),
            "Update settings");
            
        if (response == null || !response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<SettingsDto>();
    }
    
    // Prompts
    public async Task<List<PromptDto>> GetAllPromptsAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/prompts"),
            "Get prompts");
            
        if (response == null || !response.IsSuccessStatusCode) return new List<PromptDto>();
        return await response.Content.ReadFromJsonAsync<List<PromptDto>>() ?? new List<PromptDto>();
    }
    
    public async Task<List<StudentPromptDto>> GetMyPromptsAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/prompts/me"),
            "Get my prompts");
            
        if (response == null || !response.IsSuccessStatusCode) return new List<StudentPromptDto>();
        return await response.Content.ReadFromJsonAsync<List<StudentPromptDto>>() ?? new List<StudentPromptDto>();
    }
    
    public async Task<StudentPromptDto?> UpdatePromptAsync(int promptId, string answer)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsJsonAsync("/api/prompts", new UpdatePromptRequest(promptId, answer)),
            "Update prompt");
            
        if (response == null || !response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<StudentPromptDto>();
    }
    
    public async Task<bool> DeletePromptAsync(int promptId)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.DeleteAsync($"/api/prompts/{promptId}"),
            "Delete prompt");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    // Likes (Who Liked You)
    public async Task<List<LikePreviewDto>> GetReceivedLikesAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/likes/received"),
            "Get received likes");
            
        if (response == null || !response.IsSuccessStatusCode) return new List<LikePreviewDto>();
        return await response.Content.ReadFromJsonAsync<List<LikePreviewDto>>() ?? new List<LikePreviewDto>();
    }
    
    public async Task<int> GetLikesCountAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/likes/count"),
            "Get likes count");
            
        if (response == null || !response.IsSuccessStatusCode) return 0;
        var result = await response.Content.ReadFromJsonAsync<JsonDocument>();
        return result?.RootElement.GetProperty("total").GetInt32() ?? 0;
    }
    
    // Undo Last Swipe
    public async Task<UndoSwipeResponse?> UndoLastSwipeAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.DeleteAsync("/api/swipes/last"),
            "Undo swipe");
            
        if (response == null || !response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<UndoSwipeResponse>();
    }
    
    public async Task<int> GetRewindsRemainingAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/swipes/rewinds"),
            "Get rewinds");
            
        if (response == null || !response.IsSuccessStatusCode) return 0;
        var result = await response.Content.ReadFromJsonAsync<JsonDocument>();
        return result?.RootElement.GetProperty("remaining").GetInt32() ?? 0;
    }
    
    // Sessions
    public async Task<List<SessionDto>> GetSessionsAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/sessions"),
            "Get sessions");
            
        if (response == null || !response.IsSuccessStatusCode) return new List<SessionDto>();
        return await response.Content.ReadFromJsonAsync<List<SessionDto>>() ?? new List<SessionDto>();
    }
    
    public async Task<bool> RevokeSessionAsync(int sessionId)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.DeleteAsync($"/api/sessions/{sessionId}"),
            "Revoke session");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<bool> RevokeAllSessionsAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.DeleteAsync("/api/sessions"),
            "Revoke all sessions");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    // Change Password & Delete Account
    public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsJsonAsync("/api/auth/change-password", new ChangePasswordRequest(currentPassword, newPassword)),
            "Change password");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<bool> DeleteAccountAsync(string password)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/auth/account")
            {
                Content = JsonContent.Create(new DeleteAccountRequest(password))
            }),
            "Delete account");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    // Admin API Methods
    public async Task<AdminStatsDto?> GetAdminStatsAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/admin/stats"),
            "Get admin stats");
            
        if (response == null || !response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AdminStatsDto>();
    }
    
    public async Task<List<AdminUserDto>> GetAdminUsersAsync(string? search = null, bool? banned = null)
    {
        var url = "/api/admin/users";
        var query = new List<string>();
        if (!string.IsNullOrEmpty(search)) query.Add($"search={Uri.EscapeDataString(search)}");
        if (banned.HasValue) query.Add($"banned={banned.Value}");
        if (query.Count > 0) url += "?" + string.Join("&", query);
        
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync(url),
            "Get admin users");
            
        if (response == null || !response.IsSuccessStatusCode) return new List<AdminUserDto>();
        return await response.Content.ReadFromJsonAsync<List<AdminUserDto>>() ?? new List<AdminUserDto>();
    }
    
    public async Task<AdminUserDto?> GetAdminUserAsync(int userId)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync($"/api/admin/users/{userId}"),
            "Get admin user");
            
        if (response == null || !response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AdminUserDto>();
    }
    
    public async Task<bool> AdminBanUserAsync(int userId, string reason)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsJsonAsync($"/api/admin/users/{userId}/ban", new BanUserRequest(reason)),
            "Ban user");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<bool> AdminUnbanUserAsync(int userId)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsync($"/api/admin/users/{userId}/unban", null),
            "Unban user");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<bool> AdminDeleteUserAsync(int userId)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.DeleteAsync($"/api/admin/users/{userId}"),
            "Delete user");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<List<AdminReportDto>> GetAdminReportsAsync(bool pendingOnly = true)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync($"/api/admin/reports?pendingOnly={pendingOnly}"),
            "Get admin reports");
            
        if (response == null || !response.IsSuccessStatusCode) return new List<AdminReportDto>();
        return await response.Content.ReadFromJsonAsync<List<AdminReportDto>>() ?? new List<AdminReportDto>();
    }
    
    public async Task<bool> ResolveReportAsync(int reportId)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsync($"/api/admin/reports/{reportId}/resolve", null),
            "Resolve report");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<bool> DeleteReportAsync(int reportId)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.DeleteAsync($"/api/admin/reports/{reportId}"),
            "Delete report");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<List<InterestDto>> GetAdminInterestsAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/admin/interests"),
            "Get admin interests");
            
        if (response == null || !response.IsSuccessStatusCode) return new List<InterestDto>();
        return await response.Content.ReadFromJsonAsync<List<InterestDto>>() ?? new List<InterestDto>();
    }
    
    public async Task<bool> CreateInterestAsync(string name, string emoji, string category)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsJsonAsync("/api/admin/interests", new CreateInterestRequest(name, emoji, category)),
            "Create interest");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<bool> UpdateInterestAsync(int id, string name, string emoji, string category)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PutAsJsonAsync($"/api/admin/interests/{id}", new UpdateInterestAdminRequest(name, emoji, category)),
            "Update interest");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<bool> DeleteInterestAsync(int id)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.DeleteAsync($"/api/admin/interests/{id}"),
            "Delete interest");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<List<PromptDto>> GetAdminPromptsAsync()
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync("/api/admin/prompts"),
            "Get admin prompts");
            
        if (response == null || !response.IsSuccessStatusCode) return new List<PromptDto>();
        return await response.Content.ReadFromJsonAsync<List<PromptDto>>() ?? new List<PromptDto>();
    }
    
    public async Task<bool> CreatePromptAsync(string question, string category)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PostAsJsonAsync("/api/admin/prompts", new CreatePromptRequest(question, category)),
            "Create prompt");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<bool> UpdatePromptAdminAsync(int id, string question, string category, bool isActive)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.PutAsJsonAsync($"/api/admin/prompts/{id}", new UpdatePromptAdminRequest(question, category, isActive)),
            "Update prompt");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<bool> DeletePromptAdminAsync(int id)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.DeleteAsync($"/api/admin/prompts/{id}"),
            "Delete prompt");
        return response?.IsSuccessStatusCode ?? false;
    }
    
    public async Task<List<ActivityLogDto>> GetActivityLogsAsync(int page = 1, int pageSize = 50)
    {
        var response = await ExecuteWithRetryAsync(
            () => _http.GetAsync($"/api/admin/logs?page={page}&pageSize={pageSize}"),
            "Get activity logs");
            
        if (response == null || !response.IsSuccessStatusCode) return new List<ActivityLogDto>();
        return await response.Content.ReadFromJsonAsync<List<ActivityLogDto>>() ?? new List<ActivityLogDto>();
    }
}
