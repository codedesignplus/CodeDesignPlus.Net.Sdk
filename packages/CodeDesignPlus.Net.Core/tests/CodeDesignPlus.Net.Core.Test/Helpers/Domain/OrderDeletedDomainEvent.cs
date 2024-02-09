using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.Core.Test;

[Key("order.deleted")]
public class OrderDeletedDomainEvent(
    Guid id,
    DateTime deletedAt,
    Guid? eventId = null,
    DateTime? occurredAt = null
) : DomainEvent(id, eventId, occurredAt)
{
    public DateTime DeletedAt { get; private set; } = deletedAt;
}
