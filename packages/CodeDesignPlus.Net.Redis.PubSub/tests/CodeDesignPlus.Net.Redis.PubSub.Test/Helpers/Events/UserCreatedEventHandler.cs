using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.Redis.PubSub.Test.Helpers.Memory;
using CodeDesignPlus.Net.Serializers;

namespace CodeDesignPlus.Net.Redis.PubSub.Test.Helpers.Events
{
    public class UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger, IMemoryService memory) : IEventHandler<UserCreatedEvent>
    {
        public Task HandleAsync(UserCreatedEvent data, CancellationToken token)
        {
            memory.UserEventTrace.Add(data);

            logger.LogDebug("Invoked Event: {Data}", JsonSerializer.Serialize(data));

            return Task.CompletedTask;
        }
    }
}
