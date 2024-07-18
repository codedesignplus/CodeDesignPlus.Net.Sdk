using CodeDesignPlus.Net.Kafka.Test.Helpers.Memory;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.Serializers;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers.Events
{
    public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
    {
        private readonly ILogger logger;

        private readonly IMemoryService memory;

        public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger, IMemoryService memory)
        {
            this.logger = logger;
            this.memory = memory;
        }

        public Task HandleAsync(UserCreatedEvent data, CancellationToken token)
        {
            this.memory.UserEventTrace.Add(data);

            this.logger.LogDebug("Invoked Event: {json}", JsonSerializer.Serialize(data));

            return Task.CompletedTask;
        }
    }
}
