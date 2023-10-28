namespace CodeDesignPlus.Net.EventStore.Test.Helpers.Domain;


public class OrderProduct
{
    public required Product Product { get; set; }
    public int Quantity { get; set; }
}

public class Client
{
    public Guid Id { get; set; }
    public string Name { get; private set; }
    // Otras propiedades, como dirección, teléfono, etc.

    // Constructor
    public Client(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    // Métodos relacionados con el cliente (como actualizar información, etc.)
}

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    // Constructor para cuando se crea un nuevo producto
    public Product(string name, decimal price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
    }

    // Métodos relacionados con el producto (como actualizar precio, nombre, etc.)
}