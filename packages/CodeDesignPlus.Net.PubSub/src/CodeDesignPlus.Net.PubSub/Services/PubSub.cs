using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeDesignPlus.Net.PubSub.Services
{
    public class PubSub : IPubSub
    {
        private readonly IEnumerable<IMessage> messages;
        private readonly IOptions<PubSubOptions> options;
        private readonly IServiceProvider serviceProvider;

        public PubSub(IEnumerable<IMessage> meessages, IOptions<PubSubOptions> options, IServiceProvider serviceProvider)
        {
            this.messages = meessages;
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

            var tasks = this.messages.Select(x => x.PublishAsync(@event, cancellationToken));

            return Task.WhenAll(tasks);
        }

        public Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken)
        {
            var tasks = @event.Select(@event => this.PublishAsync(@event, cancellationToken));

            return Task.WhenAll(tasks);
        }
    }
}
