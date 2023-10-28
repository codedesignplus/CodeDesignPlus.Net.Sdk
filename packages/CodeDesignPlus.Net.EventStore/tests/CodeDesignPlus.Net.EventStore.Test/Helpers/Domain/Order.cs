using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.EventStore.Test.Helpers.Events;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers.Domain;

public enum OrderStatus
{
    Pending,
    Completed,
    Cancelled
}

public class OrderAggregateRoot : AggregateRootBase<Guid, Guid>
{
    // Propiedades básicas de la orden
    public DateTime Date { get; private set; }
    public Client Client { get; private set; }
    public List<OrderProduct> Products { get; private set; } = new List<OrderProduct>();
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;


    // Constructor para cuando se crea una nueva orden
    public OrderAggregateRoot(DateTime date, Client client, Guid userIdCreator)
    {
        Id = Guid.NewGuid();
        Date = date;
        Client = client;
        IdUserCreator = userIdCreator;
        DateCreated = DateTime.UtcNow;
        IsActive = true;
        base.Version = -1;

        ApplyEvent(new OrderCreatedEvent(Id, date, Client.Id, new OrderMetadata { OrderChannel = "Web" }));
    }

    // Método para agregar un producto a la orden
    public void AddProduct(Product product, int quantity)
    {
        var orderProduct = new OrderProduct { Product = product, Quantity = quantity };
        Products.Add(orderProduct);

        ApplyEvent(new ProductAddedToOrderEvent(Id, product.Id, product.Name, product.Price, quantity, new ProductMetadata()
        {
            Supplier = "Alkosto"
        }));
    }

    // Método para remover un producto de la orden
    public void RemoveProduct(Guid productId)
    {
        var product = Products.SingleOrDefault(p => p.Product.Id == productId);
        if (product == null)
            throw new InvalidOperationException("Producto no encontrado en la orden.");

        Products.Remove(product);
        ApplyEvent(new ProductRemovedFromOrderEvent(Id, productId, new ProductMetadata()
        {
            Supplier = "Alkosto"
        }));
    }

    // Método para actualizar la cantidad de un producto en la orden
    public void UpdateProductQuantity(Guid productId, int newQuantity)
    {
        var product = Products.SingleOrDefault(p => p.Product.Id == productId);
        if (product == null)
            throw new InvalidOperationException("Producto no encontrado en la orden.");

        product.Quantity = newQuantity;
        ApplyEvent(new ProductQuantityUpdatedEvent(Id, productId, newQuantity, new ProductMetadata()
        {
            Supplier = "Alkosto"
        }));
    }

    // Método para actualizar la fecha de una orden
    public void UpdateDate(DateTime newDate)
    {
        Date = newDate;
        ApplyEvent(new OrderUpdatedEvent(Id, DateTime.UtcNow, new OrderMetadata()
        {
            OrderChannel = "Web"
        }));
    }

    // Método para completar una orden
    public void CompleteOrder()
    {
        if (Status == OrderStatus.Completed)
            throw new InvalidOperationException("La orden ya está completada.");

        Status = OrderStatus.Completed;
        ApplyEvent(new OrderCompletedEvent(Id, DateTime.UtcNow, new OrderMetadata()
        {
            OrderChannel = "Web"
        }));
    }

    // Método para cancelar una orden
    public void CancelOrder(string reason)
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("La orden ya está cancelada.");

        Status = OrderStatus.Cancelled;
        ApplyEvent(new OrderCancelledEvent(Id, DateTime.UtcNow, reason));
    }

    // Método para actualizar el cliente asociado a la orden
    public void AssignClient(Client client)
    {
        Client.Id = client.Id;
        ApplyEvent(new ClientAssignedToOrderEvent(Id, client.Id, client.Name, new ClientMetadata()
        {
            ClientSegment = "Regular"
        }));
    }

    // Método para desvincular un cliente de la orden
    public void UnassignClient()
    {
        var previousClientId = Client.Id;
        Client = default!;
        ApplyEvent(new ClientUnassignedFromOrderEvent(Id, previousClientId, new ClientMetadata()
        {
            ClientSegment = "Regular"
        }));
    }

}
