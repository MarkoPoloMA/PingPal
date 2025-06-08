using PingPal.Common.Extensions;
using PingPal.Domain;
using PingPal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PingPal.Service.Managers;

namespace PingPal.Service.Startup;

public class ApplicationContextStartupService : IApplicationContextStartupService
{
    private readonly ApplicationContextUserManager _applicationContextUserManager;
    private readonly ApplicationContextRoleManager _applicationContextRoleManager;
    private readonly ApplicationContextStartupOptions _applicationContextStartupOptions;
    private readonly ILogger<ApplicationContextStartupService> _logger;

    public ApplicationContextStartupService(
        ApplicationContextUserManager applicationContextUserManager,
        ApplicationContextRoleManager applicationContextRoleManager,
        ApplicationContextStartupOptions applicationContextStartupOptions,
        ILogger<ApplicationContextStartupService> logger)
    {
        _applicationContextUserManager = applicationContextUserManager;
        _applicationContextRoleManager = applicationContextRoleManager;
        _applicationContextStartupOptions = applicationContextStartupOptions;
        _logger = logger;
    }

    public async Task InitializeUsersAndRolesAsync()
    {
        try
        {
            await InitializeRolesAsync();
            await InitializeUsersAsync();
        }
        catch (Exception exception)
        {
            const string message = "Error on initialize users and roles";

            _logger.LogCritical(exception, message);
            throw new Exception(message, exception);
        }
    }

    private async Task InitializeRolesAsync()
    {
        var adminRole = await _applicationContextRoleManager.FindByNameAsync(RoleTokens.AdminRole);
        var swaggerRole = await _applicationContextRoleManager.FindByNameAsync(RoleTokens.SwaggerRole);

        if (adminRole == null)
            await _applicationContextRoleManager.CreateAsync(new Role { Id = Guid.NewGuid(), Name = RoleTokens.AdminRole });
        if (swaggerRole == null)
            await _applicationContextRoleManager.CreateAsync(new Role { Id = Guid.NewGuid(), Name = RoleTokens.SwaggerRole });
    }

    private async Task InitializeUsersAsync()
    {
        var login = _applicationContextStartupOptions.InitializeUserLogin;
        var password = _applicationContextStartupOptions.InitializeUserPassword;
        if (login.IsNullOrEmpty() || password.IsNullOrEmpty())
            return;

        var hasUsers = await _applicationContextUserManager.Users.AnyAsync();
        if (hasUsers)
            return;

        var user = new User { Id = Guid.NewGuid(), Name = login };
        await _applicationContextUserManager.CreateAsync(user, password);
    }
}