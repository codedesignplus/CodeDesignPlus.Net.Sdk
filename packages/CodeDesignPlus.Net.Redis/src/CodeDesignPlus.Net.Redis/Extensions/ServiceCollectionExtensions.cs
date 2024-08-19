namespace CodeDesignPlus.Net.Redis.Extensions;

/// <summary>
/// Extension methods for setting up Redis services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Redis services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The configuration to bind the Redis options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="configuration"/> is null.</exception>
    /// <exception cref="Exceptions.RedisException">Thrown if the Redis configuration section does not exist.</exception>
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var redisSection = configuration.GetSection(RedisOptions.Section);

        if (!redisSection.Exists())
            throw new Exceptions.RedisException($"The section {RedisOptions.Section} is required.");

        services
            .AddOptions<RedisOptions>()
            .Bind(redisSection)
            .ValidateDataAnnotations();

        services.AddCore(configuration);
        services.TryAddSingleton<IRedisService, RedisService>();
        services.TryAddSingleton<IRedisServiceFactory, RedisServiceFactory>();

        services.TryAddSingleton((serviceProvider) =>
        {
            var connection = serviceProvider.GetService<IRedisServiceFactory>().Create(FactoryConst.RedisCore).Connection;

            return connection;
        });

        return services;
    }
}