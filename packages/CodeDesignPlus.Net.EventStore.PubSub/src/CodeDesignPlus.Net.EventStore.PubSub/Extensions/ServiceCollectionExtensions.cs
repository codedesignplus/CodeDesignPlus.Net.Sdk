using CodeDesignPlus.Net.EventStore.PubSub.Abstractions.Options;
using CodeDesignPlus.Net.EventStore.PubSub.Exceptions;
using CodeDesignPlus.Net.EventStore.PubSub.Services;
using CodeDesignPlus.Net.PubSub.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.EventStore.PubSub.Extensions;

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
    public static IServiceCollection AddEventStorePubSub(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(EventStorePubSubOptions.Section);

        if (!section.Exists())
            throw new EventStorePubSubException($"The section {EventStorePubSubOptions.Section} is required.");

        services
            .AddOptions<EventStorePubSubOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        var options = section.Get<EventStorePubSubOptions>();

        if (options.Enabled)
        {
            services.AddSingleton<IMessage, EventStorePubSubService>();
            services.AddSingleton<IEventStorePubSubService, EventStorePubSubService>();
        }

        return services;
    }

}
