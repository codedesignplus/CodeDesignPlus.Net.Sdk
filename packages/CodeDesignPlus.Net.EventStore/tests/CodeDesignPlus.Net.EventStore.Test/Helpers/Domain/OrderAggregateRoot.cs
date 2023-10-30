using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.EventStore.Test.Helpers.Events;
using StackExchange.Redis;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers.Domain;

public enum OrderStatus
{
    Pending,
    Completed,
    Cancelled
}

public class OrderAggregateRoot : AggregateRootBase<Guid>
{
    public DateTime CompletionDate { get; private set; }
    public DateTime CancellationDate { get; private set; }
    public Client? Client { get; private set; }
    public List<OrderProduct> Products { get; private set; } = new List<OrderProduct>();
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public Guid IdUserCreator { get; private set; }
    public DateTime DateCreated { get; private set; }

    private OrderAggregateRoot() { }

    public OrderAggregateRoot(Guid idUserCreator, Guid idUser, Client client)
    {
        Id = Guid.NewGuid();

        this.CreateOrder(idUserCreator, idUser, client);
    }

    public OrderAggregateRoot(Guid id, Guid idUserCreator, Guid idUser, Client client)
    {
        Id = id;

        this.CreateOrder(idUserCreator, idUser, client);
    }

    private void CreateOrder(Guid idUserCreator, Guid idUser, Client client)
    {
        var orderCreatedEvent = new OrderCreatedEvent(
            Id,
            idUserCreator,
            OrderStatus.Pending,
            client,
            DateTime.UtcNow
       );

        ApplyChange(orderCreatedEvent, idUser);
    }

    // Método para agregar un producto a la orden
    public void AddProduct(Product product, int quantity, Guid idUser)
    {
        var productAddedToOrderEvent = new ProductAddedToOrderEvent(
            Id,
            quantity,
            product
        );

        ApplyChange(productAddedToOrderEvent, idUser);
    }

    // Método para remover un producto de la orden
    public void RemoveProduct(Guid productId, Guid idUser)
    {
        var product = Products.SingleOrDefault(p => p.Product.Id == productId);

        if (product == null)
            throw new InvalidOperationException("Producto no encontrado en la orden.");

        var productRemovedFromOrderEvent = new ProductRemovedFromOrderEvent(
            Id,
            productId
        );

        ApplyChange(productRemovedFromOrderEvent, idUser);
    }

    // Método para actualizar la cantidad de un producto en la orden
    public void UpdateProductQuantity(Guid productId, int newQuantity, Guid idUser)
    {
        var product = Products.SingleOrDefault(p => p.Product.Id == productId);

        if (product == null)
            throw new InvalidOperationException("Producto no encontrado en la orden.");

        var productQuantityUpdatedEvent = new ProductQuantityUpdatedEvent(
            Id,
            productId,
            newQuantity
        );

        ApplyChange(productQuantityUpdatedEvent, idUser);
    }

    // Método para completar una orden
    public void CompleteOrder(Guid idUser)
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("La orden ya está completada.");

        var orderCompletedEvent = new OrderCompletedEvent(
            Id,
            DateTime.UtcNow
        );

        ApplyChange(orderCompletedEvent, idUser);
    }

    // Método para cancelar una orden
    public void CancelOrder(string reason, Guid idUser)
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("La orden ya está cancelada.");

        var orderCancelledEvent = new OrderCancelledEvent(
            Id,
            DateTime.UtcNow,
            reason
        );

        ApplyChange(orderCancelledEvent, idUser);
    }

    private void Apply(OrderCreatedEvent @event, Metadata<Guid> metadata)
    {
        Id = @event.AggregateId;
        Client = @event.Client;
        Status = @event.OrderStatus;
        IdUserCreator = @event.IdUserCreator;
        DateCreated = @event.DateCreated;
    }

    private void Apply(OrderCompletedEvent @event, Metadata<Guid> metadata)
    {
        Status = OrderStatus.Completed;
        CompletionDate = @event.CompletionDate;
    }

    private void Apply(OrderCancelledEvent @event, Metadata<Guid> metadata)
    {
        Status = OrderStatus.Cancelled;
        CancellationDate = @event.CancellationDate;
    }

    private void Apply(ProductAddedToOrderEvent @event, Metadata<Guid> metadata)
    {
        Products.Add(new OrderProduct { Product = @event.Product, Quantity = @event.Quantity });
    }

    private void Apply(ProductRemovedFromOrderEvent @event, Metadata<Guid> metadata)
    {
        var productToRemove = Products.SingleOrDefault(p => p.Product.Id == @event.ProductId);

        if (productToRemove != null)
            Products.Remove(productToRemove);
    }

    private void Apply(ProductQuantityUpdatedEvent @event, Metadata<Guid> metadata)
    {
        var productToUpdate = Products.SingleOrDefault(p => p.Product.Id == @event.ProductId);
        if (productToUpdate != null)
        {
            productToUpdate.Quantity = @event.NewQuantity;
        }
    }
}
