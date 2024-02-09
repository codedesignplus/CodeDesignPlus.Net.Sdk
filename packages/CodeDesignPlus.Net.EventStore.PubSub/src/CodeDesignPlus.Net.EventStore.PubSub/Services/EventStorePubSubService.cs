using System.Text;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.EventStore.Abstractions;
using EventStore.ClientAPI;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using EventStore.ClientAPI.SystemData;
using CodeDesignPlus.Net.EventStore.PubSub.Abstractions.Options;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.EventStore.Serializer;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;

namespace CodeDesignPlus.Net.EventStore.PubSub.Services;

public class EventStorePubSubService : IEventStorePubSubService, IPubSub
{
    private readonly JsonSerializerSettings jsonSettings = new()
    {
        ContractResolver = new EventStoreContratResolver([]),
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
    };
    
    private readonly IEventStoreFactory eventStoreFactory;
    private readonly ISubscriptionManager subscriptionManager;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<EventStorePubSubService> logger;
    private readonly EventStorePubSubOptions options;
    private readonly PersistentSubscriptionSettingsBuilder settings;

    private readonly IDomainEventResolverService domainEventResolverService;
    private readonly PubSubOptions pubSubOptions;

    public EventStorePubSubService(
        IEventStoreFactory eventStoreFactory,
        ISubscriptionManager subscriptionManager,
        IServiceProvider serviceProvider,
        ILogger<EventStorePubSubService> logger,
        IOptions<EventStorePubSubOptions> eventStorePubSubOptions,
        IOptions<PubSubOptions> pubSubOptions,
        IDomainEventResolverService domainEventResolverService)
    {
        ArgumentNullException.ThrowIfNull(eventStorePubSubOptions, nameof(eventStorePubSubOptions));
        ArgumentNullException.ThrowIfNull(pubSubOptions, nameof(pubSubOptions));

        this.eventStoreFactory = eventStoreFactory ?? throw new ArgumentNullException(nameof(eventStoreFactory));
        this.subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.domainEventResolverService = domainEventResolverService ?? throw new ArgumentNullException(nameof(domainEventResolverService));
        this.pubSubOptions = pubSubOptions.Value;
        this.options = eventStorePubSubOptions.Value;

        this.settings = PersistentSubscriptionSettings
            .Create()
            .StartFromCurrent();

        this.logger.LogInformation("EventStorePubSubService initialized.");
    }


    public async Task PublishAsync(IDomainEvent @event, CancellationToken token)
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core, token);

        var stream = this.domainEventResolverService.GetKeyDomainEvent(@event.GetType());

        @event.Metadata.Add("OccurredAt", @event.OccurredAt);
        @event.Metadata.Add("EventId", @event.EventId);
        @event.Metadata.Add("EventType", @event.EventType);

        var eventData = new EventData(
            @event.EventId,
            @event.EventType,
            true,
           Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, this.jsonSettings)),
           Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event.Metadata, this.jsonSettings)));

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
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core, token);

        var (user, pass) = this.eventStoreFactory.GetCredentials(EventStoreFactoryConst.Core);

        var stream = this.domainEventResolverService.GetKeyDomainEvent<TEvent>();

        var userCredentials = new UserCredentials(user, pass);

        await connection.CreatePersistentSubscriptionAsync(
            stream,
            options.Group,
            this.settings,
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
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task EventAppearedAsync<TEvent, TEventHandler>(ResolvedEvent @event, CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var domainEvent = JsonConvert.DeserializeObject<TEvent>(Encoding.UTF8.GetString(@event.Event.Data), this.jsonSettings);
        
        if (this.subscriptionManager.HasSubscriptionsForEvent<TEvent>())
        {
            var subscriptions = this.subscriptionManager.FindSubscriptions<TEvent>();

            foreach (var subscription in subscriptions)
            {
                this.logger.LogDebug("Event {EventType} is being handled by {EventHandlerType}", subscription.EventType.Name, subscription.EventHandlerType.Name);

                if (this.pubSubOptions.EnableQueue)
                {
                    var queue = this.serviceProvider.GetRequiredService<IQueueService<TEventHandler, TEvent>>();

                    queue.Enqueue(@domainEvent);
                }
                else
                {
                    var eventHandler = this.serviceProvider.GetRequiredService<TEventHandler>();

                    await eventHandler.HandleAsync(@domainEvent, cancellationToken);
                }

                this.logger.LogDebug("Event {EventType} was successfully processed by handler {EventHandlerType}", subscription.EventType.Name, subscription.EventHandlerType.Name);
            }
        }
        else
        {
            this.logger.LogWarning("No subscriptions found for event type {EventType}. Skipping processing.", typeof(TEvent).Name);
        }
    }

    /// <summary>
    /// Unsubscribes from the specified event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to unsubscribe from.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler that was handling the event.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task UnsubscribeAsync<TEvent, TEventHandler>()
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        return Task.CompletedTask;
    }
}
