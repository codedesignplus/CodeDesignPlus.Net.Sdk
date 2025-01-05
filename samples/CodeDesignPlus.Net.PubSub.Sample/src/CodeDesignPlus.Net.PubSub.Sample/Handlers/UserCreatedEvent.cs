using System;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.PubSub.Sample.Entities;

namespace CodeDesignPlus.Net.PubSub.Sample.Handlers;

[EventKey<UserEntity>(1, "created")]
public class UserCreatedEvent(Guid aggregateId, string name, Guid? eventId = null, DateTime? occurredAt = null, Dictionary<string, object>? metadata = null) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; set; } = name;

    public static UserCreatedEvent Create(Guid aggregateId, string name, Guid? eventId = null, DateTime? occurredAt = null, Dictionary<string, object>? metadata = null)
    {
        return new UserCreatedEvent(aggregateId, name, eventId, occurredAt, metadata);
    }
}
