using CodeDesignPlus.Net.Core.Extensions;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.Redis.PubSub.Exceptions;
using CodeDesignPlus.Net.Redis.PubSub.Options;
using CodeDesignPlus.Net.Redis.PubSub.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CodeDesignPlus.Net.Redis.Extensions;
using CodeDesignPlus.Net.PubSub.Extensions;

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
            services.AddCore(configuration);
            services.AddRedis(configuration);
            services.AddPubSub(x => {
                x.UseQueue = options.PubSub.UseQueue;
                x.EnableDiagnostic = options.PubSub.EnableDiagnostic;
                x.RegisterAutomaticHandlers = options.PubSub.RegisterAutomaticHandlers;
                x.SecondsWaitQueue = options.PubSub.SecondsWaitQueue;
            });
            services.TryAddSingleton<IMessage, RedisPubSubService>();
            services.TryAddSingleton<IRedisPubSubService, RedisPubSubService>();
        }

        return services;
    }

}
