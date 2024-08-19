namespace CodeDesignPlus.Net.Redis.PubSub.Extensions;

/// <summary>
/// Extension methods for setting up Redis Pub/Sub services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Redis Pub/Sub services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The configuration to bind the Redis Pub/Sub options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="configuration"/> is null.</exception>
    /// <exception cref="RedisPubSubException">Thrown if the Redis Pub/Sub configuration section does not exist.</exception>
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