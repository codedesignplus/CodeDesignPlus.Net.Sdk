using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using NodaTime;

namespace CodeDesignPlus.Net.ServiceBus.Producer.Sample;

[EventKey<UserEntity>(1, "created")]
public class UserCreatedDomainEvent(Guid aggregateId, string? name, string? email, Guid? eventId = null, Instant? occurredAt = null, Dictionary<string, object>? metadata = null)
    : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string? Name { get; set; } = name;
    public string? Email { get; set; } = email;
}
