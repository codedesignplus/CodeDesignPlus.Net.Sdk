using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.PubSub.Services
{
    public class PubSub(IMessage meessage, IOptions<PubSubOptions> options, IServiceProvider serviceProvider) : IPubSub
    {
        private readonly IMessage message = meessage;
        private readonly IOptions<PubSubOptions> options = options;
        private readonly IServiceProvider serviceProvider = serviceProvider;

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
