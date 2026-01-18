using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampusMatch.Api.Data;

namespace CampusMatch.Api.Controllers;

/// <summary>
/// Health check endpoint for monitoring API status.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    private readonly ILogger<HealthController> _logger;

    public HealthController(CampusMatchDbContext db, ILogger<HealthController> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Basic health check - returns simple status without auth.
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Detailed health check including database connectivity.
    /// </summary>
    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailed()
    {
        var health = new HealthCheckResult
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Checks = new Dictionary<string, HealthCheckItem>()
        };

        // Check database connectivity
        try
        {
            var canConnect = await _db.Database.CanConnectAsync();
            health.Checks["database"] = new HealthCheckItem
            {
                Status = canConnect ? "Healthy" : "Unhealthy",
                Description = canConnect ? "Database connection successful" : "Database connection failed"
            };
            
            if (!canConnect)
            {
                health.Status = "Unhealthy";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check database connection failed");
            health.Checks["database"] = new HealthCheckItem
            {
                Status = "Unhealthy",
                Description = $"Database error: {ex.Message}"
            };
            health.Status = "Unhealthy";
        }

        // Check basic statistics
        try
        {
            var stats = new
            {
                TotalUsers = await _db.Students.CountAsync(),
                ActiveMatches = await _db.Matches.CountAsync(),
                TotalMessages = await _db.Messages.CountAsync()
            };
            
            health.Checks["statistics"] = new HealthCheckItem
            {
                Status = "Healthy",
                Description = $"Users: {stats.TotalUsers}, Matches: {stats.ActiveMatches}, Messages: {stats.TotalMessages}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Health check statistics query failed");
            health.Checks["statistics"] = new HealthCheckItem
            {
                Status = "Degraded",
                Description = $"Could not retrieve statistics: {ex.Message}"
            };
        }

        return health.Status == "Healthy" 
            ? Ok(health) 
            : StatusCode(503, health);
    }
}

public class HealthCheckResult
{
    public string Status { get; set; } = "Healthy";
    public DateTime Timestamp { get; set; }
    public string Version { get; set; } = "1.0.0";
    public Dictionary<string, HealthCheckItem> Checks { get; set; } = new();
}

public class HealthCheckItem
{
    public string Status { get; set; } = "Healthy";
    public string Description { get; set; } = "";
}
