// See https://aka.ms/new-console-template for more information
using CodeDesignPlus.Net.Event.Sourcing.Abstractions;
using CodeDesignPlus.Net.Event.Sourcing.Extensions;
using CodeDesignPlus.Net.Event.Sourcing.Sample.Aggregates;
using CodeDesignPlus.Net.Event.Sourcing.Sample.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("This is a sample of how to use the library CodeDesignPlus.Net.Event.Sourcing");

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var servicesCollection = new ServiceCollection();

servicesCollection.AddEventSourcing(configuration);

servicesCollection.AddSingleton<IEventSourcing, InMemoryEventSourcingService>();

var serviceProvider = servicesCollection.BuildServiceProvider();

var eventSourcing = serviceProvider.GetRequiredService<IEventSourcing>();

var orderAggregate = OrderAggregate.Create(Guid.NewGuid(), "Order 1", Guid.NewGuid());

orderAggregate.UpdateName("Order 1 Updated");
orderAggregate.AddProduct("Product 1");
orderAggregate.AddProduct("Product 2");
orderAggregate.AddProduct("Product 3");
orderAggregate.AddProduct("Product 4");
orderAggregate.AddProduct("Product 5");

var events = orderAggregate.GetAndClearEvents();

var orderRehidrated = OrderAggregate.Rehydrate<OrderAggregate>(Guid.NewGuid(), events);

Console.WriteLine($"Order Id: {orderRehidrated.Id}");