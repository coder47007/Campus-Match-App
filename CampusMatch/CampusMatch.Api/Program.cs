using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Hangfire;
using Hangfire.MemoryStorage;
using CampusMatch.Api.Data;
using CampusMatch.Api.Hubs;
using CampusMatch.Api.Services;
using CampusMatch.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// DATABASE CONFIGURATION
// ========================================
builder.Services.AddDbContext<CampusMatchDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository Pattern - Phase 1.1
builder.Services.AddScoped<CampusMatch.Api.Repositories.IUnitOfWork, CampusMatch.Api.Repositories.UnitOfWork>();
builder.Services.AddScoped<CampusMatch.Api.Repositories.IStudentRepository, CampusMatch.Api.Repositories.StudentRepository>();
builder.Services.AddScoped<CampusMatch.Api.Repositories.ISwipeRepository, CampusMatch.Api.Repositories.SwipeRepository>();
builder.Services.AddScoped<CampusMatch.Api.Repositories.IMatchRepository, CampusMatch.Api.Repositories.MatchRepository>();
builder.Services.AddScoped<CampusMatch.Api.Repositories.IMessageRepository, CampusMatch.Api.Repositories.MessageRepository>();

// ========================================
// CACHING LAYER
// ========================================
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

// ========================================
// BACKGROUND JOBS (Hangfire)
// ========================================
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseMemoryStorage()); // Use Redis in production: UseRedisStorage(redisConnection)

builder.Services.AddHangfireServer();
builder.Services.AddSingleton<CampusMatch.Api.Services.BackgroundJobs.IBackgroundJobService, CampusMatch.Api.Services.BackgroundJobs.HangfireBackgroundJobService>();
builder.Services.AddScoped<CampusMatch.Api.Services.BackgroundJobs.MatchJobService>();
builder.Services.AddScoped<CampusMatch.Api.Services.BackgroundJobs.UserJobService>();

// ========================================
// JWT AUTHENTICATION - PRODUCTION READY
// ========================================
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

// ========================================
// RATE LIMITING - Production Ready
// ========================================
builder.Services.AddRateLimiter(options =>
{
    // Global rate limit per IP - higher for production
    var isProduction = !builder.Environment.IsDevelopment();
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = isProduction ? 500 : 100, // Higher limit for production
                Window = TimeSpan.FromMinutes(1)
            }));
    
    // Stricter limit for auth endpoints (prevent brute force)
    options.AddFixedWindowLimiter("auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.AutoReplenishment = true;
    });
    
    // Stricter limit for swipes
    options.AddFixedWindowLimiter("swipe", limiterOptions =>
    {
        limiterOptions.PermitLimit = isProduction ? 100 : 50;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.AutoReplenishment = true;
    });
    
    // Upload limit (prevent abuse)
    options.AddFixedWindowLimiter("upload", limiterOptions =>
    {
        limiterOptions.PermitLimit = 20;
        limiterOptions.Window = TimeSpan.FromMinutes(5);
        limiterOptions.AutoReplenishment = true;
    });
    
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ========================================
// SERVICES REGISTRATION
// ========================================
builder.Services.AddScoped<IJwtService, JwtService>();

// Blob Storage - use local for development, Supabase for production
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IBlobStorageService, LocalFileStorageService>();
    builder.Services.AddSingleton<IEmailService, DevEmailService>();
}
else
{
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<IBlobStorageService, SupabaseStorageService>();
    // Using DevEmailService in production as requested - emails will be logged but not sent
    builder.Services.AddSingleton<IEmailService, DevEmailService>(); 
}

// Push Notification Service (uses HttpClient for Expo push API)
builder.Services.AddHttpClient<IPushNotificationService, PushNotificationService>();

// ========================================
// HEALTH CHECKS
// ========================================
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "sql", "postgresql" })
    .AddCheck("self", () => HealthCheckResult.Healthy("API is running"), tags: new[] { "self" });

// ========================================
// CONTROLLERS & SIGNALR
// ========================================
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

// ========================================
// CORS - Environment Aware
// ========================================
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Development: Allow all origins for easier testing
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    }
    else
    {
        // Production: Restrict to known origins
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
            ?? new[] { "https://campusmatch.edu", "https://www.campusmatch.edu" };
        
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); // Required for SignalR
        });
    }
});

// ========================================
// SWAGGER (Development Only)
// ========================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "CampusMatch API", 
        Version = "v1",
        Description = "Campus dating app API - connecting students across universities"
    });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ========================================
// DATABASE INITIALIZATION (Production-Safe)
// ========================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CampusMatchDbContext>();
    try 
    {
        if (app.Environment.IsDevelopment())
        {
            // In development, auto-apply pending migrations
            db.Database.Migrate();
        }
        else
        {
            // In production, just verify connection - migrations should be run separately
            // Use: dotnet ef database update --environment Production
            if (!db.Database.CanConnect())
            {
                throw new Exception("Cannot connect to production database");
            }
            Console.WriteLine("Production database connection verified.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization failed: {ex.Message}. Continuing startup...");
    }
}

// ========================================
// MIDDLEWARE PIPELINE
// ========================================

// Add global exception handler first to catch all errors
app.UseGlobalExceptionHandler();

// Add request logging for performance monitoring
app.UseRequestLogging();

// HTTPS Redirection (Production only)
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Security Headers (Production)
if (!app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        // Prevent MIME-type sniffing
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        
        // Prevent clickjacking
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        
        // Enable XSS filter
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        
        // Referrer policy
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        
        // Content Security Policy (adjust as needed)
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; img-src 'self' https: data:; style-src 'self' 'unsafe-inline';");
        
        // Strict Transport Security
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        
        await next();
    });
}

// Swagger (Development only)
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

// ========================================
// ENDPOINTS
// ========================================

// Health Check Endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("self"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds
            })
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("self"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { status = "Healthy" });
    }
});

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
