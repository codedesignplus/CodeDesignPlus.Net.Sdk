using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.Core.Test;

[Key("order.deleted")]
public class OrderDeletedDomainEvent(
    Guid id,
    long? deletedAt,
    Guid? eventId = null,
    DateTime? occurredAt = null
) : DomainEvent(id, eventId, occurredAt)
{
    public long? DeletedAt { get; private set; } = deletedAt;
}
