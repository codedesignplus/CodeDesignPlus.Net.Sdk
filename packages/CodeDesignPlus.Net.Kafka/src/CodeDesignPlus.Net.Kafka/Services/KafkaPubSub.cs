using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Kafka.Options;
using CodeDesignPlus.Net.Kafka.Serializer;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace CodeDesignPlus.Net.Kafka.Services;

/// <summary>
/// Provides the default implementation for the <see cref="IKafkaService"/> interface.
/// </summary>
public class KafkaPubSub : IKafkaPubSub
{
    private readonly IDomainEventResolverService domainEventResolverService;
    private readonly ILogger<KafkaPubSub> logger;
    private readonly KafkaOptions options;
    private readonly IServiceProvider serviceProvider;
    private readonly PubSubOptions pubSubOptions;


    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaPubSub"/> class.
    /// </summary>
    /// <param name="logger">Service for logging.</param>
    /// <param name="options">Configuration options for Kafka.</param>
    /// <param name="serviceProvider">Provides an instance of a service.</param>
    /// <param name="pubSubOptions">Configuration options for the event bus.</param>
    public KafkaPubSub(ILogger<KafkaPubSub> logger, IDomainEventResolverService domainEventResolverService, IOptions<KafkaOptions> options, IServiceProvider serviceProvider, IOptions<PubSubOptions> pubSubOptions)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (pubSubOptions == null)
            throw new ArgumentNullException(nameof(pubSubOptions));

        this.domainEventResolverService = domainEventResolverService ?? throw new ArgumentNullException(nameof(domainEventResolverService));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.options = options.Value;
        this.pubSubOptions = pubSubOptions.Value;
    }

    /// <summary>
    /// Publishes an event to Kafka.
    /// </summary>
    /// <param name="event">The event to be published.</param>
    /// <param name="token">Cancellation token.</param>
    public async Task PublishAsync(IDomainEvent @event, CancellationToken token)
    {
        var type = @event.GetType();

        this.logger.LogInformation("Starting to publish event to Kafka. Event type: {EventType}", type.Name);

        var topic = this.domainEventResolverService.GetKeyDomainEvent(type);

        var headers = new Headers
        {
            { "EventType", Encoding.UTF8.GetBytes(@event.EventType) },
            { "OccurredAt", Encoding.UTF8.GetBytes(@event.OccurredAt.ToString()) },
            { "EventId", Encoding.UTF8.GetBytes(@event.EventId.ToString()) },
        };

        foreach (var item in @event.Metadata)
        {
            headers.Add(item.Key, Encoding.UTF8.GetBytes(item.Value?.ToString() ?? string.Empty));
        }

        var message = new Message<string, IDomainEvent>
        {
            Key = @event.EventId.ToString(),
            Value = @event,
            Headers = headers
        };

        var producer = this.serviceProvider.GetRequiredService<IProducer<string, IDomainEvent>>();

        await producer.ProduceAsync(topic, message, token).ConfigureAwait(false);

        this.logger.LogInformation("Event published to Kafka successfully. Event type: {EventType}", @event.GetType().Name);
    }

    /// <summary>
    /// Subscribes to a specific event from Kafka.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <typeparam name="TEventHandler">The type of event handler.</typeparam>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        this.logger.LogInformation("Subscribing to Kafka topic for event type: {EventType}", typeof(TEvent).Name);

        var consumerBuilder = new ConsumerBuilder<string, TEvent>(this.options.ConsumerConfig);

        consumerBuilder.SetValueDeserializer(new JsonSystemTextSerializer<TEvent>());

        using var consumer = consumerBuilder.Build();

        cancellationToken.Register(() =>
        {
            consumer.Close();
        });

        var topic = this.domainEventResolverService.GetKeyDomainEvent<TEvent>();

        consumer.Subscribe(topic);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                this.logger.LogInformation("Listener the event {EventType}", typeof(TEvent).Name);
                var value = consumer.Consume(cancellationToken);

                var eventHandler = this.serviceProvider.GetRequiredService<TEventHandler>();

                await eventHandler.HandleAsync(value.Message.Value, cancellationToken).ConfigureAwait(false);

                this.logger.LogInformation("End Listener the event {EventType}", typeof(TEvent).Name);

            }
            catch (ConsumeException e)
            {
                this.logger.LogError(e, "An error occurred while consuming a Kafka message for event type: {EventType}", typeof(TEvent).Name);
            }
        }

        this.logger.LogInformation("Kafka event listening has stopped for event type: {EventType} due to cancellation request.", typeof(TEvent).Name);
    }

    /// <summary>
    /// Unsubscribes from a specific event in Kafka.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <typeparam name="TEventHandler">The type of event handler.</typeparam>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task UnsubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        this.logger.LogInformation("Unsubscribing from event {EventType} for handler {EventHandlerType}", typeof(TEvent).Name, typeof(TEventHandler).Name);

        var consumer = this.serviceProvider.GetRequiredService<IConsumer<string, TEvent>>();
        consumer.Unsubscribe();

        return Task.CompletedTask;
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

}
