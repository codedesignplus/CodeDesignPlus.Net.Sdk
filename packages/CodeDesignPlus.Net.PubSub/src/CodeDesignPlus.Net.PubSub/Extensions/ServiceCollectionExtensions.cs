namespace CodeDesignPlus.Net.PubSub.Extensions;

/// <summary>
/// Provides extension methods for adding PubSub services to the IServiceCollection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds PubSub services to the specified IServiceCollection using the provided configuration.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configuration">The configuration to use for PubSub options.</param>
    /// <returns>The IServiceCollection with PubSub services added.</returns>
    /// <exception cref="ArgumentNullException">Thrown if services or configuration is null.</exception>
    /// <exception cref="PubSubException">Thrown if the PubSubOptions section is not found in the configuration.</exception>
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
            .AddEventsHandlers(options.RegisterAutomaticHandlers)
            .TryAddSingleton<IPubSub, PubSubService>();

        if (options.UseQueue)
        {
            services.TryAddSingleton<IEventQueue, EventQueueService>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, EventQueueBackgroundService>());
        }

        if (options.EnableDiagnostic)
            services.TryAddSingleton<IActivityService, ActivitySourceService>();

        return services;
    }

    /// <summary>
    /// Adds PubSub services to the specified IServiceCollection using the provided configuration and setup options.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configuration">The configuration to use for PubSub options.</param>
    /// <param name="setupOptions">An action to configure the PubSub options.</param>
    /// <returns>The IServiceCollection with PubSub services added.</returns>
    /// <exception cref="ArgumentNullException">Thrown if services, configuration, or setupOptions is null.</exception>
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
           .AddEventsHandlers(pubSubOptions.RegisterAutomaticHandlers)
           .TryAddSingleton<IPubSub, PubSubService>();

        if (pubSubOptions.UseQueue)
        {
            services.TryAddSingleton<IEventQueue, EventQueueService>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, EventQueueBackgroundService>());
        }

        if (pubSubOptions.EnableDiagnostic)
            services.TryAddSingleton<IActivityService, ActivitySourceService>();

        return services;
    }

    /// <summary>
    /// Adds event handlers to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the event handlers to.</param>
    /// <param name="registerAutomaticHandlers">Indicates whether to register automatic event handlers.</param>
    /// <returns>The IServiceCollection with event handlers added.</returns>
    private static IServiceCollection AddEventsHandlers(this IServiceCollection services, bool registerAutomaticHandlers)
    {
        if (!registerAutomaticHandlers)
            return services;

        var eventsHandlers = PubSubExtensions.GetEventHandlers();

        foreach (var eventHandler in eventsHandlers)
        {
            var interfaceEventHandlerGeneric = eventHandler.GetInterfaceEventHandlerGeneric();

            var eventType = interfaceEventHandlerGeneric.GetEventType();

            if (eventType == null)
                continue;

            var eventHandlerBackgroundType = typeof(RegisterEventHandlerBackgroundService<,>).MakeGenericType(eventHandler, eventType);
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), eventHandlerBackgroundType));

            services.TryAddScoped(eventHandler);
        }

        return services;
    }
}