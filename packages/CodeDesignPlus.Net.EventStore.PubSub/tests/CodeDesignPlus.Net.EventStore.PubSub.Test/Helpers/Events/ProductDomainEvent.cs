using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Domain;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Events;

[EventKey<Product>(1, "added")]
public class ProductAddedToOrderEvent(
    Guid aggregateId,
    int quantity,
    Product product,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public int Quantity { get; } = quantity;
    public Product Product { get; set; } = product;
}

[EventKey<Product>(1, "removed")]
public class ProductRemovedFromOrderEvent(
    Guid aggregateId,
    Guid productId,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public Guid ProductId { get; } = productId;
}

[EventKey<Product>(1, "quantity_updated")]
public class ProductQuantityUpdatedEvent(
    Guid aggregateId,
    Guid productId,
    int newQuantity,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public Guid ProductId { get; } = productId;
    public int NewQuantity { get; } = newQuantity;
}