﻿using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.Core.Sample.Resources.Aggregate;

namespace CodeDesignPlus.Net.Core.Sample.Resources.DomainEvents;

[EventKey<OrderAggregate>(1, "updated")]
public class OrderUpdatedDomainEvent(
    Guid id,
    string name,
    string description,
    decimal price,
    long? updatedAt,
    Guid UpdatedBy,
    Guid? eventId = null,
    DateTime? occurredAt = null
) : DomainEvent(id, eventId, occurredAt)
{
    private readonly Guid updatedBy = UpdatedBy;

    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public decimal Price { get; private set; } = price;
    public long? UpdatedAt { get; private set; } = updatedAt;
}