﻿using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers.Events
{
    [Key("user-topic")]
    public class UserCreatedEvent : DomainEvent
    {
        public UserCreatedEvent(Guid aggregateId, Guid? eventId = null, DateTime? occurredAt = null, Dictionary<string, object> metadata = null) 
            : base(aggregateId, eventId, occurredAt, metadata)
        {
        }

        public string? Username { get; set; }
        public string? Names { get; set; }
        public string? Lastnames { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
