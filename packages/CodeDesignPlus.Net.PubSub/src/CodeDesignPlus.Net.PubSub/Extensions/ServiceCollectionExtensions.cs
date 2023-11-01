using System.Reflection;
using CodeDesignPlus.Net.PubSub.Exceptions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using CodeDesignPlus.Net.PubSub.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.PubSub.Extensions;

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
    public static IServiceCollection AddPubSub(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(PubSubOptions.Section);

        if (!section.Exists())
            throw new PubSubException($"The section {PubSubOptions.Section} is required.");

        services
            .AddOptions<PubSubOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.AddSingleton<ISubscriptionManager, SubscriptionManager>();

        var pubSub = PubSubExtensions.GetPubSub() ?? throw new EventNotImplementedException();
        
        services.AddSingleton(typeof(IPubSub), pubSub);

        services.AddEventsHandlers(section.Get<PubSubOptions>());

        return services;
    }

   

    /// <summary>
    /// Adds the event handlers that implement the CodeDesignPlus.PubSub.Abstractions.IEventHandler interface
    /// </summary>
    /// <param name="services">A reference to this instance after the operation has completed.</param>
    /// <param name="PubSubOptions">The event bus options</param>
    /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
    private static IServiceCollection AddEventsHandlers(this IServiceCollection services, PubSubOptions PubSubOptions)
    {
        var eventsHandlers = PubSubExtensions.GetEventHandlers();

        foreach (var eventHandler in eventsHandlers)
        {
            var interfaceEventHandlerGeneric = eventHandler.GetInterfaceEventHandlerGeneric();

            var eventType = interfaceEventHandlerGeneric.GetEventType();

            if (eventType == null)
                continue;

            if (PubSubOptions.EnableQueue)
            {
                var queueServiceType = typeof(IQueueService<,>).MakeGenericType(eventHandler, eventType);
                var queueServiceImplementationType = typeof(QueueService<,>).MakeGenericType(eventHandler, eventType);
                services.AddSingleton(queueServiceType, queueServiceImplementationType);

                var hostServiceImplementationType = typeof(QueueBackgroundService<,>).MakeGenericType(eventHandler, eventType);
                services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), hostServiceImplementationType));
            }

            var eventHandlerBackgroundType = typeof(EventHandlerBackgroundService<,>).MakeGenericType(eventHandler, eventType);
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), eventHandlerBackgroundType));

            services.AddSingleton(eventHandler);
        }

        return services;
    }

}
