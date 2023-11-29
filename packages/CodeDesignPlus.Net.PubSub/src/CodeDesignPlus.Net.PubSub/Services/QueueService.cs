using System.Collections.Concurrent;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;

namespace CodeDesignPlus.Net.PubSub.Services
{
    /// <summary>
    /// Implementación por defecto para el servicio <see cref="IQueueService{TEventHandler, TEvent}"/>
    /// </summary>
    /// <typeparam name="TEventHandler">Manejador de eventos</typeparam>
    /// <typeparam name="TEvent">Evento de Integración</typeparam>
    public class QueueService<TEventHandler, TEvent> : IQueueService<TEventHandler, TEvent>
        where TEventHandler : IEventHandler<TEvent>
        where TEvent : EventBase
    {
        private readonly ConcurrentQueue<TEvent> queueEvent = new();
        private readonly TEventHandler eventHandler;
        private readonly ILogger<QueueService<TEventHandler, TEvent>> logger;
        private readonly PubSubOptions options;

        /// <summary>
        /// Crea una nueva instancia de <see cref="QueueService{TEvent}"/>
        /// </summary>
        /// <param name="eventHandler">Event Handler</param>
        /// <param name="logger">The service logger</param>
        public QueueService(TEventHandler eventHandler, ILogger<QueueService<TEventHandler, TEvent>> logger, IOptions<PubSubOptions> options)
        {
            if(options == null)
                throw new ArgumentNullException(nameof(options));

            this.eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));;
            this.options = options.Value;

        this.logger.LogInformation("QueueService initialized.");
        }

        /// <summary>
        /// Gets the number of elements contained in the System.Collections.Concurrent.ConcurrentQueue`1.
        /// </summary>
        /// <returns>The number of elements contained in the System.Collections.Concurrent.ConcurrentQueue`1.</returns>
        public int Count => this.queueEvent.Count;

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
        public bool Any() => this.queueEvent.Any();

        /// <summary>
        /// Agrega un objeto al final de la queue
        /// </summary>
        /// <param name="event">El objeto a agregar al final de la Queu</param>
        /// <exception cref="ArgumentNullException">Se genera cuando <paramref name="event"/> es nulo</exception>
        public void Enqueue(TEvent @event)
        {
            if (@event == null)
            {
                this.logger.LogError("Attempted to enqueue a null event.");
                throw new ArgumentNullException(nameof(@event));
            }

            var exist = this.queueEvent.Any(x => x.Equals(@event));

            if (!exist)
            {
                this.queueEvent.Enqueue(@event);
                this.logger.LogInformation($"Event of type {typeof(TEvent).Name} enqueued.");
            }
            else
            {
                this.logger.LogWarning($"Event of type {typeof(TEvent).Name} was already in the queue. Skipping.");
            }
        }

        /// <summary>
        /// Tries to remove and return the object at the beginning of the concurrent queue.
        /// </summary>
        /// <param name="token">Cancellation Token</param>
        /// <returns>Return Task that represents an asynchronous operation.</returns>
        public async Task DequeueAsync(CancellationToken token)
        {
            this.logger.LogInformation("DequeueAsync started.");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (this.queueEvent.TryDequeue(out TEvent @event))
                    {
                        this.logger.LogInformation("Dequeueing event of type {TEvent}.", typeof(TEvent).Name);

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

            this.logger.LogInformation("DequeueAsync stopped due to cancellation token to type {TEvent}.", typeof(TEvent).Name);
        }
    }
}
