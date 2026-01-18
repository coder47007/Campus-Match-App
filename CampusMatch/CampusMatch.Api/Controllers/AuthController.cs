using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using CampusMatch.Api.Data;
using CampusMatch.Api.Models;
using CampusMatch.Api.Services;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    private readonly IJwtService _jwt;
    private readonly IEmailService _email;
    
    public AuthController(CampusMatchDbContext db, IJwtService jwt, IEmailService email)
    {
        _db = db;
        _jwt = jwt;
        _email = email;
    }
    
    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        // Validate @mybvc.ca email
        if (!request.Email.EndsWith("@mybvc.ca", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only @mybvc.ca email addresses are allowed.");
        }
        
        // Validate password strength
        var passwordError = ValidatePassword(request.Password);
        if (passwordError != null)
        {
            return BadRequest(passwordError);
        }
        
        // Validate phone number
        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            return BadRequest("Phone number is required.");
        }
        
        if (!Regex.IsMatch(request.PhoneNumber, @"^[\d\s\-\+\(\)]{10,20}$"))
        {
            return BadRequest("Please enter a valid phone number.");
        }
        
        // Check if email exists
        if (await _db.Students.AnyAsync(s => s.Email == request.Email))
        {
            return BadRequest("Email already registered.");
        }
        
        var student = new Student
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            InstagramHandle = request.InstagramHandle
        };
        
        _db.Students.Add(student);
        
        var refreshToken = GenerateSecureToken();
        
        student.RefreshToken = refreshToken;
        student.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        student.LastActiveAt = DateTime.UtcNow;
        
        await _db.SaveChangesAsync();
        
        // Generate JWT token AFTER saving so student.Id is populated
        var token = _jwt.GenerateToken(student);
        
        // Send verification email (fire and forget)
        var verifyToken = GenerateSecureToken();
        _ = _email.SendVerificationEmailAsync(student.Email, verifyToken);
        
        return Ok(new AuthResponse(token, refreshToken, MapToDto(student)));
    }
    
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var student = await _db.Students.FirstOrDefaultAsync(s => s.Email == request.Email);
        
        if (student == null || !BCrypt.Net.BCrypt.Verify(request.Password, student.PasswordHash))
        {
            return Unauthorized("Invalid email or password.");
        }
        
        // Check if user is banned
        if (student.IsBanned)
        {
            return Unauthorized($"Your account has been suspended. Reason: {student.BanReason ?? "Violation of community guidelines"}");
        }
        
        // Generate tokens
        var token = _jwt.GenerateToken(student);
        var refreshToken = GenerateSecureToken();
        
        student.RefreshToken = refreshToken;
        student.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        student.LastActiveAt = DateTime.UtcNow;
        
        // Reset super likes if needed
        if (DateTime.UtcNow >= student.SuperLikesResetAt)
        {
            student.SuperLikesRemaining = 3;
            student.SuperLikesResetAt = DateTime.UtcNow.Date.AddDays(1);
        }
        
        await _db.SaveChangesAsync();
        
        return Ok(new AuthResponse(token, refreshToken, MapToDto(student), student.IsAdmin));
    }
    
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> RefreshToken(RefreshTokenRequest request)
    {
        var student = await _db.Students.FirstOrDefaultAsync(s => 
            s.RefreshToken == request.RefreshToken && 
            s.RefreshTokenExpiry > DateTime.UtcNow);
        
        if (student == null)
        {
            return Unauthorized("Invalid or expired refresh token.");
        }
        
        // Generate new tokens
        var token = _jwt.GenerateToken(student);
        var refreshToken = GenerateSecureToken();
        
        student.RefreshToken = refreshToken;
        student.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        student.LastActiveAt = DateTime.UtcNow;
        
        await _db.SaveChangesAsync();
        
        return Ok(new AuthResponse(token, refreshToken, MapToDto(student)));
    }
    
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var student = await _db.Students.FindAsync(userId);
        
        if (student != null)
        {
            student.RefreshToken = null;
            student.RefreshTokenExpiry = null;
            await _db.SaveChangesAsync();
        }
        
        return Ok(new { message = "Logged out successfully." });
    }
    
    [HttpPost("forgot-password")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var student = await _db.Students.FirstOrDefaultAsync(s => s.Email == request.Email);
        
        // Always return success to prevent email enumeration
        if (student == null)
        {
            return Ok(new { message = "If the email exists, a reset link has been sent." });
        }
        
        // Generate reset token (store in RefreshToken field temporarily)
        var resetToken = GenerateSecureToken();
        student.RefreshToken = $"RESET:{resetToken}";
        student.RefreshTokenExpiry = DateTime.UtcNow.AddHours(1);
        
        await _db.SaveChangesAsync();
        
        // Send email
        await _email.SendPasswordResetEmailAsync(student.Email, resetToken);
        
        return Ok(new { message = "If the email exists, a reset link has been sent." });
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var student = await _db.Students.FirstOrDefaultAsync(s => 
            s.RefreshToken == $"RESET:{request.Token}" && 
            s.RefreshTokenExpiry > DateTime.UtcNow);
        
        if (student == null)
        {
            return BadRequest("Invalid or expired reset token.");
        }
        
        // Validate new password
        var passwordError = ValidatePassword(request.NewPassword);
        if (passwordError != null)
        {
            return BadRequest(passwordError);
        }
        
        // Update password
        student.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        student.RefreshToken = null;
        student.RefreshTokenExpiry = null;
        
        await _db.SaveChangesAsync();
        
        return Ok(new { message = "Password reset successfully. Please log in." });
    }
    
    [HttpGet("verify")]
    public IActionResult VerifyEmail([FromQuery] string token)
    {
        // For now, just return a success page
        // In production, verify and mark email as verified
        return Content(@"
            <html>
            <body style='background: linear-gradient(135deg, #1e1b4b, #7c3aed); color: white; 
                         font-family: Arial; display: flex; align-items: center; justify-content: center; 
                         height: 100vh; margin: 0;'>
                <div style='text-align: center;'>
                    <h1>✅ Email Verified!</h1>
                    <p>Your email has been verified. You can now close this window.</p>
                </div>
            </body>
            </html>", "text/html");
    }
    
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var student = await _db.Students.FindAsync(userId);
        
        if (student == null)
            return NotFound();
        
        // Verify current password
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, student.PasswordHash))
        {
            return BadRequest("Current password is incorrect.");
        }
        
        // Validate new password
        var passwordError = ValidatePassword(request.NewPassword);
        if (passwordError != null)
        {
            return BadRequest(passwordError);
        }
        
        // Don't allow same password
        if (BCrypt.Net.BCrypt.Verify(request.NewPassword, student.PasswordHash))
        {
            return BadRequest("New password must be different from current password.");
        }
        
        // Update password
        student.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        
        // Invalidate all refresh tokens for security
        student.RefreshToken = null;
        student.RefreshTokenExpiry = null;
        
        await _db.SaveChangesAsync();
        
        return Ok(new { message = "Password changed successfully. Please log in again." });
    }
    
    [HttpDelete("account")]
    [Authorize]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequest request)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var student = await _db.Students
            .Include(s => s.Photos)
            .Include(s => s.Sessions)
            .FirstOrDefaultAsync(s => s.Id == userId);
        
        if (student == null)
            return NotFound();
        
        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, student.PasswordHash))
        {
            return BadRequest("Incorrect password.");
        }
        
        // Delete all related data
        // Photos
        _db.Photos.RemoveRange(student.Photos);
        
        // Student interests
        var interests = await _db.StudentInterests.Where(si => si.StudentId == userId).ToListAsync();
        _db.StudentInterests.RemoveRange(interests);
        
        // Student prompts
        var prompts = await _db.StudentPrompts.Where(sp => sp.StudentId == userId).ToListAsync();
        _db.StudentPrompts.RemoveRange(prompts);
        
        // Sessions
        _db.Sessions.RemoveRange(student.Sessions);
        
        // Swipes (made and received)
        var swipes = await _db.Swipes.Where(s => s.SwiperId == userId || s.SwipedId == userId).ToListAsync();
        _db.Swipes.RemoveRange(swipes);
        
        // Matches and messages
        var matches = await _db.Matches
            .Include(m => m.Messages)
            .Where(m => m.Student1Id == userId || m.Student2Id == userId)
            .ToListAsync();
        foreach (var match in matches)
        {
            _db.Messages.RemoveRange(match.Messages);
        }
        _db.Matches.RemoveRange(matches);
        
        // Reports (made by user)
        var reportsMade = await _db.Reports.Where(r => r.ReporterId == userId).ToListAsync();
        _db.Reports.RemoveRange(reportsMade);
        
        // Blocks (both directions)
        var blocks = await _db.Blocks.Where(b => b.BlockerId == userId || b.BlockedId == userId).ToListAsync();
        _db.Blocks.RemoveRange(blocks);
        
        // Finally delete the student
        _db.Students.Remove(student);
        
        await _db.SaveChangesAsync();
        
        return Ok(new { message = "Account deleted successfully." });
    }
    
    [HttpPost("push-token")]
    [Authorize]
    public async Task<ActionResult> SavePushToken([FromBody] PushTokenRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);
        var student = await _db.Students.FindAsync(userId);
        if (student == null) return NotFound();
        student.PushNotificationToken = request.Token;
        await _db.SaveChangesAsync();
        return Ok();
    }
    
    private static string? ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return "Password is required.";
        
        if (password.Length < 8)
            return "Password must be at least 8 characters long.";
        
        if (!Regex.IsMatch(password, @"[A-Za-z]"))
            return "Password must contain at least one letter.";
        
        if (!Regex.IsMatch(password, @"\d"))
            return "Password must contain at least one number.";
        
        return null;
    }
    
    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
    
    private static StudentDto MapToDto(Student s) => new(
        s.Id, s.Email, s.Name, s.Age, s.Major, s.Year, s.Bio, s.PhotoUrl, s.University, s.Gender, s.PreferredGender, s.PhoneNumber, s.InstagramHandle
    );
}

public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string NewPassword);
public record PushTokenRequest(string Token);
