namespace CodeDesignPlus.Net.Core.Test.Helpers.Domain;

[EventKey<OrderAggregate>(1, "deleted")]
public class OrderDeletedDomainEvent(
    Guid id,
    Instant? deletedAt,
    Guid? eventId = null,
    Instant? occurredAt = null
) : DomainEvent(id, eventId, occurredAt)
{
    public Instant? DeletedAt { get; private set; } = deletedAt;
}
