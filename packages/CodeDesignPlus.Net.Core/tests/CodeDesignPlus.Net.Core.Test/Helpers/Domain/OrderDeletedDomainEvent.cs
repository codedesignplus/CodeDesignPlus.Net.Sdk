namespace CodeDesignPlus.Net.Core.Test;

public class OrderDeletedDomainEvent(
    Guid id,
    DateTime deletedAt
) : DomainEvent(Guid.NewGuid(), "OrderDeleted", DateTime.UtcNow, id)
{
    public DateTime DeletedAt { get; private set; } = deletedAt;
}
