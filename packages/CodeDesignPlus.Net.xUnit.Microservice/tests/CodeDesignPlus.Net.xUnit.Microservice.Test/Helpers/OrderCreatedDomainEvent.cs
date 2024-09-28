using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Helpers;

public class OrderCreatedDomainEvent(
   Guid aggregateId,
   string orderStatus,
   ClientEntity client,
   Guid tenant,
   Guid createBy,
   long createdAt,
   Guid? eventId = null,
   DateTime? occurredAt = null,
   Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string OrderStatus { get; } = orderStatus;
    public ClientEntity Client { get; } = client;
    public long CreatedAt { get; } = createdAt;
    public Guid Tenant { get; private set; } = tenant;
    public Guid CreateBy { get; private set; } = createBy;

    public static OrderCreatedDomainEvent Create(Guid id, ClientEntity client, Guid tenant, Guid creaateBy)
    {
        return new OrderCreatedDomainEvent(id, "Created", client, tenant, creaateBy, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }
}

