using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.Exceptions;
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

        var section = configuration.GetSection(RedisOptions.Section);

        if (!section.Exists())
            throw new Exceptions.RedisException($"The section {RedisOptions.Section} is required.");

        services
            .AddOptions<RedisOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.AddSingleton<IRedisService, RedisService>();

        return services;
    }

}
