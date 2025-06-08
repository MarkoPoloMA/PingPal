namespace PingPal.Service.Migrations
{
	public interface IApplicationContextMigrationsService
	{
		Task ApplyMigrationsAsync();
	}
}