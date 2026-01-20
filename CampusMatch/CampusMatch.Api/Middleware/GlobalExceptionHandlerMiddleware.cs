using System.Net;
using System.Text.Json;
using CampusMatch.Api.Exceptions;

namespace CampusMatch.Api.Middleware;

/// <summary>
/// Global exception handler middleware that catches all unhandled exceptions
/// and returns a consistent JSON error response.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing {Method} {Path}", 
                context.Request.Method, context.Request.Path);
            
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new ErrorResponse
        {
            TraceId = context.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };
        
        switch (exception)
        {
            // Custom domain exceptions - Phase 1.4
            case CampusMatchException campusEx:
                context.Response.StatusCode = campusEx.StatusCode;
                response.Error = campusEx.ErrorCode;
                response.Message = campusEx.Message;
                
                if (campusEx is ValidationException validationEx)
                {
                    response.ValidationErrors = validationEx.Errors;
                }
                
                if (campusEx is RateLimitException rateLimitEx)
                {
                    context.Response.Headers.Append("Retry-After", rateLimitEx.RetryAfter.TotalSeconds.ToString("0"));
                }
                
                _logger.LogWarning("Domain exception: {ErrorCode} - {Message}", 
                    campusEx.ErrorCode, campusEx.Message);
                break;
                
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Error = "UNAUTHORIZED";
                response.Message = "You are not authorized to perform this action.";
                break;
                
            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Error = "NOT_FOUND";
                response.Message = exception.Message;
                break;
                
            case ArgumentException:
            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Error = "BAD_REQUEST";
                response.Message = exception.Message;
                break;
                
            case OperationCanceledException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Error = "REQUEST_CANCELLED";
                response.Message = "The request was cancelled.";
                _logger.LogInformation("Request cancelled by client");
                break;
                
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Error = "INTERNAL_ERROR";
                response.Message = _env.IsDevelopment() 
                    ? exception.Message 
                    : "An unexpected error occurred. Please try again later.";
                break;
        }
        
        // Include stack trace in development
        if (_env.IsDevelopment())
        {
            response.Details = exception.StackTrace;
        }
        
        var jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}

/// <summary>
/// Standard error response format for API errors.
/// </summary>
public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? TraceId { get; set; }
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public IDictionary<string, string[]>? ValidationErrors { get; set; }
}

/// <summary>
/// Extension method to register the global exception handler middleware.
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
