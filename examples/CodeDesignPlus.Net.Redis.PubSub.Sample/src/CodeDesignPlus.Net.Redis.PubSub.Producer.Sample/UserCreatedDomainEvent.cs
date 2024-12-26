using System;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.Redis.PubSub.Producer.Sample;

[EventKey<UserEntity>(1, "created")]
public class UserCreatedDomainEvent(
    Guid aggregateId,
    string name,
    string email,
    string? password = null,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; } = name;
    public string Email { get; } = email;
    public string? Password { get; set; } = password;

    public static UserCreatedDomainEvent Create(Guid aggregateId, string name, string email, string? password = null)
    {
        return new UserCreatedDomainEvent(aggregateId, name, email, password);
    }
}
