namespace CodeDesignPlus.Net.Redis.PubSub.Extensions;

/// <summary>
/// Provides a set of extension methods for CodeDesignPlus.EFCore
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add CodeDesignPlus.EFCore configuration options
    /// </summary>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
    /// <param name="configuration">The configuration being bound.</param>
    /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
    public static IServiceCollection AddRedisPubSub(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(RedisPubSubOptions.Section);

        if (!section.Exists())
            throw new RedisPubSubException($"The section {RedisPubSubOptions.Section} is required.");

        services
            .AddOptions<RedisPubSubOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        var options = section.Get<RedisPubSubOptions>();

        if (options.Enable)
        {
            services.AddRedis(configuration);
            services.AddPubSub(configuration, x =>
            {
                x.UseQueue = options.UseQueue;
                x.EnableDiagnostic = options.EnableDiagnostic;
                x.RegisterAutomaticHandlers = options.RegisterAutomaticHandlers;
                x.SecondsWaitQueue = options.SecondsWaitQueue;
            });
            services.TryAddSingleton<IMessage, RedisPubSubService>();
            services.TryAddSingleton<IRedisPubSubService, RedisPubSubService>();
        }

        return services;
    }

}
