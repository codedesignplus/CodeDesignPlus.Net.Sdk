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
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(PubSubOptions.Section);

        if (!section.Exists())
            throw new PubSubException($"The section {PubSubOptions.Section} is required.");

        services
            .AddOptions<PubSubOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        var options = section.Get<PubSubOptions>();

        services
            .AddCore(configuration)
            .AddEventsHandlers()
            .TryAddSingleton<IPubSub, PubSubService>();

        if (options.UseQueue)
        {
            services.TryAddSingleton<IEventQueueService, EventQueueService>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), typeof(EventQueueBackgroundService)));
        }

        if (options.EnableDiagnostic)
            services.TryAddSingleton<IActivityService, ActivitySourceService>();

        return services;
    }

    public static IServiceCollection AddPubSub(this IServiceCollection services, IConfiguration configuration, Action<PubSubOptions> setupOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(setupOptions);

        var pubSubOptions = new PubSubOptions();

        setupOptions(pubSubOptions);

        services
            .AddOptions<PubSubOptions>()
            .Configure(setupOptions)
            .ValidateDataAnnotations();

        services
           .AddCore(configuration)
           .AddEventsHandlers()
           .TryAddSingleton<IPubSub, PubSubService>();

        services.TryAddSingleton<IPubSub, PubSubService>();

        services.AddEventsHandlers();

        if (pubSubOptions.UseQueue)
        {
            services.TryAddSingleton<IEventQueueService, EventQueueService>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), typeof(EventQueueBackgroundService)));
        }

        if (pubSubOptions.EnableDiagnostic)
            services.TryAddSingleton<IActivityService, ActivitySourceService>();

        return services;
    }



    /// <summary>
    /// Adds the event handlers that implement the CodeDesignPlus.PubSub.Abstractions.IEventHandler interface
    /// </summary>
    /// <param name="services">A reference to this instance after the operation has completed.</param>
    /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
    private static IServiceCollection AddEventsHandlers(this IServiceCollection services)
    {
        var eventsHandlers = PubSubExtensions.GetEventHandlers();

        foreach (var eventHandler in eventsHandlers)
        {
            var interfaceEventHandlerGeneric = eventHandler.GetInterfaceEventHandlerGeneric();

            var eventType = interfaceEventHandlerGeneric.GetEventType();

            if (eventType == null)
                continue;

            var eventHandlerBackgroundType = typeof(RegisterEventHandlerBackgroundService<,>).MakeGenericType(eventHandler, eventType);
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), eventHandlerBackgroundType));

            services.TryAddSingleton(eventHandler);
        }

        return services;
    }

}
