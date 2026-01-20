using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using Hangfire;
using Hangfire.MemoryStorage;
using CampusMatch.Api.Data;
using CampusMatch.Api.Hubs;
using CampusMatch.Api.Services;
using CampusMatch.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<CampusMatchDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository Pattern - Phase 1.1
builder.Services.AddScoped<CampusMatch.Api.Repositories.IUnitOfWork, CampusMatch.Api.Repositories.UnitOfWork>();
builder.Services.AddScoped<CampusMatch.Api.Repositories.IStudentRepository, CampusMatch.Api.Repositories.StudentRepository>();
builder.Services.AddScoped<CampusMatch.Api.Repositories.ISwipeRepository, CampusMatch.Api.Repositories.SwipeRepository>();
builder.Services.AddScoped<CampusMatch.Api.Repositories.IMatchRepository, CampusMatch.Api.Repositories.MatchRepository>();
builder.Services.AddScoped<CampusMatch.Api.Repositories.IMessageRepository, CampusMatch.Api.Repositories.MessageRepository>();

// Caching Layer - Phase 1.2
var redisConnection = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnection))
{
    // Use Redis for distributed caching in production
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnection;
        options.InstanceName = "CampusMatch:";
    });
    builder.Services.AddSingleton<ICacheService, RedisCacheService>();
}
else
{
    // Fallback to in-memory cache for development
    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
}

// Background Jobs - Phase 1.3
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseMemoryStorage()); // Use Redis in production: UseRedisStorage(redisConnection)

builder.Services.AddHangfireServer();
builder.Services.AddSingleton<CampusMatch.Api.Services.BackgroundJobs.IBackgroundJobService, CampusMatch.Api.Services.BackgroundJobs.HangfireBackgroundJobService>();
builder.Services.AddScoped<CampusMatch.Api.Services.BackgroundJobs.MatchJobService>();
builder.Services.AddScoped<CampusMatch.Api.Services.BackgroundJobs.UserJobService>();

// JWT Authentication - PRODUCTION READY
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

// API Versioning - Phase 1.5
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = Asp.Versioning.ApiVersionReader.Combine(
        new Asp.Versioning.UrlSegmentApiVersionReader(),
        new Asp.Versioning.HeaderApiVersionReader("X-Api-Version")
    );
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "CampusMatch API", 
        Version = "v1",
        Description = "Campus dating app API - connecting students across universities"
    });
});

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
