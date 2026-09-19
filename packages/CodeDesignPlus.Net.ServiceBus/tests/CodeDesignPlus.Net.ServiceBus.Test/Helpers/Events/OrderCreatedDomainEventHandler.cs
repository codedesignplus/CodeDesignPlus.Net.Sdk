using CodeDesignPlus.Net.ServiceBus.Test.Helpers.Entities;

namespace CodeDesignPlus.Net.ServiceBus.Test.Helpers.Events;

[QueueName<OrderEntity>("provision_check_on_order_created")]
public class OrderCreatedDomainEventHandler(IMemoryHandler memoryHandler) : IEventHandler<OrderCreatedDomainEvent>
{
    public Task HandleAsync(OrderCreatedDomainEvent data, CancellationToken token)
    {
        memoryHandler.Record(data);

        return Task.CompletedTask;
    }
}
