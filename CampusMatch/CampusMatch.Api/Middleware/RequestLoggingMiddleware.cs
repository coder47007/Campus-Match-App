using System.Diagnostics;

namespace CampusMatch.Api.Middleware;

/// <summary>
/// Middleware for logging HTTP request/response details including timing.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString("N")[..8];
        
        // Log request
        _logger.LogInformation(
            "[{RequestId}] {Method} {Path}{QueryString} - Started",
            requestId,
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString);

        try
        {
            await _next(context);
            stopwatch.Stop();
            
            // Log successful response
            _logger.LogInformation(
                "[{RequestId}] {Method} {Path} - {StatusCode} in {ElapsedMs}ms",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // Log error (the GlobalExceptionHandler will handle the response)
            _logger.LogError(
                ex,
                "[{RequestId}] {Method} {Path} - FAILED in {ElapsedMs}ms",
                requestId,
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);
            
            throw;
        }
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
