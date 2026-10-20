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
            .AddOutbox(options)
            .TryAddSingleton<IPubSub, PubSubService>();

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
           .AddOutbox(pubSubOptions)
           .TryAddSingleton<IPubSub, PubSubService>();

        if (pubSubOptions.EnableDiagnostic)
            services.TryAddSingleton<IActivityService, ActivitySourceService>();

        return services;
    }

    /// <summary>
    /// Registra el aviso de la bandeja de salida y, si está activa, su despachador (pendings/245).
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="options">The PubSub options.</param>
    /// <returns>The IServiceCollection with the outbox services added.</returns>
    /// <exception cref="ValidationException">Thrown when the outbox options are out of range.</exception>
    private static IServiceCollection AddOutbox(this IServiceCollection services, PubSubOptions options)
    {
        // ValidateDataAnnotations no entra en los objetos anidados: se validan aquí, al arrancar.
        Validator.ValidateObject(options.Outbox, new ValidationContext(options.Outbox), validateAllProperties: true);

        services.TryAddSingleton<IOutboxSignal, OutboxSignal>();

        if (options.Outbox.Enable)
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, OutboxDispatcherBackgroundService>());

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
        services.TryAddSingleton<ISubscriptionTracker, SubscriptionTracker>();

        if (!registerAutomaticHandlers)
            return services;

        var eventsHandlers = PubSubExtensions.GetEventHandlers();
        var handlerCount = 0;
        var snapshots = new List<(Type Snapshot, Type Handler)>();

        foreach (var eventHandler in eventsHandlers)
        {
            var interfaceEventHandlerGeneric = eventHandler.GetInterfaceEventHandlerGeneric();

            var eventType = interfaceEventHandlerGeneric.GetEventType();

            if (eventType == null)
                continue;

            var eventHandlerBackgroundType = typeof(RegisterEventHandlerBackgroundService<,>).MakeGenericType(eventHandler, eventType);
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), eventHandlerBackgroundType));

            handlerCount++;

            if (typeof(SnapshotDomainEvent).IsAssignableFrom(eventType))
            {
                services.AddSnapshotHandler(eventHandler);
                snapshots.Add((eventType, eventHandler));
            }
            else
            {
                services.TryAddScoped(eventHandler);
            }
        }

        handlerCount += services.AddSnapshots(snapshots);

        services.Configure<SubscriptionTrackerInitializer>(x => x.ExpectedCount = handlerCount);
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, SubscriptionTrackerInitializerService>());

        return services;
    }

    /// <summary>
    /// Registra la sincronización por fotos: el lado del dueño si la aplicación tiene alguna <see cref="ISnapshotSource"/>,
    /// y el de la copia si consume algún evento de foto.
    /// </summary>
    /// <param name="services">La colección de servicios.</param>
    /// <param name="snapshots">Los eventos de foto que consume la aplicación, con su consumidor.</param>
    /// <returns>Cuántos consumidores propios del paquete se registraron, para el contador de suscripciones.</returns>
    private static int AddSnapshots(this IServiceCollection services, List<(Type Snapshot, Type Handler)> snapshots)
    {
        var count = 0;
        var sources = PubSubExtensions.GetSnapshotSources();

        foreach (var source in sources)
            services.TryAddEnumerable(ServiceDescriptor.Scoped(typeof(ISnapshotSource), source));

        if (sources.Count > 0)
        {
            services.AddOwnHandler<SnapshotRequestedHandler, SnapshotRequestedDomainEvent>();
            count++;
        }

        if (snapshots.Count > 0)
        {
            services.AddOwnHandler<SnapshotCompletedHandler, SnapshotCompletedDomainEvent>();
            count++;

            services.TryAddSingleton<ISnapshotProgressTracker, SnapshotProgressTracker>();

            services.TryAddSingleton<ISnapshotRequester, SnapshotRequester>();
            services.Configure<SnapshotSyncOptions>(x => x.Snapshots.AddRange(snapshots));
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, SnapshotSyncBackgroundService>());
        }

        return count;
    }

    /// <summary>
    /// Registra un consumidor de fotos y le entrega, al resolverlo, el contador de fotos aplicadas (pendings/485).
    /// </summary>
    /// <remarks>
    /// El contador no va en el constructor de <see cref="SnapshotHandler{TSnapshot}"/>: los consumidores de los micros
    /// ya están declarados con el suyo, y así cuentan sin cambiar una línea al subir de versión.
    /// </remarks>
    /// <param name="services">La colección de servicios.</param>
    /// <param name="handlerType">El consumidor de fotos.</param>
    private static void AddSnapshotHandler(this IServiceCollection services, Type handlerType)
    {
        services.TryAddScoped(handlerType, provider =>
        {
            var handler = ActivatorUtilities.CreateInstance(provider, handlerType);

            if (handler is ISnapshotProgressReporter reporter)
                reporter.ProgressTracker = provider.GetService<ISnapshotProgressTracker>();

            return handler;
        });
    }

    private static void AddOwnHandler<THandler, TEvent>(this IServiceCollection services) where THandler : class, IEventHandler<TEvent> where TEvent : IDomainEvent
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, RegisterEventHandlerBackgroundService<THandler, TEvent>>());
        services.TryAddScoped<THandler>();
    }
}