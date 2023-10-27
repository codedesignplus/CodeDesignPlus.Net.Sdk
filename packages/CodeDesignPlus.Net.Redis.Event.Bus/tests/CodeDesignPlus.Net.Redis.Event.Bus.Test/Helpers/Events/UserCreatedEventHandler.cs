﻿using CodeDesignPlus.Net.Event.Bus.Abstractions;
using CodeDesignPlus.Net.Redis.Event.Bus.Test.Helpers.Memory;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Redis.Event.Bus.Test.Helpers.Events
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

            this.logger.LogDebug("Invoked Event: {0}", JsonConvert.SerializeObject(data));

            return Task.CompletedTask;
        }
    }
}