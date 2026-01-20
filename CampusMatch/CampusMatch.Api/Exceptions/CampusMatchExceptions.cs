namespace CampusMatch.Api.Exceptions;

/// <summary>
/// Base exception for domain-specific errors
/// </summary>
public abstract class CampusMatchException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }

    protected CampusMatchException(string message, int statusCode, string errorCode) 
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    protected CampusMatchException(string message, int statusCode, string errorCode, Exception innerException) 
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Thrown when a requested resource is not found
/// </summary>
public class NotFoundException : CampusMatchException
{
    public NotFoundException(string resource, object id) 
        : base($"{resource} with ID '{id}' was not found.", 404, "RESOURCE_NOT_FOUND")
    {
    }
    
    public NotFoundException(string message) 
        : base(message, 404, "RESOURCE_NOT_FOUND")
    {
    }
}

/// <summary>
/// Thrown when user doesn't have permission
/// </summary>
public class ForbiddenException : CampusMatchException
{
    public ForbiddenException(string message = "You don't have permission to perform this action.") 
        : base(message, 403, "FORBIDDEN")
    {
    }
}

/// <summary>
/// Thrown when user is not authenticated
/// </summary>
public class UnauthorizedException : CampusMatchException
{
    public UnauthorizedException(string message = "Authentication required.") 
        : base(message, 401, "UNAUTHORIZED")
    {
    }
}

/// <summary>
/// Thrown when request validation fails
/// </summary>
public class ValidationException : CampusMatchException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message) 
        : base(message, 400, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors) 
        : base("One or more validation errors occurred.", 400, "VALIDATION_ERROR")
    {
        Errors = errors;
    }
}

/// <summary>
/// Thrown when there's a business rule violation
/// </summary>
public class BusinessRuleException : CampusMatchException
{
    public BusinessRuleException(string message) 
        : base(message, 400, "BUSINESS_RULE_VIOLATION")
    {
    }
}

/// <summary>
/// Thrown when there's a conflict (e.g., duplicate entry)
/// </summary>
public class ConflictException : CampusMatchException
{
    public ConflictException(string message) 
        : base(message, 409, "CONFLICT")
    {
    }
}

/// <summary>
/// Thrown when rate limit is exceeded
/// </summary>
public class RateLimitException : CampusMatchException
{
    public TimeSpan RetryAfter { get; }

    public RateLimitException(TimeSpan retryAfter) 
        : base($"Rate limit exceeded. Try again in {retryAfter.TotalSeconds:0} seconds.", 429, "RATE_LIMIT_EXCEEDED")
    {
        RetryAfter = retryAfter;
    }
}
