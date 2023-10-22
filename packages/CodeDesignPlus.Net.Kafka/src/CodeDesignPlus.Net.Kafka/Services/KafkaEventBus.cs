using System.Text.Json;
using CodeDesignPlus.Net.Event.Bus.Abstractions;
using CodeDesignPlus.Net.Kafka.Options;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Kafka.Services;

/// <summary>
/// Default implementation of the <see cref="IKafkaService"/>
/// </summary>
public class KafkaEventBus : IKafkaEventBus
{
    /// <summary>
    /// Logger Service
    /// </summary>
    private readonly ILogger<KafkaEventBus> logger;
    /// <summary>
    /// Kafka Options
    /// </summary>
    private readonly KafkaOptions options;
    /// <summary>
    /// The service provider    
    /// </summary>
    private readonly IServiceProvider serviceProvider;
    /// <summary>
    /// The subscription Manager
    /// </summary>
    private readonly ISubscriptionManager subscriptionManager;



    /// <summary>
    /// Initialize a new instance of the <see cref="KafkaEventBus"/>
    /// </summary>
    /// <param name="logger">Logger Service</param>
    /// <param name="options">Kafka Options</param>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="subscriptionManager">The subscription Manager</param>	
    public KafkaEventBus(ILogger<KafkaEventBus> logger, IOptions<KafkaOptions> options, ISubscriptionManager subscriptionManager, IServiceProvider serviceProvider)
    {
        this.logger = logger;
        this.options = options.Value;
        this.serviceProvider = serviceProvider;
        this.subscriptionManager = subscriptionManager;
    }

    public Task PublishAsync(EventBase @event, CancellationToken token)
    {
        return this.PublishAsync<DeliveryResult<string, object>>(@event, token);
    }

    public Task<TResult> PublishAsync<TResult>(EventBase @event, CancellationToken token)
    {
        var type = @event.GetType();

        var producerType = typeof(IProducer<,>).MakeGenericType(typeof(string), type);

        var producer = this.serviceProvider.GetRequiredService(producerType);

        var method = producer
            .GetType()
            .GetMethods()
            .Where(m => m.Name == "ProduceAsync" && m.GetParameters().Any(o => o.ParameterType == typeof(string)))
            .FirstOrDefault();

        var message = Activator.CreateInstance(typeof(Message<,>).MakeGenericType(typeof(string), type));

        return (Task<TResult>)method.Invoke(producer, new object[] { message });
    }

    public Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken token)
        where TEvent : EventBase
        where TEventHandler : IEventHandler<TEvent>
    {
        var consumer = this.serviceProvider.GetRequiredService<IConsumer<string, TEvent>>();

        token.Register(() => consumer.Close());

        while (!token.IsCancellationRequested)
        {
            try
            {
                var value = consumer.Consume(token);

                this.ListenerEvent<TEvent, TEventHandler>(value.Message.Key, value.Message.Value);
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Consume error: {e.Error.Reason}");

                // TODO: CodeDesignPlus.Net.Event.Bus.Services.QueueService.DequeueAsync
            }
        }

        return Task.CompletedTask;
    }

    public void ListenerEvent<TEvent, TEventHandler>(string key, TEvent @event)
       where TEvent : EventBase
       where TEventHandler : IEventHandler<TEvent>
    {
        this.logger.LogDebug($"Message received on the channel {typeof(TEvent).Name} with message {@event}");

        if (this.subscriptionManager.HasSubscriptionsForEvent<TEvent>())
        {
            var subscriptions = this.subscriptionManager.FindSubscriptions<TEvent>();

            foreach (var subscription in subscriptions)
            {
                this.logger.LogDebug($"The message will add to the queue with event {subscription.EventType.Name} and the handler {subscription.EventHandlerType.Name}");

                var queueType = typeof(IQueueService<,>);

                queueType = queueType.MakeGenericType(subscription.EventHandlerType, subscription.EventType);

                var queue = this.serviceProvider.GetService(queueType);

                queue.GetType().GetMethod(nameof(IQueueService<TEventHandler, TEvent>.Enqueue)).Invoke(queue, new object[] { @event });

                this.logger.LogDebug($"The message was added successfully");
            }
        }
    }

    public void Unsubscribe<TEvent, TEventHandler>()
        where TEvent : EventBase
        where TEventHandler : IEventHandler<TEvent>
    {
        var consumer = this.serviceProvider.GetRequiredService<IConsumer<string, TEvent>>();
        consumer.Close();
        consumer.Dispose();
    }

}
