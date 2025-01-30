using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Microservice.Domain.Entities;
using CodeDesignPlus.Net.Microservice.Domain.ValueObjects;
using NodaTime;

namespace CodeDesignPlus.Net.Microservice.Domain.DomainEvents;

public class OrderCreatedDomainEvent(
   Guid aggregateId,
   string orderStatus,
   ClientEntity client,
   AddressValueObject address,
   Guid tenant,
   Guid createBy,
   Instant createdAt,
   Guid? eventId = null,
   Instant? occurredAt = null,
   Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string OrderStatus { get; } = orderStatus;
    public ClientEntity Client { get; } = client;
    public AddressValueObject Address { get; } = address;
    public Instant CreatedAt { get; } = createdAt;
    public Guid Tenant { get; private set; } = tenant;
    public Guid CreateBy { get; private set; } = createBy;

    public static OrderCreatedDomainEvent Create(Guid id, ClientEntity client, AddressValueObject address, Guid tenant, Guid creaateBy)
    {
        return new OrderCreatedDomainEvent(id, "Created", client, address, tenant, creaateBy, SystemClock.Instance.GetCurrentInstant());
    }
}

