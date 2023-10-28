using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers.Events;


public class ProductAddedToOrderEvent : ThinDomainEventBase<ProductMetadata>
{
    public Guid ProductId { get; }
    public string ProductName { get; }
    public decimal ProductPrice { get; }
    public int Quantity { get; }

    public ProductAddedToOrderEvent(Guid orderId, Guid productId, string productName, decimal productPrice, int quantity, ProductMetadata metadata)
        : base(orderId, metadata)
    {
        ProductId = productId;
        ProductName = productName;
        ProductPrice = productPrice;
        Quantity = quantity;
    }
}

public class ProductRemovedFromOrderEvent : ThinDomainEventBase<ProductMetadata>
{
    public Guid ProductId { get; }

    public ProductRemovedFromOrderEvent(Guid orderId, Guid productId, ProductMetadata metadata)
        : base(orderId, metadata)
    {
        ProductId = productId;
    }
}

public class ProductQuantityUpdatedEvent : ThinDomainEventBase<ProductMetadata>
{
    public Guid ProductId { get; }
    public int NewQuantity { get; }

    public ProductQuantityUpdatedEvent(Guid orderId, Guid productId, int newQuantity, ProductMetadata metadata)
        : base(orderId, metadata)
    {
        ProductId = productId;
        NewQuantity = newQuantity;
    }
}