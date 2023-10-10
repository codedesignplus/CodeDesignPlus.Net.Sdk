using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Event.Bus.Exceptions;
using CodeDesignPlus.Net.Event.Bus.Options;
using CodeDesignPlus.Net.Event.Bus.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        var eventBus = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .FirstOrDefault(x => typeof(IEventBus).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract && !x.IsInterface);

        if (eventBus == null)
            throw new EventNotImplementedException();

        services.AddSingleton(typeof(IEventBus), eventBus);

        return services;
    }


    /// <summary>
    /// Adds the event handlers that implement the CodeDesignPlus.Event.Bus.Abstractions.IEventHandler interface
    /// </summary>
    /// <typeparam name="TStartupLogic">Implementation of the IStartupServices type</typeparam>
    /// <param name="services">A reference to this instance after the operation has completed.</param>
    public static IServiceCollection AddEventsHandlers<TStartupLogic>(this IServiceCollection services)
        where TStartupLogic : IStartupServices
    {
        var eventsHandlers = GetEventHandlers<TStartupLogic>();

        foreach (var eventHandler in eventsHandlers)
        {
            var interfaceEventHandlerGeneric = eventHandler.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));

            if (interfaceEventHandlerGeneric != null)
            {
                var eventType = interfaceEventHandlerGeneric.GetGenericArguments().FirstOrDefault(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(EventBase)));

                if (eventType != null)
                {
                    var queueServiceType = typeof(IQueueService<,>).MakeGenericType(eventHandler, eventType);
                    var queueServiceImplementationType = typeof(QueueService<,>).MakeGenericType(eventHandler, eventType);

                    var hostServiceType = typeof(IEventBusBackgroundService<,>).MakeGenericType(eventHandler, eventType);
                    var hostServiceImplementationType = typeof(EventBusBackgroundService<,>).MakeGenericType(eventHandler, eventType);

                    services.AddSingleton(queueServiceType, queueServiceImplementationType);
                    services.AddTransient(hostServiceType, hostServiceImplementationType);
                    services.AddTransient(eventHandler);
                }
            }
        }

        return services;
    }

    /// <summary>
    /// Subscribe all implementation of the type IEventHandler
    /// </summary>
    /// <typeparam name="TStartupLogic">Implementation of the IStartupServices type</typeparam>
    /// <param name="provider">Provider of services</param>
    /// <returns>The service provider</returns>
    public static IServiceProvider SubscribeEventsHandlers<TStartupLogic>(this IServiceProvider provider)
        where TStartupLogic : IStartupServices
    {
        var subscriptionManager = provider.GetRequiredService<ISubscriptionManager>();

        var typeSubscriptionManager = subscriptionManager.GetType();

        var methodAddSubscription = typeSubscriptionManager.GetMethods().FirstOrDefault(x => x.Name.Contains("AddSubscription"));

        var eventBus = provider.GetRequiredService<IEventBus>();

        var typeEventBus = eventBus.GetType();

        var eventsHandlers = GetEventHandlers<TStartupLogic>();

        foreach (var eventHandler in eventsHandlers)
        {
            var interfaceEventHandlerGeneric = eventHandler.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));

            if (interfaceEventHandlerGeneric != null)
            {
                var member = interfaceEventHandlerGeneric.GetGenericArguments().FirstOrDefault(x => x.IsSubclassOf(typeof(EventBase)));

                if (!member.IsGenericParameter)
                {
                    var methodAdd = methodAddSubscription.MakeGenericMethod(member, eventHandler);

                    methodAdd.Invoke(subscriptionManager, null);

                    var methodSuscribe = typeEventBus.GetMethods().FirstOrDefault(x => x.Name == nameof(IEventBus.SubscribeAsync) && x.IsGenericMethod);

                    var methodGeneric = methodSuscribe.MakeGenericMethod(member, eventHandler);

                    (methodGeneric.Invoke(eventBus, null) as Task).ConfigureAwait(false);
                }
            }
        }

        return provider;
    }

    /// <summary>
    /// Determines whether an instance of a specified type can be assigned to a variable of the current type.
    /// </summary>
    /// <param name="type">Current type.</param>
    /// <param name="interface">The type to compare with the current type.</param>
    /// <returns>Return true if type implemented <paramref name="interface"/></returns>
    public static bool IsAssignableGenericFrom(this Type type, Type @interface)
    {
        return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == @interface);
    }

    /// <summary>
    /// Escanea y retorna las clase que implementan la interfaz <see cref="IEventHandler{TEvent}"/>
    /// </summary>
    /// <typeparam name="TStartupLogic">Clase de inicio que implementa la interfaz <see cref="IStartupServices"/></typeparam>
    /// <returns>Return a list of type</returns>
    public static List<Type> GetEventHandlers<TStartupLogic>() where TStartupLogic : IStartupServices
    {
        return Assembly.GetAssembly(typeof(TStartupLogic))
            .GetTypes()
            .Where(x =>
                x.IsClass &&
                x.IsAssignableGenericFrom(typeof(IEventHandler<>))
            ).ToList();
    }

}
