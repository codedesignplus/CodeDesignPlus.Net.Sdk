using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.RabbitMQ.Attributes;
using CodeDesignPlus.Net.Serializers;

namespace CodeDesignPlus.Net.RabbitMQ.Consumer.Sample;

[QueueName("userentity", "register-user")]
public class UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger) : IEventHandler<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent data, CancellationToken token)
    {
        logger.LogInformation("Invoked Event: {Json}", JsonSerializer.Serialize(data));

        return Task.CompletedTask;
    }
}