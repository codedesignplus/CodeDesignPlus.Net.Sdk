using Newtonsoft.Json;
using CodeDesignPlus.Net.Event.Sourcing.Abstractions;
using CodeDesignPlus.Net.EventStore.Test.Helpers.Domain;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers.Events;


public class ProductAddedToOrderEvent : DomainEventBase
{
    public int Quantity { get; }
    public Product Product { get; set; }

    public ProductAddedToOrderEvent(Guid aggregateId, int quantity, Product product)
        : base(aggregateId)
    {
        this.Product = product;
        this.Quantity = quantity;
    }
}

public class ProductRemovedFromOrderEvent : DomainEventBase
{
    public Guid ProductId { get; }

    public ProductRemovedFromOrderEvent(Guid aggregateId, Guid productId)
        : base(aggregateId)
    {
        this.ProductId = productId;
    }
}

public class ProductQuantityUpdatedEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public int NewQuantity { get; }

    public ProductQuantityUpdatedEvent(Guid aggregateId, Guid productId, int newQuantity)
        : base(aggregateId)
    {
        this.ProductId = productId;
        this.NewQuantity = newQuantity;
    }
}