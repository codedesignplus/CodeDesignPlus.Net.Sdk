using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Microservice.Domain.Entities;
using CodeDesignPlus.Net.Microservice.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.Domain.DomainEvents;

public class OrderCreatedDomainEvent(
   Guid aggregateId,
   string orderStatus,
   ClientEntity client,
   AddressValueObject address,
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
    public AddressValueObject Address { get; } = address;
    public long CreatedAt { get; } = createdAt;
    public Guid Tenant { get; private set; } = tenant;
    public Guid CreateBy { get; private set; } = createBy;

    public static OrderCreatedDomainEvent Create(Guid id, ClientEntity client, AddressValueObject address, Guid tenant, Guid creaateBy)
    {
        return new OrderCreatedDomainEvent(id, "Created", client, address, tenant, creaateBy, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }
}

