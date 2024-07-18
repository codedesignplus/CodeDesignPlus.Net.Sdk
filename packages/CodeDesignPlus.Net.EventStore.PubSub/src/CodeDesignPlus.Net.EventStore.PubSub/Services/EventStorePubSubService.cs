using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.EventStore.Abstractions;
using CodeDesignPlus.Net.EventStore.PubSub.Abstractions.Options;
using CodeDesignPlus.Net.PubSub.Abstractions;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.Serializers;
using System.Text;

namespace CodeDesignPlus.Net.EventStore.PubSub.Services;

public class EventStorePubSubService : IEventStorePubSubService
{
    private readonly IEventStoreFactory eventStoreFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<EventStorePubSubService> logger;
    private readonly EventStorePubSubOptions options;
    private readonly PersistentSubscriptionSettings settings;

    private readonly IDomainEventResolverService domainEventResolverService;

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
    /// Publish a list of domain events
    /// </summary>
    /// <param name="event">Domains event to publish</param>
    /// <param name="cancellationToken">The cancellation token that will be assigned to the new task.</param>
    /// <returns>Return a <see cref="Task"/></returns>
    public Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken)
    {
        var tasks = @event.Select(@event => this.PublishAsync(@event, cancellationToken));

        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Subscribes to the specified event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event that was received.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler that was handling the event.</typeparam>
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
            this.logger.LogWarning(e, "{message}", e.Message);
        }


        await connection.ConnectToPersistentSubscriptionAsync(
                stream,
                options.Group,
                (_, evt) => EventAppearedAsync<TEvent, TEventHandler>(evt, cancellationToken).ConfigureAwait(false),
                (sub, reason, exception) => this.logger.LogDebug("Subscription dropped: {reason}", reason)
            ).ConfigureAwait(false);

        this.logger.LogInformation("Subscription to {stream} created.", stream);
    }

    /// <summary>
    /// Handles the event that was received from the EventStore.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event that was received.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler that was handling the event.</typeparam>
    /// <param name="event">The event that was received.</param>
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
    /// Unsubscribes from the specified event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to unsubscribe from.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler that was handling the event.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task UnsubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        return Task.CompletedTask;
    }


}
