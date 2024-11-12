// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using CodeDesignPlus.Net.EventStore.PubSub.Extensions;
using CodeDesignPlus.Net.EventStore.PubSub.Sample.Aggregates;
using CodeDesignPlus.Net.PubSub.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("This is a sample of how to use the CodeDesignPlus.Net.EventStore.PubSub package.");

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var servicesCollection = new ServiceCollection();
servicesCollection.AddLogging();
servicesCollection.AddSingleton(configuration);

servicesCollection.AddEventStorePubSub(configuration);

var serviceProvider = servicesCollection.BuildServiceProvider();

var backgroundServices = serviceProvider.GetServices<IHostedService>();

foreach (var backgroundService in backgroundServices)
{
    _ = backgroundService.StartAsync(CancellationToken.None);
}

// Wait for the background services to start
await Task.Delay(1000);

var message = serviceProvider.GetRequiredService<IMessage>();

var orderAggregate = OrderAggregate.Create(Guid.NewGuid(), "Order 1", Guid.NewGuid());

orderAggregate.UpdateName("Order 1 Updated");
orderAggregate.AddProduct("Product 1");
orderAggregate.AddProduct("Product 2");
orderAggregate.AddProduct("Product 3");
orderAggregate.AddProduct("Product 4");
orderAggregate.AddProduct("Product 5");

foreach (var @event in orderAggregate.GetAndClearEvents())
{
    _ = message.PublishAsync(@event, CancellationToken.None);

    Console.WriteLine($"Event {@event.GetType().Name} published, id: {@event.EventId}, aggregate id: {@event.AggregateId}");
}

// Wait for the background services to finish
await Task.Delay(1000);

Console.ReadLine();