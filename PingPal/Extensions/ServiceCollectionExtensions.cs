namespace PingPal.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingletonOptions<TOptions>(
        this IServiceCollection service,
        ConfigurationManager configuration)
        where TOptions : class, new()
    {
        var options = new TOptions();
        configuration.GetSection(typeof(TOptions).Name).Bind(options);
        service.AddSingleton(options);
        return service;
    }
}