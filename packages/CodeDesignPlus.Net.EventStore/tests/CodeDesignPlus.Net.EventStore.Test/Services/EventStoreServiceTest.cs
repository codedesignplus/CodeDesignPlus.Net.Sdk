using CodeDesignPlus.Net.Event.Sourcing.Abstractions;
using CodeDesignPlus.Net.Event.Sourcing.Extensions;
using CodeDesignPlus.Net.EventStore.Extensions;
using CodeDesignPlus.Net.EventStore.Test.Helpers.Domain;
using CodeDesignPlus.Net.EventStore.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Helpers;
using CodeDesignPlus.Net.xUnit.Helpers.EventStoreContainer;

namespace CodeDesignPlus.Net.EventStore.Test;

public class EventStoreServiceTest : IClassFixture<EventStoreContainer>
{
    private readonly EventStoreContainer fixture;

    public EventStoreServiceTest(EventStoreContainer fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task AppendEventAsync_CheckAppend_Rehydrate()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            EventSourcing = new
            {
                MainName = "aggregate",
                SnapshotSuffix = "snapshot"
            },
            EventStore = OptionsUtil.GetOptions("localhost", this.fixture.Port)
        });
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddLogging()
            .AddEventSourcing(configuration)
            .AddEventStore(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var eventSourcing = serviceProvider.GetRequiredService<IEventSourcingService<Guid>>();

        var idAggregator = Guid.NewGuid();
        var idUserCreator = Guid.NewGuid();
        var idUserEvent = Guid.NewGuid();
        var idProductUpdate = Guid.NewGuid();
        var idProductRemove = Guid.NewGuid();

        var client = new Client()
        {
            Name = "CodeDesignPlus",
            Id = Guid.NewGuid()
        };

        var orderExpected = new OrderAggregateRoot(idAggregator, idUserCreator, idUserEvent, client);

        orderExpected.AddProduct(new Product()
        {
            Id = Guid.NewGuid(),
            Name = "TV",
            Price = 10000
        }, 1, idUserEvent);

        orderExpected.AddProduct(new Product()
        {
            Id = Guid.NewGuid(),
            Name = "Phone",
            Price = 10000
        }, 3, idUserEvent);

        orderExpected.AddProduct(new Product()
        {
            Id = idProductRemove,
            Name = "Monitor",
            Price = 10000
        }, 7, idUserEvent);

        orderExpected.AddProduct(new Product()
        {
            Id = idProductUpdate,
            Name = "Mouse",
            Price = 10000
        }, 10, idUserEvent);

        orderExpected.UpdateProductQuantity(idProductUpdate, 20, idUserEvent);

        orderExpected.RemoveProduct(idProductRemove, idUserEvent);

        orderExpected.CompleteOrder(idUserEvent);

        // Act
        foreach (var (@event, metadata) in orderExpected.UncommittedEvents)
        {
            await eventSourcing.AppendEventAsync(@event, metadata);
        }

        var allEvents = await eventSourcing.LoadEventsAsync(orderExpected.Category, orderExpected.Id);

        var order = OrderAggregateRoot.Rehydrate<OrderAggregateRoot>(allEvents);

        // Assert
        Assert.Equal(orderExpected.Id, order.Id);
        Assert.Equal(orderExpected.Status, order.Status);
        Assert.Equal(orderExpected.CancellationDate, order.CancellationDate);
        Assert.Equal(orderExpected.DateCreated, order.DateCreated);
        Assert.NotNull(orderExpected.Client);
        Assert.NotNull(order.Client);
        Assert.Equal(orderExpected.Client.Id, order.Client.Id);
        Assert.Equal(orderExpected.Client.Name, order.Client.Name);
        Assert.Equal(orderExpected.UncommittedEvents.Count - 1, order.Version);
        Assert.Equal(orderExpected.CompletionDate, order.CompletionDate);
        Assert.Equal(orderExpected.IdUserCreator, order.IdUserCreator);
        Assert.Contains(
            orderExpected.Products,
            x => order.Products
                .Any(o =>
                    o.Quantity == x.Quantity
                    && o.Product.Id == x.Product.Id
                    && o.Product.Name == x.Product.Name
                    && o.Product.Price == o.Product.Price
                )
        );


        orderExpected.ClearUncommittedEvents();

        Assert.Empty(orderExpected.UncommittedEvents);
    }


    [Fact]
    public async Task SaveSnapshotAsync_LoadSnapshotAsync_Same()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            EventSourcing = new
            {
                MainName = "aggregate",
                SnapshotSuffix = "snapshot"
            },
            EventStore = OptionsUtil.GetOptions("localhost", this.fixture.Port)
        });
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddLogging()
            .AddEventSourcing(configuration)
            .AddEventStore(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var eventSourcing = serviceProvider.GetRequiredService<IEventSourcingService<Guid>>();

        var idAggregator = Guid.NewGuid();
        var idUserCreator = Guid.NewGuid();
        var idUserEvent = Guid.NewGuid();
        var idProductUpdate = Guid.NewGuid();
        var idProductRemove = Guid.NewGuid();

        var client = new Client()
        {
            Name = "CodeDesignPlus",
            Id = Guid.NewGuid()
        };

        var orderExpected = new OrderAggregateRoot(idAggregator, idUserCreator, idUserEvent, client);

        orderExpected.AddProduct(new Product()
        {
            Id = Guid.NewGuid(),
            Name = "TV",
            Price = 10000
        }, 1, idUserEvent);

        orderExpected.AddProduct(new Product()
        {
            Id = Guid.NewGuid(),
            Name = "Phone",
            Price = 10000
        }, 3, idUserEvent);

        orderExpected.AddProduct(new Product()
        {
            Id = idProductRemove,
            Name = "Monitor",
            Price = 10000
        }, 7, idUserEvent);

        orderExpected.AddProduct(new Product()
        {
            Id = idProductUpdate,
            Name = "Mouse",
            Price = 10000
        }, 10, idUserEvent);

        orderExpected.UpdateProductQuantity(idProductUpdate, 20, idUserEvent);

        orderExpected.RemoveProduct(idProductRemove, idUserEvent);

        orderExpected.CompleteOrder(idUserEvent);

        foreach (var (@event, metadata) in orderExpected.UncommittedEvents)
        {
            await eventSourcing.AppendEventAsync(@event, metadata);
        }

        // Act
        await eventSourcing.SaveSnapshotAsync(orderExpected);

        var order = await eventSourcing.LoadSnapshotAsync<OrderAggregateRoot>(orderExpected.Category, orderExpected.Id);

        // Assert
        Assert.Equal(orderExpected.Id, order.Id);
        Assert.Equal(orderExpected.Status, order.Status);
        Assert.Equal(orderExpected.CancellationDate, order.CancellationDate);
        Assert.Equal(orderExpected.DateCreated, order.DateCreated);
        Assert.NotNull(orderExpected.Client);
        Assert.NotNull(order.Client);
        Assert.Equal(orderExpected.Client.Id, order.Client.Id);
        Assert.Equal(orderExpected.Client.Name, order.Client.Name);
        Assert.Equal(orderExpected.Version, order.Version);
        Assert.Equal(orderExpected.CompletionDate, order.CompletionDate);
        Assert.Equal(orderExpected.IdUserCreator, order.IdUserCreator);
        Assert.Contains(
            orderExpected.Products,
            x => order.Products
                .Any(o =>
                    o.Quantity == x.Quantity
                    && o.Product.Id == x.Product.Id
                    && o.Product.Name == x.Product.Name
                    && o.Product.Price == o.Product.Price
                )
        );

        orderExpected.ClearUncommittedEvents();

        Assert.Empty(orderExpected.UncommittedEvents);
    }

    [Fact]
    public async Task SearchEventsAsync_Generic_ProductAddedToOrderEvent()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            EventSourcing = new
            {
                MainName = "aggregate",
                SnapshotSuffix = "snapshot"
            },
            EventStore = OptionsUtil.GetOptions("localhost", this.fixture.Port)
        });
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddLogging()
            .AddEventSourcing(configuration)
            .AddEventStore(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var eventSourcing = serviceProvider.GetRequiredService<IEventSourcingService<Guid>>();

        var idAggregator = Guid.NewGuid();
        var idUserCreator = Guid.NewGuid();
        var idUserEvent = Guid.NewGuid();
        var idProductUpdate = Guid.NewGuid();
        var idProductRemove = Guid.NewGuid();

        var client = new Client()
        {
            Name = "CodeDesignPlus",
            Id = Guid.NewGuid()
        };

        var orderExpected = new OrderAggregateRoot(idAggregator, idUserCreator, idUserEvent, client);

        orderExpected.AddProduct(new Product()
        {
            Id = Guid.NewGuid(),
            Name = "TV",
            Price = 10000
        }, 1, idUserEvent);

        orderExpected.AddProduct(new Product()
        {
            Id = Guid.NewGuid(),
            Name = "Phone",
            Price = 10000
        }, 3, idUserEvent);

        orderExpected.AddProduct(new Product()
        {
            Id = idProductRemove,
            Name = "Monitor",
            Price = 10000
        }, 7, idUserEvent);

        orderExpected.AddProduct(new Product()
        {
            Id = idProductUpdate,
            Name = "Mouse",
            Price = 10000
        }, 10, idUserEvent);

        orderExpected.UpdateProductQuantity(idProductUpdate, 20, idUserEvent);

        orderExpected.RemoveProduct(idProductRemove, idUserEvent);

        orderExpected.CompleteOrder(idUserEvent);

        foreach (var (@event, metadata) in orderExpected.UncommittedEvents)
        {
            await eventSourcing.AppendEventAsync(@event, metadata);
        }

        // Act
        var productAddedToOrderEvent = await eventSourcing.SearchEventsAsync<ProductAddedToOrderEvent>();

        var order = await eventSourcing.LoadSnapshotAsync<OrderAggregateRoot>(orderExpected.Category, orderExpected.Id);

        // Assert
        foreach (var (eventExpected, metadataExpected) in orderExpected.UncommittedEvents)
        {
            if (eventExpected is ProductAddedToOrderEvent value)
            {
                var (@event, metadata) = productAddedToOrderEvent.FirstOrDefault(x => x.Item1.IdEvent == value.IdEvent);

                Assert.Equal(value.AggregateId, @event.AggregateId);
                Assert.Equal(value.IdEvent, @event.IdEvent);
                Assert.Equal(value.EventDate, @event.EventDate);
                Assert.Equal(value.EventType, @event.EventType);
                Assert.Equal(value.Quantity, @event.Quantity);
                Assert.Equal(value.Product.Id, @event.Product.Id);
                Assert.Equal(value.Product.Name, @event.Product.Name);
                Assert.Equal(value.Product.Price, @event.Product.Price);

                Assert.Equal(metadataExpected.Category, metadata.Category);
                Assert.Equal(metadataExpected.AggregateId, metadata.AggregateId);
                Assert.Equal(metadataExpected.Version, metadata.Version);
                Assert.Equal(metadataExpected.UserId, metadata.UserId);
            }
        }

        orderExpected.ClearUncommittedEvents();

        Assert.Empty(orderExpected.UncommittedEvents);
    }

    [Fact]
    public async Task SearchEventsAsync_Category_ProductAddedToOrderEvent()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            EventSourcing = new
            {
                MainName = "aggregate",
                SnapshotSuffix = "snapshot"
            },
            EventStore = OptionsUtil.GetOptions("localhost", this.fixture.Port)
        });
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddLogging()
            .AddEventSourcing(configuration)
            .AddEventStore(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var eventSourcing = serviceProvider.GetRequiredService<IEventSourcingService<Guid>>();

        var idAggregator = Guid.NewGuid();
        var idUserCreator = Guid.NewGuid();
        var idUserEvent = Guid.NewGuid();
        var idProductUpdate = Guid.NewGuid();
        var idProductRemove = Guid.NewGuid();

        var client = new Client()
        {
            Name = "CodeDesignPlus",
            Id = Guid.NewGuid()
        };

        var orderExpected = new OrderAggregateRoot(idAggregator, idUserCreator, idUserEvent, client);

        orderExpected.AddProduct(new Product()
        {
            Id = Guid.NewGuid(),
            Name = "TV",
            Price = 10000
        }, 1, idUserEvent);

        orderExpected.AddProduct(new Product()
        {
            Id = Guid.NewGuid(),
            Name = "Phone",
            Price = 10000
        }, 3, idUserEvent);

        orderExpected.AddProduct(new Product()
        {
            Id = idProductRemove,
            Name = "Monitor",
            Price = 10000
        }, 7, idUserEvent);

        orderExpected.AddProduct(new Product()
        {
            Id = idProductUpdate,
            Name = "Mouse",
            Price = 10000
        }, 10, idUserEvent);

        orderExpected.UpdateProductQuantity(idProductUpdate, 20, idUserEvent);

        orderExpected.RemoveProduct(idProductRemove, idUserEvent);

        orderExpected.CompleteOrder(idUserEvent);

        foreach (var (@event, metadata) in orderExpected.UncommittedEvents)
        {
            await eventSourcing.AppendEventAsync(@event, metadata);
        }

        // Act
        var allEvents = await eventSourcing.SearchEventsAsync<ProductAddedToOrderEvent>(orderExpected.Category);

        var order = await eventSourcing.LoadSnapshotAsync<OrderAggregateRoot>(orderExpected.Category, orderExpected.Id);

        // Assert
        foreach (var (eventExpected, metadataExpected) in orderExpected.UncommittedEvents)
        {
            if (eventExpected is ProductAddedToOrderEvent value)
            {
                var (@event, metadata) = allEvents.FirstOrDefault(x => x.Item1.IdEvent == value.IdEvent);

                Assert.Equal(value.AggregateId, @event.AggregateId);
                Assert.Equal(value.IdEvent, @event.IdEvent);
                Assert.Equal(value.EventDate, @event.EventDate);
                Assert.Equal(value.EventType, @event.EventType);
                Assert.Equal(value.Quantity, @event.Quantity);
                Assert.Equal(value.Product.Id, @event.Product.Id);
                Assert.Equal(value.Product.Name, @event.Product.Name);
                Assert.Equal(value.Product.Price, @event.Product.Price);

                Assert.Equal(metadataExpected.Category, metadata.Category);
                Assert.Equal(metadataExpected.AggregateId, metadata.AggregateId);
                Assert.Equal(metadataExpected.Version, metadata.Version);
                Assert.Equal(metadataExpected.UserId, metadata.UserId);
            }
        }

        orderExpected.ClearUncommittedEvents();

        Assert.Empty(orderExpected.UncommittedEvents);
    }

    
    [Fact]
    public async Task SearchEventsAsync_Stream_ReturnAllEvents()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            EventSourcing = new
            {
                MainName = "aggregate",
                SnapshotSuffix = "snapshot"
            },
            EventStore = OptionsUtil.GetOptions("localhost", this.fixture.Port)
        });
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddLogging()
            .AddEventSourcing(configuration)
            .AddEventStore(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var eventSourcing = serviceProvider.GetRequiredService<IEventSourcingService<Guid>>();

        var idAggregator = Guid.NewGuid();
        var idUserCreator = Guid.NewGuid();
        var idUserEvent = Guid.NewGuid();
        var idProductUpdate = Guid.NewGuid();
        var idProductRemove = Guid.NewGuid();

        var client = new Client()
        {
            Name = "CodeDesignPlus",
            Id = Guid.NewGuid()
        };

        var orderExpected = new OrderAggregateRoot(idAggregator, idUserCreator, idUserEvent, client);

        orderExpected.AddProduct(new Product()
        {
            Id = Guid.NewGuid(),
            Name = "TV",
            Price = 10000
        }, 1, idUserEvent);

        orderExpected.AddProduct(new Product()
        {
            Id = Guid.NewGuid(),
            Name = "Phone",
            Price = 10000
        }, 3, idUserEvent);

        orderExpected.AddProduct(new Product()
        {
            Id = idProductRemove,
            Name = "Monitor",
            Price = 10000
        }, 7, idUserEvent);

        orderExpected.AddProduct(new Product()
        {
            Id = idProductUpdate,
            Name = "Mouse",
            Price = 10000
        }, 10, idUserEvent);

        orderExpected.UpdateProductQuantity(idProductUpdate, 20, idUserEvent);

        orderExpected.RemoveProduct(idProductRemove, idUserEvent);

        orderExpected.CompleteOrder(idUserEvent);

        foreach (var (@event, metadata) in orderExpected.UncommittedEvents)
        {
            await eventSourcing.AppendEventAsync(@event, metadata);
        }

        // Act
        var allEvents = await eventSourcing.SearchEventsAsync($"{orderExpected.Category}-{orderExpected.Id}");

        var order = await eventSourcing.LoadSnapshotAsync<OrderAggregateRoot>(orderExpected.Category, orderExpected.Id);

        // Assert
        foreach (var (eventExpected, metadataExpected) in orderExpected.UncommittedEvents.Select(x => ((DomainEventBase)x.Event, x.Metadata)))
        {
                var (@event, metadata) = allEvents.Select(x => ((DomainEventBase)x.Item1, x.Item2)).FirstOrDefault(x => x.Item1.IdEvent == eventExpected.IdEvent);

                Assert.Equal(eventExpected.AggregateId, @event.AggregateId);
                Assert.Equal(eventExpected.IdEvent, @event.IdEvent);
                Assert.Equal(eventExpected.EventDate, @event.EventDate);
                Assert.Equal(eventExpected.EventType, @event.EventType);

                Assert.Equal(metadataExpected.Category, metadata.Category);
                Assert.Equal(metadataExpected.AggregateId, metadata.AggregateId);
                Assert.Equal(metadataExpected.Version, metadata.Version);
                Assert.Equal(metadataExpected.UserId, metadata.UserId);
            
        }

        orderExpected.ClearUncommittedEvents();

        Assert.Empty(orderExpected.UncommittedEvents);
    }
}
