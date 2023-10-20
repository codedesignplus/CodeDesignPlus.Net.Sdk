using CodeDesignPlus.Net.Redis.Event.Bus.Abstractions;
using CodeDesignPlus.Net.Redis.Event.Bus.Exceptions;
using CodeDesignPlus.Net.Redis.Event.Bus.Options;
using CodeDesignPlus.Net.Redis.Event.Bus.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Redis.Event.Bus.Extensions;

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
    public static IServiceCollection AddRedisEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(RedisEventBusOptions.Section);

        if (!section.Exists())
            throw new RedisEventBusException($"The section {RedisEventBusOptions.Section} is required.");

        services
            .AddOptions<RedisEventBusOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.AddSingleton<IRedisEventBusService, RedisEventBusService>();

        return services;
    }

}
