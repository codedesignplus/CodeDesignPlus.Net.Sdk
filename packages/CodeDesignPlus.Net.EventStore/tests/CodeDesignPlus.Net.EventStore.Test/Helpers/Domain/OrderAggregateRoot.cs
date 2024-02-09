using System.Reflection;
using System.Text.Json.Serialization;
using CodeDesignPlus.Net.Event.Sourcing.Abstractions;
using CodeDesignPlus.Net.EventStore.Test.Helpers.Events;
using StackExchange.Redis;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers.Domain;

public enum OrderStatus
{
    Pending,
    Completed,
    Cancelled
}

public class OrderAggregateRoot : AggregateRoot
{
    public override string Category { get; protected set; } = "Order";

    public DateTime CompletionDate { get; private set; }
    public DateTime CancellationDate { get; private set; }
    public Client? Client { get; private set; }
    public List<OrderProduct> Products { get; private set; } = [];
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public Guid IdUserCreator { get; private set; }
    public DateTime DateCreated { get; private set; }

    public OrderAggregateRoot(Guid id) : base(id) { }

    public static OrderAggregateRoot Create(Guid id, Guid idUserCreator, Client client)
    {
        var aggregate = new OrderAggregateRoot(id);

        aggregate.AddEvent(new OrderCreatedEvent(id, idUserCreator, OrderStatus.Pending, client, DateTime.UtcNow));

        return aggregate;
    }

    public void AddProduct(Product product, int quantity)
    {
        base.AddEvent(new ProductAddedToOrderEvent(
            Id,
            quantity,
            product
        ));
    }

    public void RemoveProduct(Guid productId)
    {
        var product = Products.SingleOrDefault(p => p.Product.Id == productId);

        if (product == null)
            throw new InvalidOperationException("Producto no encontrado en la orden.");

        base.AddEvent(new ProductRemovedFromOrderEvent(
            Id,
            productId
        ));
    }

    public void UpdateProductQuantity(Guid productId, int newQuantity)
    {
        var product = Products.SingleOrDefault(p => p.Product.Id == productId);

        if (product == null)
            throw new InvalidOperationException("Producto no encontrado en la orden.");

        base.AddEvent(new ProductQuantityUpdatedEvent(
            Id,
            productId,
            newQuantity
        ));
    }

    public void CompleteOrder()
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("La orden ya está completada.");

        base.AddEvent(new OrderCompletedEvent(
            Id,
            DateTime.UtcNow
        ));
    }

    // Método para cancelar una orden
    public void CancelOrder(string reason)
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("La orden ya está cancelada.");

        base.AddEvent(new OrderCancelledEvent(
            Id,
            DateTime.UtcNow,
            reason
        ));
    }

    private void Apply(OrderCreatedEvent @event)
    {
        Client = @event.Client;
        Status = @event.OrderStatus;
        IdUserCreator = @event.IdUserCreator;
        DateCreated = @event.DateCreated;
    }

    private void Apply(OrderCompletedEvent @event)
    {
        Status = OrderStatus.Completed;
        CompletionDate = @event.CompletionDate;
    }

    private void Apply(OrderCancelledEvent @event)
    {
        Status = OrderStatus.Cancelled;
        CancellationDate = @event.CancellationDate;
    }

    private void Apply(ProductAddedToOrderEvent @event)
    {
        Products.Add(new OrderProduct { Product = @event.Product, Quantity = @event.Quantity });
    }

    private void Apply(ProductRemovedFromOrderEvent @event)
    {
        var productToRemove = Products.SingleOrDefault(p => p.Product.Id == @event.ProductId);

        if (productToRemove != null)
            Products.Remove(productToRemove);
    }

    private void Apply(ProductQuantityUpdatedEvent @event)
    {
        var productToUpdate = Products.SingleOrDefault(p => p.Product.Id == @event.ProductId);
        if (productToUpdate != null)
        {
            productToUpdate.Quantity = @event.NewQuantity;
        }
    }
}
