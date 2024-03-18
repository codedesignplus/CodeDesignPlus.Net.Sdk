using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Redis.Options;
using CodeDesignPlus.Net.Redis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Redis.Extensions;

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
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var redisSection = configuration.GetSection(RedisOptions.Section);
        var coreSection = configuration.GetSection(CoreOptions.Section);

        if (!redisSection.Exists())
            throw new Exceptions.RedisException($"The section {RedisOptions.Section} is required.");

        if (!coreSection.Exists())
            throw new Exceptions.RedisException($"The section {CoreOptions.Section} is required.");

        var options = coreSection.Get<CoreOptions>();

        services
            .AddOptions<RedisOptions>()
            .Bind(redisSection)
            .ValidateDataAnnotations();

        services.AddSingleton<IRedisService, RedisService>();
        services.AddSingleton<IRedisServiceFactory, RedisServiceFactory>();

        services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
        {
            var connection = serviceProvider.GetService<IRedisServiceFactory>().Create(FactoryConst.RedisCore).Connection;

            return connection;
        });

        return services;
    }

}
