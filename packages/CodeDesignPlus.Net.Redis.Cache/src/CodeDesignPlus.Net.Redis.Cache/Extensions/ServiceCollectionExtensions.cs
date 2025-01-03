using CodeDesignPlus.Net.Redis.Cache.Abstractions;
using CodeDesignPlus.Net.Redis.Cache.Exceptions;
using CodeDesignPlus.Net.Redis.Cache.Abstractions.Options;
using CodeDesignPlus.Net.Redis.Cache.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.Cache.Abstractions;

namespace CodeDesignPlus.Net.Redis.Cache.Extensions;

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
    public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(RedisCacheOptions.Section);

        if (!section.Exists())
            throw new RedisCacheException($"The section {RedisCacheOptions.Section} is required.");

        var options = section.Get<RedisCacheOptions>();

        services
            .AddOptions<RedisCacheOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        if (options.Enable)
        {
            services.AddSingleton<ICacheManager, RedisCacheManager>();
            services.AddSingleton<IRedisCacheManager, RedisCacheManager>();
        }

        return services;
    }

}
