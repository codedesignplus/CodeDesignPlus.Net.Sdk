using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.Redis.PubSub.Exceptions;
using CodeDesignPlus.Net.Redis.PubSub.Options;
using CodeDesignPlus.Net.Redis.PubSub.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

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
            services.AddSingleton<IMessage, RedisPubSubService>();
            services.AddSingleton<IRedisPubSubService, RedisPubSubService>();
        }

        return services;
    }

}
