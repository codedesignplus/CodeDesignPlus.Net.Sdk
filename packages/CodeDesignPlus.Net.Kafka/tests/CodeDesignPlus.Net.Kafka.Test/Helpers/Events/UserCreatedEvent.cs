﻿using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.Kafka.Test.Helpers.Entities;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers.Events
{
    [EventKey<UserEntity>(1, "created")]
    public class UserCreatedEvent(Guid aggregateId, Guid? eventId = null, Instant? occurredAt = null, Dictionary<string, object> metadata = null!)
        : DomainEvent(aggregateId, eventId, occurredAt, metadata)
    {
        public string? Username { get; set; }
        public string? Names { get; set; }
        public string? Lastnames { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
