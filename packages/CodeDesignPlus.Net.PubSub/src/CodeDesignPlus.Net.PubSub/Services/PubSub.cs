using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.PubSub.Services
{
    public class PubSub : IPubSub
    {
        private readonly IMessage message;
        private readonly IOptions<PubSubOptions> options;
        private readonly IServiceProvider serviceProvider;

        public PubSub(IMessage meessage, IOptions<PubSubOptions> options, IServiceProvider serviceProvider)
        {
            this.message = meessage;
            this.options = options;
            this.serviceProvider = serviceProvider;
        }

        public Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken)
        {
            if (this.options.Value.UseQueue)
            {
                var eventQueueService = this.serviceProvider.GetRequiredService<IEventQueueService>();

                return eventQueueService.EnqueueAsync(@event, cancellationToken);
            }

            return this.message.PublishAsync(@event, cancellationToken);
        }

        public Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken)
        {
            var tasks = @event.Select(@event => this.PublishAsync(@event, cancellationToken));

            return Task.WhenAll(tasks);
        }
    }
}
