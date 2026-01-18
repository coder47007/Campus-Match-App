using System.Net;
using System.Net.Mail;

namespace CampusMatch.Api.Services;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string email, string token);
    Task SendPasswordResetEmailAsync(string email, string token);
}

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<SmtpEmailService> _logger;
    
    public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
    {
        _config = config;
        _logger = logger;
    }
    
    public async Task SendVerificationEmailAsync(string email, string token)
    {
        var subject = "Verify your CampusMatch email";
        var baseUrl = _config["App:BaseUrl"] ?? "http://localhost:5229";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; background-color: #1e1b4b; color: white; padding: 40px;'>
                <div style='max-width: 500px; margin: 0 auto; background: linear-gradient(135deg, #7c3aed, #ec4899); border-radius: 20px; padding: 40px;'>
                    <h1 style='margin: 0 0 20px;'>🎓 Welcome to CampusMatch!</h1>
                    <p style='font-size: 16px; margin-bottom: 30px;'>
                        Thanks for signing up! Please verify your email address to start matching with other students.
                    </p>
                    <a href='{baseUrl}/api/auth/verify?token={token}' 
                       style='display: inline-block; background: white; color: #7c3aed; padding: 15px 30px; 
                              border-radius: 10px; text-decoration: none; font-weight: bold; font-size: 16px;'>
                        Verify Email
                    </a>
                    <p style='margin-top: 30px; font-size: 12px; color: rgba(255,255,255,0.7);'>
                        If you didn't create this account, please ignore this email.
                    </p>
                </div>
            </body>
            </html>";
        
        await SendEmailAsync(email, subject, body);
    }
    
    public async Task SendPasswordResetEmailAsync(string email, string token)
    {
        var subject = "Reset your CampusMatch password";
        var baseUrl = _config["App:BaseUrl"] ?? "http://localhost:5229";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; background-color: #1e1b4b; color: white; padding: 40px;'>
                <div style='max-width: 500px; margin: 0 auto; background: linear-gradient(135deg, #7c3aed, #ec4899); border-radius: 20px; padding: 40px;'>
                    <h1 style='margin: 0 0 20px;'>🔐 Password Reset</h1>
                    <p style='font-size: 16px; margin-bottom: 30px;'>
                        We received a request to reset your password. Click the button below to choose a new password.
                    </p>
                    <a href='{baseUrl}/reset-password?token={token}' 
                       style='display: inline-block; background: white; color: #7c3aed; padding: 15px 30px; 
                              border-radius: 10px; text-decoration: none; font-weight: bold; font-size: 16px;'>
                        Reset Password
                    </a>
                    <p style='margin-top: 30px; font-size: 12px; color: rgba(255,255,255,0.7);'>
                        This link expires in 1 hour. If you didn't request this, please ignore this email.
                    </p>
                </div>
            </body>
            </html>";
        
        await SendEmailAsync(email, subject, body);
    }
    
    private async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        try
        {
            var smtpHost = _config["Email:SmtpHost"] ?? "localhost";
            var smtpPort = int.Parse(_config["Email:SmtpPort"] ?? "25");
            var smtpUser = _config["Email:SmtpUser"];
            var smtpPass = _config["Email:SmtpPass"];
            var fromEmail = _config["Email:FromEmail"] ?? "noreply@campusmatch.edu";
            var fromName = _config["Email:FromName"] ?? "CampusMatch";
            
            using var client = new SmtpClient(smtpHost, smtpPort);
            
            if (!string.IsNullOrEmpty(smtpUser) && !string.IsNullOrEmpty(smtpPass))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                client.EnableSsl = true;
            }
            
            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(to);
            
            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent to {Email}: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            // Don't throw - we don't want email failures to break registration
        }
    }
}

// Dev-only service that logs emails instead of sending
public class DevEmailService : IEmailService
{
    private readonly ILogger<DevEmailService> _logger;
    
    public DevEmailService(ILogger<DevEmailService> logger)
    {
        _logger = logger;
    }
    
    public Task SendVerificationEmailAsync(string email, string token)
    {
        _logger.LogInformation("DEV EMAIL: Verification for {Email}, Token: {Token}", email, token);
        return Task.CompletedTask;
    }
    
    public Task SendPasswordResetEmailAsync(string email, string token)
    {
        _logger.LogInformation("DEV EMAIL: Password reset for {Email}, Token: {Token}", email, token);
        return Task.CompletedTask;
    }
}
