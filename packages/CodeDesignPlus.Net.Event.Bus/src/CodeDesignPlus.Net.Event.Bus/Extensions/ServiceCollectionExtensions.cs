using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Event.Bus.Exceptions;
using CodeDesignPlus.Net.Event.Bus.Options;
using CodeDesignPlus.Net.Event.Bus.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.Event.Bus.Extensions;

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
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(EventBusOptions.Section);

        if (!section.Exists())
            throw new EventBusException($"The section {EventBusOptions.Section} is required.");

        services
            .AddOptions<EventBusOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.AddSingleton<ISubscriptionManager, SubscriptionManager>();
        //services.AddHostedService<SubscribeBackgroundService>();

        var eventBus = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .FirstOrDefault(x => typeof(IEventBus).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract && !x.IsInterface);

        if (eventBus == null)
            throw new EventNotImplementedException();

        services.AddSingleton(typeof(IEventBus), eventBus);

        services.AddEventsHandlers(section.Get<EventBusOptions>());

        return services;
    }

    /// <summary>
    /// Adds the event handlers that implement the CodeDesignPlus.Event.Bus.Abstractions.IEventHandler interface
    /// </summary>
    /// <param name="services">A reference to this instance after the operation has completed.</param>
    /// <param name="eventBusOptions">The event bus options</param>
    /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
    private static IServiceCollection AddEventsHandlers(this IServiceCollection services, EventBusOptions eventBusOptions)
    {
        var eventsHandlers = EventBusExtensions.GetEventHandlers();

        foreach (var eventHandler in eventsHandlers)
        {
            var interfaceEventHandlerGeneric = eventHandler.GetInterfaceEventHandlerGeneric();

            var eventType = interfaceEventHandlerGeneric.GetEventType();

            if (eventType == null)
                continue;

            if (eventBusOptions.EnableQueue)
            {
                var queueServiceType = typeof(IQueueService<,>).MakeGenericType(eventHandler, eventType);
                var queueServiceImplementationType = typeof(QueueService<,>).MakeGenericType(eventHandler, eventType);
                services.AddSingleton(queueServiceType, queueServiceImplementationType);

                var hostServiceImplementationType = typeof(QueueBackgroundService<,>).MakeGenericType(eventHandler, eventType);
                var hostServiceType = typeof(IEventBusBackgroundService<,>).MakeGenericType(eventHandler, eventType);
                services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), hostServiceImplementationType));
            }

            var eventHandlerBackgroundType = typeof(EventHandlerBackgroundService<,>).MakeGenericType(eventHandler, eventType);
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), eventHandlerBackgroundType));

            services.AddSingleton(eventHandler);
        }

        return services;
    }

}
