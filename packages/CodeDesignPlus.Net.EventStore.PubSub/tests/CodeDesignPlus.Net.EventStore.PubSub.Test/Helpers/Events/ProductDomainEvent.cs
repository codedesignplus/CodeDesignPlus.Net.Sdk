using Newtonsoft.Json;
using CodeDesignPlus.Net.Event.Sourcing.Abstractions;
using CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Domain;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Events;


public class ProductAddedToOrderEvent : DomainEventBase
{
    public int Quantity { get; }
    public Product Product { get; set; }

    public ProductAddedToOrderEvent(Guid idAggregate, int quantity, Product product)
        : base(idAggregate)
    {
        this.Product = product;
        this.Quantity = quantity;
    }
}

public class ProductRemovedFromOrderEvent : DomainEventBase
{
    public Guid ProductId { get; }

    public ProductRemovedFromOrderEvent(Guid idAggregate, Guid productId)
        : base(idAggregate)
    {
        this.ProductId = productId;
    }
}

public class ProductQuantityUpdatedEvent : DomainEventBase
{
    public Guid ProductId { get; }
    public int NewQuantity { get; }

    public ProductQuantityUpdatedEvent(Guid idAggregate, Guid productId, int newQuantity)
        : base(idAggregate)
    {
        this.ProductId = productId;
        this.NewQuantity = newQuantity;
    }
}