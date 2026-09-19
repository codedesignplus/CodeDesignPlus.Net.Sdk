using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions.Attributes;
using CodeDesignPlus.Net.Serializers;

namespace CodeDesignPlus.Net.ServiceBus.Consumer.Sample;

[QueueName("userentity", "register-user")]
public class UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger) : IEventHandler<UserCreatedDomainEvent>
{
    public Task HandleAsync(UserCreatedDomainEvent data, CancellationToken token)
    {
        logger.LogInformation("Invoked Event: {Json}", JsonSerializer.Serialize(data));

        return Task.CompletedTask;
    }
}
