﻿using CodeDesignPlus.Net.EventStore.Test.Helpers.Domain;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers.Events;

[Key("product.added")]
public class ProductAddedToOrderEvent(
    Guid aggregateId,
    int quantity,
    Product product,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public int Quantity { get; } = quantity;
    public Product Product { get; set; } = product;
}

[Key("product.removed")]
public class ProductRemovedFromOrderEvent(
    Guid aggregateId,
    Guid productId,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public Guid ProductId { get; } = productId;
}

[Key("product.quantity.updated")]
public class ProductQuantityUpdatedEvent(
    Guid aggregateId, 
    Guid productId, 
    int newQuantity,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public Guid ProductId { get; } = productId;
    public int NewQuantity { get; } = newQuantity;
}