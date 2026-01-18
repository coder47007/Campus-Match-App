using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using CampusMatch.Api.Data;
using CampusMatch.Api.Hubs;
using CampusMatch.Api.Services;
using CampusMatch.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<CampusMatchDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
        
        // Allow SignalR to receive JWT from query string
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    // Global rate limit per IP
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
    
    // Stricter limit for auth endpoints
    options.AddFixedWindowLimiter("auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.AutoReplenishment = true;
    });
    
    // Stricter limit for swipes
    options.AddFixedWindowLimiter("swipe", limiterOptions =>
    {
        limiterOptions.PermitLimit = 50;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.AutoReplenishment = true;
    });
    
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Services
builder.Services.AddScoped<IJwtService, JwtService>();

// Blob Storage - use local for development, Azure for production
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IBlobStorageService, LocalFileStorageService>();
    builder.Services.AddSingleton<IEmailService, DevEmailService>();
}
else
{
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<IBlobStorageService, SupabaseStorageService>();
    builder.Services.AddSingleton<IEmailService, SmtpEmailService>();
}

// Push Notification Service (uses HttpClient for Expo push API)
builder.Services.AddHttpClient<IPushNotificationService, PushNotificationService>();


// Controllers & SignalR
builder.Services.AddControllers();
builder.Services.AddSignalR();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Auto-migrate and seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CampusMatchDbContext>();
    try 
    {
        db.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization failed: {ex.Message}. Continuing startup...");
    }
}

// Middleware
// Add global exception handler first to catch all errors
app.UseGlobalExceptionHandler();

// Add request logging for performance monitoring
app.UseRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Serve uploaded files (for local development)
var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "uploads");
if (!Directory.Exists(uploadsPath))
    Directory.CreateDirectory(uploadsPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
