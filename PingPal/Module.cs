using PingPal.Extensions;
using PingPal.Service.Managers;
using PingPal.Service.Migrations;
using PingPal.Service.Startup;
using PingPal.Service.Stores;
using Microsoft.AspNetCore.Identity;
using PingPal.Database.Context.Factory;
using PingPal.Domain.Entities;

namespace PingPal;

public static class Module
{
    public static void RegisterDependencies(IServiceCollection service, ConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        service.AddSingletonOptions<ApplicationContextStartupOptions>(configuration);

        service.AddSingleton<IApplicationContextFactory, ApplicationContextFactory>();

        service.AddScoped<ApplicationContextUserManager>();
        service.AddScoped<ApplicationContextRoleManager>();
        service.AddScoped<ApplicationContextSignInManager>();

        service.AddScoped<UserManager<User>, ApplicationContextUserManager>();
        service.AddScoped<RoleManager<Role>, ApplicationContextRoleManager>();
        service.AddScoped<SignInManager<User>, ApplicationContextSignInManager>();

        service.AddScoped<IApplicationContextUserStore, ApplicationContextUserStore>();
        service.AddScoped<IApplicationContextRoleStore, ApplicationContextRoleStore>();
        service.AddScoped<IApplicationContextStartupService, ApplicationContextStartupService>();
        service.AddScoped<IApplicationContextMigrationsService, ApplicationContextMigrationsService>();

        service.AddIdentity<User, Role>()
            .AddUserStore<ApplicationContextUserStore>()
            .AddRoleStore<ApplicationContextRoleStore>();
    }
}