using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using System.Collections.Concurrent;

namespace CodeDesignPlus.Net.PubSub.Services
{
    public class EventQueueService: IEventQueueService
    {
        private readonly ConcurrentQueue<IDomainEvent> queue = new();
        private readonly ILogger<EventQueueService> logger;
        private readonly IEnumerable<IMessage> messages;
        private readonly PubSubOptions options;

        public EventQueueService(ILogger<EventQueueService> logger, IOptions<PubSubOptions> options, IEnumerable<IMessage> messages)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.messages = messages;
            
            this.options = options.Value;

            this.logger.LogDebug("EventQueueService initialized.");
        }

        public Task EnqueueAsync(IDomainEvent @event, CancellationToken cancellationToken)
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
                this.logger.LogDebug("Event of type {name} enqueued.", @event.GetType().Name);
            }
            else
            {
                this.logger.LogWarning("Event of type {name} was already in the queue. Skipping.", @event.GetType().Name);
            }

            return Task.CompletedTask;
        }

        public async Task DequeueAsync(CancellationToken token)
        {
            this.logger.LogDebug("DequeueAsync started.");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (this.queue.TryDequeue(out IDomainEvent @event))
                    {
                        this.logger.LogDebug("Dequeueing event of type {TEvent}.", @event.GetType().Name);

                        var tasks = this.messages.Select(x => x.PublishAsync(@event, token));

                        await Task.WhenAll(tasks);
                    }
                    else
                    {
                        this.logger.LogDebug("No events in the queue. Waiting...");

                        await Task.Delay(TimeSpan.FromSeconds(this.options.SecondsWaitQueue), CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error processing event.");
                }
            }

            this.logger.LogDebug("DequeueAsync stopped due to cancellation token.");
        }
    }
}
