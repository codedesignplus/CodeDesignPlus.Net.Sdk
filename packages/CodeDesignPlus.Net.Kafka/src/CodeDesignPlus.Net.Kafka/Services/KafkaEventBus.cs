using System.Text;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using CodeDesignPlus.Net.Kafka.Options;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Kafka.Serializer;

namespace CodeDesignPlus.Net.Kafka.Services;

/// <summary>
/// Provides the default implementation for the <see cref="IKafkaService"/> interface.
/// </summary>
public class KafkaEventBus : IKafkaEventBus
{
    private readonly IDomainEventResolverService domainEventResolverService;
    private readonly ILogger<KafkaEventBus> logger;
    private readonly KafkaOptions options;
    private readonly IServiceProvider serviceProvider;
    private readonly PubSubOptions pubSubOptions;


    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaEventBus"/> class.
    /// </summary>
    /// <param name="logger">Service for logging.</param>
    /// <param name="options">Configuration options for Kafka.</param>
    /// <param name="serviceProvider">Provides an instance of a service.</param>
    /// <param name="pubSubOptions">Configuration options for the event bus.</param>
    public KafkaEventBus(ILogger<KafkaEventBus> logger, IDomainEventResolverService domainEventResolverService, IOptions<KafkaOptions> options, IServiceProvider serviceProvider, IOptions<PubSubOptions> pubSubOptions)
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

        var headers = new Headers();

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

        var producerBuilder = new ProducerBuilder<string, IDomainEvent>(options.ProducerConfig);

        producerBuilder.SetValueSerializer(new JsonSystemTextSerializer<IDomainEvent>());
        
        var producer = producerBuilder.Build();

        await producer.ProduceAsync(topic, message, token);

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
        try
        {

            this.logger.LogInformation("Subscribing to Kafka topic for event type: {EventType}", typeof(TEvent).Name);

            var consumerBuilder = new ConsumerBuilder<string, TEvent>(this.options.ConsumerConfig);
            consumerBuilder.SetValueDeserializer(new JsonSystemTextSerializer<TEvent>());

            using var consumer = consumerBuilder.Build();

            cancellationToken.Register(consumer.Close);

            var topic = this.domainEventResolverService.GetKeyDomainEvent(typeof(TEvent));

            consumer.Subscribe(topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    this.logger.LogInformation("Listener the event {EventType}", typeof(TEvent).Name);
                    var value = consumer.Consume(cancellationToken);

                    await this.ProcessEventAsync<TEvent, TEventHandler>(value.Message.Key, value.Message.Value, cancellationToken);

                    this.logger.LogInformation("End Listener the event {EventType}", typeof(TEvent).Name);

                }
                catch (ConsumeException e)
                {
                    this.logger.LogError(e, "An error occurred while consuming a Kafka message for event type: {EventType}", typeof(TEvent).Name);
                }
            }

            this.logger.LogInformation("Kafka event listening has stopped for event type: {EventType} due to cancellation request.", typeof(TEvent).Name);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "An error occurred while consuming a Kafka message for event type: {EventType}", typeof(TEvent).Name);
        }

    }

    /// <summary>
    /// Processes a received event.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <typeparam name="TEventHandler">The type of event handler.</typeparam>
    /// <param name="key">The key of the message.</param>
    /// <param name="event">The received event.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task ProcessEventAsync<TEvent, TEventHandler>(string key, TEvent @event, CancellationToken cancellationToken)
       where TEvent : IDomainEvent
       where TEventHandler : IEventHandler<TEvent>
    {
        if (this.pubSubOptions.EnableQueue)
        {
            var queue = this.serviceProvider.GetRequiredService<IQueueService<TEventHandler, TEvent>>();

            queue.Enqueue(@event);
        }
        else
        {
            var eventHandler = this.serviceProvider.GetRequiredService<TEventHandler>();

            await eventHandler.HandleAsync(@event, cancellationToken);
        }
    }

    /// <summary>
    /// Unsubscribes from a specific event in Kafka.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <typeparam name="TEventHandler">The type of event handler.</typeparam>
    public Task UnsubscribeAsync<TEvent, TEventHandler>()
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        this.logger.LogInformation("Unsubscribing from event {EventType} for handler {EventHandlerType}", typeof(TEvent).Name, typeof(TEventHandler).Name);

        var consumer = this.serviceProvider.GetRequiredService<IConsumer<string, TEvent>>();
        consumer.Unsubscribe();

        return Task.CompletedTask;
    }

}
