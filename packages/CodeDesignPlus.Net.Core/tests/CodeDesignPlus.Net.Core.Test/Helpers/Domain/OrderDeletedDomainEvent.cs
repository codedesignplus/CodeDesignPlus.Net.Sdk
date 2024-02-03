namespace CodeDesignPlus.Net.Core.Test;

public class OrderDeletedDomainEvent(
    Guid id,
    DateTime deletedAt,
    Guid? eventId = null,
    DateTime? occurredAt = null
) : DomainEvent(id, eventId, occurredAt)
{
    public DateTime DeletedAt { get; private set; } = deletedAt;

    public override string GetEventType()
    {
        return "OrderDeleted";
    }
}
