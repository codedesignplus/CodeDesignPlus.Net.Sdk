using System.Reflection;
using System.Text;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using CodeDesignPlus.Net.Kafka.Options;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using KafkaException = CodeDesignPlus.Net.Kafka.Exceptions.KafkaException;

namespace CodeDesignPlus.Net.Kafka.Services;

/// <summary>
/// Provides the default implementation for the <see cref="IKafkaService"/> interface.
/// </summary>
public class KafkaEventBus : IKafkaEventBus
{
    private readonly ILogger<KafkaEventBus> logger;
    private readonly KafkaOptions options;
    private readonly IServiceProvider serviceProvider;
    private readonly ISubscriptionManager subscriptionManager;
    private readonly PubSubOptions pubSubOptions;


    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaEventBus"/> class.
    /// </summary>
    /// <param name="logger">Service for logging.</param>
    /// <param name="options">Configuration options for Kafka.</param>
    /// <param name="serviceProvider">Provides an instance of a service.</param>
    /// <param name="subscriptionManager">Manages event subscriptions.</param>	
    /// <param name="pubSubOptions">Configuration options for the event bus.</param>
    public KafkaEventBus(ILogger<KafkaEventBus> logger, IOptions<KafkaOptions> options, ISubscriptionManager subscriptionManager, IServiceProvider serviceProvider, IOptions<PubSubOptions> pubSubOptions)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (pubSubOptions == null)
            throw new ArgumentNullException(nameof(pubSubOptions));

        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
        this.options = options.Value;
        this.pubSubOptions = pubSubOptions.Value;
    }

    /// <summary>
    /// Publishes an event to Kafka.
    /// </summary>
    /// <param name="event">The event to be published.</param>
    /// <param name="token">Cancellation token.</param>
    public async Task PublishAsync(EventBase @event, CancellationToken token)
    {
        this.logger.LogInformation("Starting to publish event to Kafka. Event type: {EventType}", @event.GetType().Name);

        var type = @event.GetType();

        var topic = type.GetCustomAttribute<TopicAttribute>();

        if (string.IsNullOrEmpty(topic?.Name))
        {
            this.logger.LogError("Failed to publish event to Kafka. The topic is missing.");
            throw new KafkaException("The topic is required");
        }

        var producerType = typeof(IProducer<,>).MakeGenericType(typeof(string), type);

        var producer = this.serviceProvider.GetRequiredService(producerType);

        var method = producer
            .GetType()
            .GetMethods()
            .Where(m => m.Name == "ProduceAsync" && m.GetParameters().Any(o => o.ParameterType == typeof(string)))
            .FirstOrDefault();

        var message = Activator.CreateInstance(typeof(Message<,>).MakeGenericType(typeof(string), type));

        message.GetType().GetProperty("Key").SetValue(message, @event.IdEvent.ToString());
        message.GetType().GetProperty("Value").SetValue(message, @event);

        var headers = new Headers
        {
            { "EventType", Encoding.UTF8.GetBytes(type.Name) },
            { "Topic", Encoding.UTF8.GetBytes(topic.Name) }
        };

        message.GetType().GetProperty("Headers").SetValue(message, headers);

        await (Task)method.Invoke(producer, new object[] { topic.Name, message, token });

        this.logger.LogInformation("Event published to Kafka successfully. Event type: {EventType}", @event.GetType().Name);
    }

    /// <summary>
    /// Subscribes to a specific event from Kafka.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <typeparam name="TEventHandler">The type of event handler.</typeparam>
    /// <param name="token">Cancellation token.</param>
    public async Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken token)
        where TEvent : EventBase
        where TEventHandler : IEventHandler<TEvent>
    {
        this.logger.LogInformation("Subscribing to Kafka topic for event type: {EventType}", typeof(TEvent).Name);

        var consumer = this.serviceProvider.GetRequiredService<IConsumer<string, TEvent>>();

        token.Register(consumer.Close);

        var topic = typeof(TEvent).GetCustomAttribute<TopicAttribute>();

        if (string.IsNullOrEmpty(topic?.Name))
        {
            this.logger.LogError("Failed to subscribe to Kafka topic. Topic name is missing.");
            throw new KafkaException("Topic name is required");
        }

        consumer.Subscribe(topic.Name);

        while (!token.IsCancellationRequested)
        {
            try
            {
                this.logger.LogInformation("Listener the event {EventType}", typeof(TEvent).Name);
                var value = consumer.Consume(token);

                await this.ProcessEventAsync<TEvent, TEventHandler>(value.Message.Key, value.Message.Value, token);

                this.logger.LogInformation("End Listener the event {EventType}", typeof(TEvent).Name);

            }
            catch (ConsumeException e)
            {
                this.logger.LogError(e, "An error occurred while consuming a Kafka message for event type: {EventType}", typeof(TEvent).Name);
            }

            await Task.Delay(TimeSpan.FromSeconds(1), CancellationToken.None);
        }

        this.logger.LogInformation("Kafka event listening has stopped for event type: {EventType} due to cancellation request.", typeof(TEvent).Name);
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
       where TEvent : EventBase
       where TEventHandler : IEventHandler<TEvent>
    {
        this.logger.LogDebug("Processing received event of type {EventType} with key {EventKey}", typeof(TEvent).Name, key);

        if (this.subscriptionManager.HasSubscriptionsForEvent<TEvent>())
        {
            var subscriptions = this.subscriptionManager.FindSubscriptions<TEvent>();

            foreach (var subscription in subscriptions)
            {
                this.logger.LogDebug("Event {EventType} is being handled by {EventHandlerType}", subscription.EventType.Name, subscription.EventHandlerType.Name);

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

                this.logger.LogDebug("Event {EventType} was successfully processed by handler {EventHandlerType}", subscription.EventType.Name, subscription.EventHandlerType.Name);
            }
        }
        else
        {
            this.logger.LogWarning("No subscriptions found for event type {EventType}. Skipping processing.", typeof(TEvent).Name);
        }
    }

    /// <summary>
    /// Unsubscribes from a specific event in Kafka.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <typeparam name="TEventHandler">The type of event handler.</typeparam>
    public Task UnsubscribeAsync<TEvent, TEventHandler>()
        where TEvent : EventBase
        where TEventHandler : IEventHandler<TEvent>
    {
        this.logger.LogInformation("Unsubscribing from event {EventType} for handler {EventHandlerType}", typeof(TEvent).Name, typeof(TEventHandler).Name);

        var consumer = this.serviceProvider.GetRequiredService<IConsumer<string, TEvent>>();
        consumer.Unsubscribe();

        return Task.CompletedTask;
    }

}
