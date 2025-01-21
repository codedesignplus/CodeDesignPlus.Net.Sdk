using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.Core.Sample.Resources.Aggregate;

namespace CodeDesignPlus.Net.Core.Sample.Resources.DomainEvents;

[EventKey<OrderAggregate>(1, "deleted")]
public class OrderDeletedDomainEvent(
    Guid id,
    long? deletedAt,
    Guid? eventId = null,
    Instant? occurredAt = null
) : DomainEvent(id, eventId, occurredAt)
{
    public long? DeletedAt { get; private set; } = deletedAt;
}
