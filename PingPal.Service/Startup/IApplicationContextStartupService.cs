namespace PingPal.Service.Startup;

public interface IApplicationContextStartupService
{
    Task InitializeUsersAndRolesAsync();
}