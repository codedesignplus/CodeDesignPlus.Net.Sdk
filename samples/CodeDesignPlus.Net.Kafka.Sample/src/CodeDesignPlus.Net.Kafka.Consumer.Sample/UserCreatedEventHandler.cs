using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.Serializers;

namespace CodeDesignPlus.Net.Kafka.Consumer.Sample;

public class UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger) : IEventHandler<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent data, CancellationToken token)
    {
        logger.LogDebug("Invoked Event: {Json}", JsonSerializer.Serialize(data));

        return Task.CompletedTask;
    }
}