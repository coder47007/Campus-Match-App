using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CampusMatch.Api.Attributes;

/// <summary>
/// Attribute that restricts access to admin users only.
/// Checks if the current user has the IsAdmin claim set to true.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AdminOnlyAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        
        // Check if user is authenticated
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Authentication required" });
            return;
        }
        
        // Check if user has admin claim
        var isAdminClaim = user.FindFirst("IsAdmin")?.Value;
        if (isAdminClaim == null || !bool.TryParse(isAdminClaim, out var isAdmin) || !isAdmin)
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}
