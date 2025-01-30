using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.Serializers.Test.Helpers.DomainEvents;

[EventKey("UserEntity", 1, "created")]
public class UserCreatedDomainEvent(Guid aggregateId, Guid? eventId = null, Instant? occurredAt = null, Dictionary<string, object>? metadata = null)
    : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
}
