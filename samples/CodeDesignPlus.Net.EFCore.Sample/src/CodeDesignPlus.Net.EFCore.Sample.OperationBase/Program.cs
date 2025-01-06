using CodeDesignPlus.Net.EFCore.Extensions;
using CodeDesignPlus.Net.EFCore.Sample.OperationBase;
using CodeDesignPlus.Net.EFCore.Sample.OperationBase.Entities;
using CodeDesignPlus.Net.EFCore.Sample.OperationBase.Repositories;
using CodeDesignPlus.Net.EFCore.Sample.OperationBase.Services;
using CodeDesignPlus.Net.Security.Abstractions;
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
serviceCollection.AddSingleton<IUserContext, UserContext>();

serviceCollection.AddDbContext<OrderContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), x =>
    {
        x.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    });
});

var serviceProvider = serviceCollection.BuildServiceProvider();

var context = serviceProvider.GetRequiredService<OrderContext>();
var user = serviceProvider.GetRequiredService<IUserContext>();

await context.Database.EnsureDeletedAsync();

await context.Database.EnsureCreatedAsync();

var orderRepository = serviceProvider.GetRequiredService<IOrderRepository>();

// Create
var order = OrderAggregate.Create(Guid.NewGuid(), "Order 1", "Description 1", 1000, user.IdUser, user.Tenant);

await orderRepository.CreateAsync(order);

// Update
order = orderRepository.GetEntity<OrderAggregate>().FirstOrDefault(x => x.Id == order.Id);

if (order is not null)
{
    order.Update("Order 1 Updated", "Description 1 Updated", 2000, user.IdUser);

    await orderRepository.UpdateAsync(order.Id, order, CancellationToken.None);
}

// Delete
await orderRepository.DeleteAsync(order.Id);


Console.ReadLine();