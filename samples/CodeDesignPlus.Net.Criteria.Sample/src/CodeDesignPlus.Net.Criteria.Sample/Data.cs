using CodeDesignPlus.Net.Criteria.Sample.Models;

namespace CodeDesignPlus.Net.Criteria.Sample;

public class OrdersData
{
    public static List<Order> GetOrders()
    {
        return
        [
            new() {
                Id = 1,
                Name = "Order 1",
                Description = "Description Order 1",
                CreatedAt = new DateTime(2021, 1, 1),
                Total = 90,
                Products =
                [
                    new Product
                    {
                        Id = 1,
                        Name = "Product 1.1",
                        Description = "Description Product 1",
                        Price = 50,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "Product 1.2",
                        Description = "Description Product 2",
                        Price = 40,
                        CreatedAt = DateTime.Now
                    }
                ],
                Client = new Client
                {
                    Id = 1,
                    Name = "Client 1.1",
                    LastName = "Last Name Client 1.1",
                    Email = "client11@outlook.com",
                    Phone = "1234567890"
                }
            },
            new() {
                Id = 2,
                Name = "Order 2",
                Description = "Description Order 2",
                CreatedAt = DateTime.Now,
                Total = 200,
                Products =
                [
                    new Product
                    {
                        Id = 3,
                        Name = "Product 2.1",
                        Description = "Description Product 1",
                        Price = 50,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Id = 4,
                        Name = "Product 2.2",
                        Description = "Description Product 2",
                        Price = 150,
                        CreatedAt = DateTime.Now
                    }
                ],
                Client = new Client
                {
                    Id = 2,
                    Name = "Client 2.1",
                    LastName = "Last Name Client 2.1",
                    Email = "client21@outlook.com",
                    Phone = "1234567890"
                }
            },
            new() {
                Id = 3,
                Name = "Order 3",
                Description = "Description Order 3",
                CreatedAt = DateTime.Now,
                Total = 300,
                Products =
                [
                    new Product
                    {
                        Id = 5,
                        Name = "Product 3.1",
                        Description = "Description Product 1",
                        Price = 100,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Id = 6,
                        Name = "Product 3.2",
                        Description = "Description Product 2",
                        Price = 200,
                        CreatedAt = DateTime.Now
                    }
                ],
                Client = new Client
                {
                    Id = 3,
                    Name = "Client 3.1",
                    LastName = "Last Name Client 3.1",
                    Email = "client21@outlook.com",
                    Phone = "1234567890"
                }
            }
        ];
    }
}
