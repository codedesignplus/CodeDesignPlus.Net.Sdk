using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using System.Collections.Concurrent;

namespace CodeDesignPlus.Net.PubSub.Services
{
    /// <summary>
    /// This service is responsible for managing the queue of events to be processed by the event handler
    /// </summary>
    /// <typeparam name="TEventHandler">Manejador de eventos</typeparam>
    /// <typeparam name="TEvent">Evento de Integración</typeparam>
    public class QueueService<TEventHandler, TEvent> : IQueueService<TEventHandler, TEvent>
        where TEventHandler : IEventHandler<TEvent>
        where TEvent : IDomainEvent
    {
        private readonly ConcurrentQueue<TEvent> queue = new();
        private readonly TEventHandler eventHandler;
        private readonly ILogger<QueueService<TEventHandler, TEvent>> logger;
        private readonly PubSubOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueService{TEventHandler, TEvent}"/> class.
        /// </summary>
        /// <param name="eventHandler">The event handler to process the events in the queue.</param>
        /// <param name="logger">The logger service.</param>
        /// <param name="options">The event bus options.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public QueueService(TEventHandler eventHandler, ILogger<QueueService<TEventHandler, TEvent>> logger, IOptions<PubSubOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            this.eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler)); ;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
            this.options = options.Value;

            this.logger.LogDebug("QueueService initialized.");
        }

        /// <summary>
        /// Gets the number of elements contained in the System.Collections.Concurrent.ConcurrentQueue`1.
        /// </summary>
        /// <returns>The number of elements contained in the System.Collections.Concurrent.ConcurrentQueue`1.</returns>
        public int Count => this.queue.Count;

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
        public bool Any() => !queue.IsEmpty;

        /// <summary>
        /// Enqueues the specified event.
        /// </summary>
        /// <param name="event">The event to be enqueued.</param>
        /// <exception cref="ArgumentNullException">throw if the event is null.</exception>
        public void Enqueue(TEvent @event)
        {
            if (@event == null)
            {
                this.logger.LogError("Attempted to enqueue a null event.");
                throw new ArgumentNullException(nameof(@event));
            }

            var exist = this.queue.Any(x => x.Equals(@event));

            if (!exist)
            {
                this.queue.Enqueue(@event);
                this.logger.LogDebug("Event of type {name} enqueued.", typeof(TEvent).Name);
            }
            else
            {
                this.logger.LogWarning("Event of type {name} was already in the queue. Skipping.", typeof(TEvent).Name);
            }
        }

        /// <summary>
        /// Dequeues the event from the queue and processes it with the event handler.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DequeueAsync(CancellationToken token)
        {
            this.logger.LogDebug("DequeueAsync started.");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (this.queue.TryDequeue(out TEvent @event))
                    {
                        this.logger.LogDebug("Dequeueing event of type {TEvent}.", typeof(TEvent).Name);

                        await this.eventHandler.HandleAsync(@event, token);
                    }
                    else
                    {
                        this.logger.LogDebug("No events in the queue of type {TEvent}. Waiting...", typeof(TEvent).Name);
                        await Task.Delay(TimeSpan.FromSeconds(this.options.SecondsWaitQueue), CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error processing event of type {TEvent}.", typeof(TEvent).Name);
                }
            }

            this.logger.LogDebug("DequeueAsync stopped due to cancellation token to type {TEvent}.", typeof(TEvent).Name);
        }
    }
}
