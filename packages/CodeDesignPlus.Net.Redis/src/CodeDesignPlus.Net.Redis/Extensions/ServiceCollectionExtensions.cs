using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Redis.Options;
using CodeDesignPlus.Net.Redis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.Core.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

        services.AddSingleton((serviceProvider) =>
        {
            var connection = serviceProvider.GetService<IRedisServiceFactory>().Create(FactoryConst.RedisCore).Connection;

            return connection;
        });

        return services;
    }

}
