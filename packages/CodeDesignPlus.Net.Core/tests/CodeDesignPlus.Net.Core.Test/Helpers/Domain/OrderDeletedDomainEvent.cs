namespace CodeDesignPlus.Net.Core.Test.Helpers.Domain;

[EventKey("order.deleted")]
public class OrderDeletedDomainEvent(
    Guid id,
    long? deletedAt,
    Guid? eventId = null,
    DateTime? occurredAt = null
) : DomainEvent(id, eventId, occurredAt)
{
    public long? DeletedAt { get; private set; } = deletedAt;
}
