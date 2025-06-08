using PingPal.Database.Context.Factory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace PingPal.Service.Migrations;

public class ApplicationContextMigrationsService : IApplicationContextMigrationsService
{
    private readonly IApplicationContextFactory _applicationContextFactory;
    private readonly ILogger<ApplicationContextMigrationsService> _logger;

    public ApplicationContextMigrationsService(
        IApplicationContextFactory applicationContextFactory,
        ILogger<ApplicationContextMigrationsService> logger)
    {
        _applicationContextFactory = applicationContextFactory;
        _logger = logger;
    }

    public async Task ApplyMigrationsAsync()
    {
        try
        {
            await ApplyMigrationsInternalAsync();
        }
        catch (Exception exception)
        {
            const string message = "Error on apply migrations";

            _logger.LogCritical(exception, message);
            throw new Exception(message, exception);
        }
    }

    private async Task ApplyMigrationsInternalAsync()
    {
        await using var context = _applicationContextFactory.Create();

        var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
        _logger.LogInformation($"Applied migrations: {string.Join(", ", appliedMigrations)}");

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        _logger.LogInformation($"Pending migrations: {string.Join(", ", pendingMigrations)}");

        await context.Database.MigrateAsync();
    }
}