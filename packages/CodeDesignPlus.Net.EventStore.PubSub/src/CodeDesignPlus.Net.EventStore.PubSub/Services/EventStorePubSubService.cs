using System.Text;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using CodeDesignPlus.Net.EventStore.Abstractions;
using CodeDesignPlus.Net.EventStore.Abstractions.Options;
using EventStore.ClientAPI;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using EventStore.ClientAPI.SystemData;
using CodeDesignPlus.Net.EventStore.PubSub.Abstractions.Options;

namespace CodeDesignPlus.Net.EventStore.PubSub.Services;

public class EventStorePubSubService : IEventStorePubSubService, IPubSub
{
    private readonly IEventStoreFactory eventStoreFactory;
    private readonly ISubscriptionManager subscriptionManager;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<EventStorePubSubService> logger;
    private readonly EventStorePubSubOptions options;
    private readonly PersistentSubscriptionSettingsBuilder settings;

    public EventStorePubSubService(
        IEventStoreFactory eventStoreFactory,
        ISubscriptionManager subscriptionManager,
        IServiceProvider serviceProvider,
        ILogger<EventStorePubSubService> logger,
        IOptions<EventStorePubSubOptions> eventStorePubSubOptions)
    {
        if (eventStorePubSubOptions == null)
            throw new ArgumentNullException(nameof(eventStorePubSubOptions));

        this.eventStoreFactory = eventStoreFactory ?? throw new ArgumentNullException(nameof(eventStoreFactory));
        this.subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = eventStorePubSubOptions.Value;


        this.settings = PersistentSubscriptionSettings
            .Create()
            .StartFromCurrent();

        this.logger.LogInformation("RedisPubSubService initialized.");
    }


    public async Task PublishAsync(EventBase @event, CancellationToken token)
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var stream = @event.GetType().Name;

        var eventData = new EventData(
            @event.IdEvent,
            @event.GetType().Name,
            true,
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
            null);

        await connection.AppendToStreamAsync(stream, ExpectedVersion.Any, eventData);
    }

    /// <summary>
    /// Subscribes to the specified event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event that was received.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler that was handling the event.</typeparam>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken token)
        where TEvent : EventBase
        where TEventHandler : IEventHandler<TEvent>
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);
        var (user, pass) = this.eventStoreFactory.GetCredentials(EventStoreFactoryConst.Core);

        var stream = typeof(TEvent).Name;

        var userCredentials = new UserCredentials(user, pass);

        var settings = PersistentSubscriptionSettings
            .Create()
            .StartFromCurrent();

        await connection.CreatePersistentSubscriptionAsync(
            stream,
            options.Group,
            settings,
            userCredentials
        );

        var subscription = await connection.ConnectToPersistentSubscriptionAsync(
            stream,
            options.Group,
            (_, evt) => EventAppearedAsync<TEvent, TEventHandler>(evt, token),
            (sub, reason, exception) => this.logger.LogDebug("Subscription dropped: {reason}", reason)
        );
    }

    /// <summary>
    /// Handles the event that was received from the EventStore.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event that was received.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler that was handling the event.</typeparam>
    /// <param name="event">The event that was received.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task EventAppearedAsync<TEvent, TEventHandler>(ResolvedEvent @event, CancellationToken token)
        where TEvent : EventBase
        where TEventHandler : IEventHandler<TEvent>
    {
        var domainEvent = JsonConvert.DeserializeObject<TEvent>(Encoding.UTF8.GetString(@event.Event.Data));

        var projectionHandler = this.serviceProvider.GetRequiredService<TEventHandler>();

        await projectionHandler.HandleAsync(domainEvent, token);
    }

    /// <summary>
    /// Unsubscribes from the specified event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to unsubscribe from.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler that was handling the event.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task UnsubscribeAsync<TEvent, TEventHandler>()
        where TEvent : EventBase
        where TEventHandler : IEventHandler<TEvent>
    {
        return Task.CompletedTask;
    }
}
