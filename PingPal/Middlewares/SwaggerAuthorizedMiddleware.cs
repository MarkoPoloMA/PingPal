using PingPal.Service.Managers;
using Microsoft.AspNetCore.Authentication;
using PingPal.Domain;

namespace PingPal.Middlewares;

public class SwaggerAuthorizedMiddleware : IMiddleware
{
    private readonly ILogger<SwaggerAuthorizedMiddleware> _logger;

    public SwaggerAuthorizedMiddleware(
        ILogger<SwaggerAuthorizedMiddleware> logger)
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
        if (!context.Request.Path.StartsWithSegments("/swagger"))
            return true;

        if (context.User.Identity?.IsAuthenticated != true)
        {
            _logger.LogWarning("User not authorized");

            await context.ForbidAsync();
            return false;
        }

        var applicationContextUserManager = context.RequestServices
            .GetRequiredService<ApplicationContextUserManager>();

        var userId = applicationContextUserManager.GetUserId(context.User);
        if (userId == null)
        {
            _logger.LogWarning("UserId not found");

            await context.ForbidAsync();
            return false;
        }

        if (!context.User.IsInRole(RoleTokens.SwaggerRole))
        {
            _logger.LogWarning($"Access denied for {userId}. Required role: {RoleTokens.SwaggerRole}");

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

        if (!await applicationContextUserManager.IsInRoleAsync(user, RoleTokens.SwaggerRole))
        {
            _logger.LogWarning($"Access denied for {userId}. Required role: {RoleTokens.SwaggerRole}");

            await context.ForbidAsync();
            return false;
        }

        return true;
    }
}