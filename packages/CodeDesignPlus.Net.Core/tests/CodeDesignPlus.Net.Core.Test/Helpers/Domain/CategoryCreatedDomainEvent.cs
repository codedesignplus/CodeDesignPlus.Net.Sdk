using System;

namespace CodeDesignPlus.Net.Core.Test.Helpers.Domain;


[EventKey<ProductAggregate>(1, "register-category", "ms-category")]
public class CategoryCreatedDomainEvent
(
    Guid id,
    string name,
    Guid? eventId = null,
    DateTime? occurredAt = null
) : DomainEvent(id, eventId, occurredAt)
{

    public string Name { get; private set; } = name;
}