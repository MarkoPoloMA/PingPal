using PingPal.Common.Extensions;
using PingPal.Service.Managers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace PingPal.Middlewares;

public class DatabaseCheckUserRolesMiddleware : IMiddleware
{
    private readonly ILogger<DatabaseCheckUserRolesMiddleware> _logger;

    public DatabaseCheckUserRolesMiddleware(
        ILogger<DatabaseCheckUserRolesMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (await CheckUserRolesAsync(context))
            await next.Invoke(context);
    }

    private async Task<bool> CheckUserRolesAsync(HttpContext context)
    {
        var authorizeRoles = context.GetEndpoint()?.Metadata.GetMetadata<AuthorizeAttribute>()?.Roles;
        var requiredRoles = authorizeRoles?.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (requiredRoles.IsNullOrEmpty() || context.User.Identity?.IsAuthenticated != true)
            return true;

        var applicationContextUserManager = context.RequestServices
            .GetRequiredService<ApplicationContextUserManager>();

        var userId = applicationContextUserManager.GetUserId(context.User);
        if (userId == null)
        {
            _logger.LogWarning("UserId not found");

            await context.ForbidAsync();
            return false;
        }

        var user = await applicationContextUserManager.FindByIdAndLoadRolesAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"User {userId} not found");

            await context.ForbidAsync();
            return false;
        }

        var userRoles = await applicationContextUserManager.GetRolesAsync(user);
        if (!requiredRoles.Any(role => userRoles.Contains(role.Trim(), StringComparer.OrdinalIgnoreCase)))
        {
            _logger.LogWarning($"Access denied for {userId}. Required roles: {authorizeRoles}");

            await context.ForbidAsync();
            return false;
        }

        return true;
    }
}