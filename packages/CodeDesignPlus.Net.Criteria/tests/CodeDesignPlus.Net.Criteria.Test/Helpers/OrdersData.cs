namespace CodeDesignPlus.Net.Criteria.Test.Helpers;

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
                Code = Guid.Parse("9432915d-8418-4896-86f6-6d2e0d0160af"),
                CreatedAt = new LocalDate(2021, 1, 1).AtMidnight().InUtc().ToInstant(),
                Total = 90,
                Products =
                [
                    new Product
                    {
                        Id = 1,
                        Name = "Product 1.1",
                        Description = "Description Product 1",
                        Price = 50,
                        CreatedAt = SystemClock.Instance.GetCurrentInstant()
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "Product 1.2",
                        Description = "Description Product 2",
                        Price = 40,
                        CreatedAt = SystemClock.Instance.GetCurrentInstant()
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
                Code = Guid.Parse("f37fdc53-858b-4b6b-821a-406eee568a9c"),
                CreatedAt = SystemClock.Instance.GetCurrentInstant(),
                Total = 200,
                Products =
                [
                    new Product
                    {
                        Id = 3,
                        Name = "Product 2.1",
                        Description = "Description Product 1",
                        Price = 50,
                        CreatedAt = SystemClock.Instance.GetCurrentInstant()
                    },
                    new Product
                    {
                        Id = 4,
                        Name = "Product 2.2",
                        Description = "Description Product 2",
                        Price = 150,
                        CreatedAt = SystemClock.Instance.GetCurrentInstant()
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
                Code = Guid.Parse("00c6d6c7-12e1-4e92-8663-88e3df24ed21"),
                CreatedAt = SystemClock.Instance.GetCurrentInstant(),
                Total = 300,
                Products =
                [
                    new Product
                    {
                        Id = 5,
                        Name = "Product 3.1",
                        Description = "Description Product 1",
                        Price = 100,
                        CreatedAt = SystemClock.Instance.GetCurrentInstant()
                    },
                    new Product
                    {
                        Id = 6,
                        Name = "Product 3.2",
                        Description = "Description Product 2",
                        Price = 200,
                        CreatedAt = SystemClock.Instance.GetCurrentInstant()
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
