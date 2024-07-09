using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.PubSub.Services
{
    public class PubSubService : IPubSub
    {
        private readonly IMessage message;
        private readonly IOptions<PubSubOptions> options;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<PubSubService> logger;

        public PubSubService(IMessage message, IOptions<PubSubOptions> options, IServiceProvider serviceProvider, ILogger<PubSubService> logger)
        {
            ArgumentNullException.ThrowIfNull(message);
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(logger);

            this.message = message;
            this.options = options;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            
            this.logger.LogDebug("PubSubService initialized.");
        }

        public Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken)
        {
            if (this.options.Value.UseQueue)
            {
                this.logger.LogDebug("UseQueue is true, enqueuing event of type {name}.", @event.GetType().Name);

                var eventQueueService = this.serviceProvider.GetRequiredService<IEventQueueService>();

                return eventQueueService.EnqueueAsync(@event, cancellationToken);
            }

            this.logger.LogDebug("UseQueue is false, publishing event of type {name}.", @event.GetType().Name);

            return this.message.PublishAsync(@event, cancellationToken);
        }

        public Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken)
        {
            var tasks = @event.Select(@event => this.PublishAsync(@event, cancellationToken));

            return Task.WhenAll(tasks);
        }
    }
}
