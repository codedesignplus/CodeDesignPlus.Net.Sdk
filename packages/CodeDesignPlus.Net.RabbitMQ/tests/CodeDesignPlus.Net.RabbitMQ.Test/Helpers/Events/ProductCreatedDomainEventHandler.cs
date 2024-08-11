using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.RabbitMQ.Attributes;
using CodeDesignPlus.Net.RabbitMQ.Test.Helpers.Entities;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Helpers.Events;


[QueueName<ProductEntity>("notify_email_on_user_created")]
public class ProductCreatedDomainEventHandler(IMemoryHandler memoryHandler) : IEventHandler<ProductCreatedDomainEvent>
{
    public Task HandleAsync(ProductCreatedDomainEvent data, CancellationToken token)
    {
        memoryHandler.Memory.Add(data.AggregateId, data);

        return Task.CompletedTask;
    }
}
