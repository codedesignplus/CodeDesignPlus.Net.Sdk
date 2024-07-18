using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.RabbitMQ.Attributes;
using CodeDesignPlus.Net.RabbitMQ.Test.Helpers.Entities;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Helpers.Events;

[QueueName<NotificationEntity>("notify_email_on_user_created")]
public class UserCreatedDomainEventHandler(IMemoryHandler memoryHandler) : IEventHandler<UserCreatedDomainEvent>
{
    public Task HandleAsync(UserCreatedDomainEvent data, CancellationToken token)
    {
        memoryHandler.Memory.Add(data.AggregateId, data);

        if (data.Name == "Throw Exception")
            throw new Exception("Custom Error");

        return Task.CompletedTask;
    }
}
