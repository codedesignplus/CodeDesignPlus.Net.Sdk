﻿using CodeDesignPlus.Net.Kafka.Test.Helpers.Events;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers.Memory
{
    public class MemoryService : IMemoryService
    {
        public List<UserCreatedEvent> UserEventTrace { get; private set; } = new();

        public List<ProductCreatedEvent> ProductEventTrace { get; private set; } = new();
    }
}
