using CodeDesignPlus.Net.Mongo.Test.Helpers.Models;

namespace CodeDesignPlus.Net.Mongo.Test;

public class OrderData
{
    private static Client Client1 = new()
    {
        Id = Guid.NewGuid(),
        Name = "Client 1",
        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
    };

    private static Client Client2 = new()
    {
        Id = Guid.NewGuid(),
        Name = "Client 2",
        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
    };

    public static List<Order> GetOrders()
    {
        return [
            new Order
            {
                Id = Guid.NewGuid(),
                Name = "Order 1",
                Description = "Description Order 1",
                Total = 1000,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Client = Client1,
                Products =
                [
                    new() {
                        Id = Guid.NewGuid(),
                        Name = "Product 1",
                        Description = "Description Product 1",
                        Price = 100,
                        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        Name = "Product 2",
                        Description = "Description Product 2",
                        Price = 200,
                        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    }
                ]
            },
            new Order
            {
                Id = Guid.NewGuid(),
                Name = "Order 2",
                Description = "Description Order 2",
                Total = 2000,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Client = Client1,
                Products =
                [
                    new() {
                        Id = Guid.NewGuid(),
                        Name = "Product 3",
                        Description = "Description Product 3",
                        Price = 300,
                        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        Name = "Product 4",
                        Description = "Description Product 4",
                        Price = 400,
                        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    }
                ]
            },
            new Order
            {
                Id = Guid.NewGuid(),
                Name = "Order 3",
                Description = "Description Order 3",
                Total = 3000,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Client = Client2,
                Products =
                [
                    new() {
                        Id = Guid.NewGuid(),
                        Name = "Product 5",
                        Description = "Description Product 5",
                        Price = 500,
                        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        Name = "Product 6",
                        Description = "Description Product 6",
                        Price = 600,
                        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    }
                ]
            },
            new Order
            {
                Id = Guid.NewGuid(),
                Name = "Order 4",
                Description = "Description Order 4",
                Total = 4000,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Client = Client2,
                Products =
                [
                    new() {
                        Id = Guid.NewGuid(),
                        Name = "Product 7",
                        Description = "Description Product 7",
                        Price = 700,
                        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        Name = "Product 8",
                        Description = "Description Product 8",
                        Price = 800,
                        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    }
                ]
            }
        ];
    }
}
