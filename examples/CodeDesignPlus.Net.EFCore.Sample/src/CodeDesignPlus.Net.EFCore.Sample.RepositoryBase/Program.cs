using CodeDesignPlus.Net.EFCore.Extensions;
using CodeDesignPlus.Net.EFCore.Sample.RepositoryBase;
using CodeDesignPlus.Net.EFCore.Sample.RepositoryBase.Entities;
using CodeDesignPlus.Net.EFCore.Sample.RepositoryBase.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var configuration = configurationBuilder.Build();

var serviceCollection = new ServiceCollection();

serviceCollection.AddSingleton(configuration);
serviceCollection.AddEFCore<OrderContext>(configuration);

serviceCollection.AddDbContext<OrderContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), x =>
    {
        x.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    });
});

var serviceProvider = serviceCollection.BuildServiceProvider();

var context = serviceProvider.GetRequiredService<OrderContext>();

await context.Database.EnsureDeletedAsync();

await context.Database.EnsureCreatedAsync();

var orderRepository = serviceProvider.GetRequiredService<IOrderRepository>();

// Create
var order = OrderAggregate.Create(Guid.NewGuid(), "Order 1", "Description 1", 1000, Guid.NewGuid(), Guid.NewGuid());

await orderRepository.CreateAsync(order);

// Read
var orders = await orderRepository.GetEntity<OrderAggregate>().ToListAsync();

foreach (var item in orders)
{
    Console.WriteLine($"Id: {item.Id}");
    Console.WriteLine($"Name: {item.Name}");
    Console.WriteLine($"Description: {item.Description}");
    Console.WriteLine($"Price: {item.Price}");
    Console.WriteLine($"Created At: {item.CreatedAt}");
    Console.WriteLine($"Created By: {item.CreatedBy}");
    Console.WriteLine($"Is Active: {item.IsActive}");
    Console.WriteLine($"Tenant: {item.Tenant}");
    Console.WriteLine();
}

// Update

order = orderRepository.GetEntity<OrderAggregate>().FirstOrDefault(x => x.Id == order.Id);

if (order is not null)
{
    order.Update("Order 1 Updated", "Description 1 Updated", 2000, Guid.NewGuid());

    await orderRepository.UpdateAsync(order);
}


// Delete
await orderRepository.DeleteAsync<OrderAggregate>(x => x.Id == order.Id);

// CreateRangeAsync
var ordersList = new List<OrderAggregate>
{
    OrderAggregate.Create(Guid.NewGuid(), "Order 2", "Description 2", 2000, Guid.NewGuid(), Guid.NewGuid()),
    OrderAggregate.Create(Guid.NewGuid(), "Order 3", "Description 3", 3000, Guid.NewGuid(), Guid.NewGuid()),
    OrderAggregate.Create(Guid.NewGuid(), "Order 4", "Description 4", 4000, Guid.NewGuid(), Guid.NewGuid()),
    OrderAggregate.Create(Guid.NewGuid(), "Order 5", "Description 5", 5000, Guid.NewGuid(), Guid.NewGuid()),
};

await orderRepository.CreateRangeAsync(ordersList);

// UpdateRangeAsync
ordersList = [.. orderRepository.GetEntity<OrderAggregate>().Where(x => x.Price > 3000)];

foreach (var item in ordersList)
{
    item.Update(item.Name, item.Description, item.Price + 1000, Guid.NewGuid());
}

await orderRepository.UpdateRangeAsync(ordersList);

// DeleteRangeAsync
ordersList = [.. orderRepository.GetEntity<OrderAggregate>().Where(x => x.Price < 3000)];

await orderRepository.DeleteRangeAsync(ordersList);

// ChangeStateAsync
order = orderRepository.GetEntity<OrderAggregate>().FirstOrDefault();

if (order is not null)
    await orderRepository.ChangeStateAsync<OrderAggregate>(order.Id, false, CancellationToken.None);

// TransactionAsync
await orderRepository.TransactionAsync<bool>(async (context) =>
{
    var order = OrderAggregate.Create(Guid.NewGuid(), "Order 6", "Description 6", 6000, Guid.NewGuid(), Guid.NewGuid());

    await orderRepository.CreateAsync(order);

    order = orderRepository.GetEntity<OrderAggregate>().FirstOrDefault(x => x.Id == order.Id);

    if (order is not null)
    {
        order.Update("Order 6 Updated", "Description 6 Updated", 7000, Guid.NewGuid());

        await orderRepository.UpdateAsync(order);
    }

    order = orderRepository.GetEntity<OrderAggregate>().FirstOrDefault(x => x.Id == order.Id);

    if (order is not null)
        await orderRepository.DeleteAsync<OrderAggregate>(x => x.Id == order.Id);

});

// GetContext

var context2 = orderRepository.GetContext<OrderContext>();


Console.ReadLine();