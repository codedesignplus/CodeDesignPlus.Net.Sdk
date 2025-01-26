using System;
using NodaTime;

namespace CodeDesignPlus.Net.Core.Test.Helpers.Domain;


[EventKey<ProductAggregate>(1, "set-tenant-context", "ms-client")]
public class ClientWithTenantDomainEvent
(
    Guid id,
    string name,
    Guid tenant,
    Guid? eventId = null,
    Instant? occurredAt = null
) : DomainEvent(id, eventId, occurredAt), ITenant
{

    public string Name { get; private set; } = name;

    public Guid Tenant { get; private set; } = tenant;
}