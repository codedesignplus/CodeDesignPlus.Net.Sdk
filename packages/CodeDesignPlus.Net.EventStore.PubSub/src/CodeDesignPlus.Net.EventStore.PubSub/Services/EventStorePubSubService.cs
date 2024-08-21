namespace CodeDesignPlus.Net.EventStore.PubSub.Services;

/// <summary>
/// Provides Pub/Sub services for interacting with EventStore.
/// </summary>
public class EventStorePubSubService : IEventStorePubSubService
{
    private readonly IEventStoreFactory eventStoreFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<EventStorePubSubService> logger;
    private readonly EventStorePubSubOptions options;
    private readonly PersistentSubscriptionSettings settings;
    private readonly IDomainEventResolverService domainEventResolverService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStorePubSubService"/> class.
    /// </summary>
    /// <param name="eventStoreFactory">The factory to create EventStore connections.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="eventStorePubSubOptions">The EventStore Pub/Sub options.</param>
    /// <param name="domainEventResolverService">The service to resolve domain events.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="eventStoreFactory"/>, <paramref name="serviceProvider"/>, <paramref name="logger"/>, <paramref name="eventStorePubSubOptions"/>, or <paramref name="domainEventResolverService"/> is null.
    /// </exception>
    public EventStorePubSubService(
        IEventStoreFactory eventStoreFactory,
        IServiceProvider serviceProvider,
        ILogger<EventStorePubSubService> logger,
        IOptions<EventStorePubSubOptions> eventStorePubSubOptions,
        IDomainEventResolverService domainEventResolverService)
    {
        ArgumentNullException.ThrowIfNull(eventStoreFactory);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(eventStorePubSubOptions);
        ArgumentNullException.ThrowIfNull(domainEventResolverService);

        this.eventStoreFactory = eventStoreFactory;
        this.serviceProvider = serviceProvider;
        this.logger = logger;
        this.domainEventResolverService = domainEventResolverService;
        this.options = eventStorePubSubOptions.Value;

        this.settings = PersistentSubscriptionSettings
            .Create()
            .StartFromCurrent();

        this.logger.LogInformation("EventStorePubSubService initialized.");
    }

    /// <summary>
    /// Publishes a domain event to the EventStore.
    /// </summary>
    /// <param name="event">The domain event to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken)
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core, cancellationToken).ConfigureAwait(false);

        var stream = this.domainEventResolverService.GetKeyDomainEvent(@event.GetType());

        @event.Metadata.Add("OccurredAt", @event.OccurredAt);
        @event.Metadata.Add("EventId", @event.EventId);
        @event.Metadata.Add("EventType", stream);

        var eventData = new EventData(
            @event.EventId,
            stream,
            true,
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event)),
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event.Metadata)));

        await connection.AppendToStreamAsync(stream, ExpectedVersion.Any, eventData).ConfigureAwait(false);
    }

    /// <summary>
    /// Publishes a list of domain events to the EventStore.
    /// </summary>
    /// <param name="events">The list of domain events to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task PublishAsync(IReadOnlyList<IDomainEvent> events, CancellationToken cancellationToken)
    {
        var tasks = events.Select(@event => this.PublishAsync(@event, cancellationToken));

        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Subscribes to a domain event in the EventStore.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core, cancellationToken).ConfigureAwait(false);

        var (user, pass) = this.eventStoreFactory.GetCredentials(EventStoreFactoryConst.Core);

        var stream = this.domainEventResolverService.GetKeyDomainEvent<TEvent>();

        var userCredentials = new UserCredentials(user, pass);

        try
        {
            await connection.CreatePersistentSubscriptionAsync(
                stream,
                options.Group,
                this.settings,
                userCredentials
            ).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            this.logger.LogWarning(e, "{Message}", e.Message);
        }

        await connection.ConnectToPersistentSubscriptionAsync(
            stream,
            options.Group,
            (_, evt) => EventAppearedAsync<TEvent, TEventHandler>(evt, cancellationToken).ConfigureAwait(false),
            (sub, reason, exception) => this.logger.LogDebug("Subscription dropped: {Reason}", reason)
        ).ConfigureAwait(false);

        this.logger.LogInformation("Subscription to {Stream} created.", stream);
    }

    /// <summary>
    /// Handles the event when it appears in the EventStore.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="event">The resolved event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private Task EventAppearedAsync<TEvent, TEventHandler>(ResolvedEvent @event, CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var domainEvent = JsonSerializer.Deserialize<TEvent>(Encoding.UTF8.GetString(@event.Event.Data));

        var eventHandler = this.serviceProvider.GetRequiredService<TEventHandler>();

        return eventHandler.HandleAsync(@domainEvent, cancellationToken);
    }

    /// <summary>
    /// Unsubscribes from a domain event in the EventStore.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task UnsubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        return Task.CompletedTask;
    }
}