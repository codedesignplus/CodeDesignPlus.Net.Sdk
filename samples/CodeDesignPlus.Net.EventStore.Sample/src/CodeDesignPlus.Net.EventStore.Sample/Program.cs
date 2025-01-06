// See https://aka.ms/new-console-template for more information
using CodeDesignPlus.Net.Event.Sourcing.Abstractions;
using CodeDesignPlus.Net.EventStore.Extensions;
using CodeDesignPlus.Net.EventStore.Sample.Aggregates;
using CodeDesignPlus.Net.EventStore.Sample.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("This is a sample of how to use the CodeDesignPlus.Net.EventStore package.");

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var servicesCollection = new ServiceCollection();
servicesCollection.AddLogging();
servicesCollection.AddSingleton(configuration);

servicesCollection.AddEventStore(configuration);

var serviceProvider = servicesCollection.BuildServiceProvider();

var eventSourcing = serviceProvider.GetRequiredService<IEventSourcing>();

var orderAggregate = OrderAggregate.Create(Guid.NewGuid(), "Order 1", Guid.NewGuid());

orderAggregate.UpdateName("Order 1 Updated");
orderAggregate.AddProduct("Product 1");
orderAggregate.AddProduct("Product 2");
orderAggregate.AddProduct("Product 3");
orderAggregate.AddProduct("Product 4");
orderAggregate.AddProduct("Product 5");

// AppendEventAsync - Append a new event to the event store
foreach (var @event in orderAggregate.GetAndClearEvents())
{
    await eventSourcing.AppendEventAsync("Order", @event);
}

// CountEventAsync - Count the number of events in the event store
var countEvents = await eventSourcing.CountEventsAsync("Order", orderAggregate.Id);

Console.WriteLine($"Count events: {countEvents}");

// GetVersionAsync - Get the version of the aggregate

var version = await eventSourcing.GetVersionAsync("Order", orderAggregate.Id);

Console.WriteLine($"Version Aggregate: {orderAggregate.Version} - Version Event Store: {version}");

// LoadEventsAsync - Load all events of the aggregate

var events = await eventSourcing.LoadEventsAsync("Order", orderAggregate.Id);

var orderRehydrate = AggregateRoot.Rehydrate<OrderAggregate>(orderAggregate.Id, events);

Console.WriteLine($"Order rehydrate: {orderRehydrate.Name}");

// SaveSnapshotAsync - Save a snapshot of the aggregate

await eventSourcing.SaveSnapshotAsync(orderAggregate);

// LoadSnapshotAsync - Load the snapshot of the aggregate

var orderSnapshot = await eventSourcing.LoadSnapshotAsync<OrderAggregate>("Order", orderAggregate.Id);

Console.WriteLine($"Order snapshot: {orderSnapshot.Name}");

// SearchEventsAsync - Search all events of the event store

var allEvents = await eventSourcing.SearchEventsAsync($"Order-{orderAggregate.Id}");

foreach (var @event in allEvents)
{
    Console.WriteLine($"Event: {@event}");
}

// SearchEventsAsync - Search all events of the event store by type

var allEventsByType = await eventSourcing.SearchEventsAsync<ProductAddedDomainEvent>();

foreach (var @event in allEventsByType)
{
    Console.WriteLine($"Event: {@event}");
}

// SearchEventsAsync - Search all events of the event store by category

var allEventsByCategory = await eventSourcing.SearchEventsAsync<ProductAddedDomainEvent>("Order");

foreach (var @event in allEventsByCategory)
{
    Console.WriteLine($"Event: {@event}");
}
