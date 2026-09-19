using CodeDesignPlus.Net.ServiceBus.Test.Helpers.Entities;

namespace CodeDesignPlus.Net.ServiceBus.Test.Helpers.Events;

[QueueName<NotificationEntity>("notify_email_on_user_created")]
public class UserCreatedDomainEventHandler(IMemoryHandler memoryHandler) : IEventHandler<UserCreatedDomainEvent>
{
    public Task HandleAsync(UserCreatedDomainEvent data, CancellationToken token)
    {
        memoryHandler.Record(data);

        if (data.Name == "Throw Infrastructure Exception" && memoryHandler.Attempts(data.AggregateId) <= 3)
            throw new InvalidOperationException("Transient failure");

        if (data.Name == "Throw Business Exception")
            throw new CodeDesignPlus.Net.Exceptions.CodeDesignPlusException(CodeDesignPlus.Net.Exceptions.Layer.Domain, "100", "Business rule violated");

        return Task.CompletedTask;
    }
}
