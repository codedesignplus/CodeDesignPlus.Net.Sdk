using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Core.Extensions;
using CodeDesignPlus.Net.Core.Services;
using CodeDesignPlus.Net.Event.Sourcing.Abstractions;
using CodeDesignPlus.Net.Event.Sourcing.Abstractions.Options;
using CodeDesignPlus.Net.Event.Sourcing.Extensions;
using CodeDesignPlus.Net.EventStore.Extensions;
using CodeDesignPlus.Net.EventStore.Test.Helpers.Domain;
using CodeDesignPlus.Net.EventStore.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Containers.EventStoreContainer;
using Moq;
using MO = Microsoft.Extensions.Options;
using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.EventStore.Test.Services;

[Collection(EventStoreCollectionFixture.Collection)]
public class EventStoreServiceTest(EventStoreCollectionFixture fixture) 
{
    private readonly EventStoreContainer container = fixture.Container;
    private readonly IDomainEventResolver domainEventResolverService = new DomainEventResolverService(MO.Options.Create(OptionsUtil.GetCoreOptions()));

    [Fact]
    public void Constructor_NullEventStoreFactory_ThrowsArgumentNullException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventStoreService>>();
        var options = MO.Options.Create(new EventSourcingOptions());

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new EventStoreService(null!, domainEventResolverService, loggerMock.Object, options));
    }

    [Fact]
    public void Consturctor_DomainEventResolverService_ThrowsArgumentNullException()
    {
        // Arrange
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var loggerMock = new Mock<ILogger<EventStoreService>>();
        var options = MO.Options.Create(new EventSourcingOptions());

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new EventStoreService(eventStoreFactoryMock.Object, null!, loggerMock.Object, options));
    }

    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var options = MO.Options.Create(new EventSourcingOptions());

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new EventStoreService(eventStoreFactoryMock.Object, domainEventResolverService, null!, options));
    }

    [Fact]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var loggerMock = new Mock<ILogger<EventStoreService>>();

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new EventStoreService(eventStoreFactoryMock.Object, domainEventResolverService, loggerMock.Object, null!));
    }

    [Fact]
    public void Constructor_ValidParameters_CreatesInstance()
    {
        // Arrange
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var loggerMock = new Mock<ILogger<EventStoreService>>();
        var options = MO.Options.Create(new EventSourcingOptions());

        // Act
        var eventStoreService = new EventStoreService(eventStoreFactoryMock.Object, domainEventResolverService, loggerMock.Object, options);

        // Assert
        Assert.NotNull(eventStoreService);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CountEventsAsync_CategoryIsNull_ThrowArgumentNullException(string? category)
    {
        // Arrange
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var loggerMock = new Mock<ILogger<EventStoreService>>();
        var options = MO.Options.Create(new EventSourcingOptions());

        var eventStoreService = new EventStoreService(eventStoreFactoryMock.Object, domainEventResolverService, loggerMock.Object, options);

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => eventStoreService.CountEventsAsync(category, Guid.NewGuid()));
    }

    [Fact]
    public async Task CountEventsAsync_GuidInvalid_ThrowArgumentException()
    {
        // Arrange
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var loggerMock = new Mock<ILogger<EventStoreService>>();
        var options = MO.Options.Create(new EventSourcingOptions());

        var eventStoreService = new EventStoreService(eventStoreFactoryMock.Object, domainEventResolverService, loggerMock.Object, options);

        // Act and Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => eventStoreService.CountEventsAsync("YourCategory", Guid.Empty));

        Assert.Equal("The provided aggregate ID cannot be an empty GUID. (Parameter 'aggregateId')", exception.Message);
    }

    [Fact]
    public async Task CountEventsAsync_CheckEvents_ReturnEvents()
    {
        // Arrange        
        var idAggregator = Guid.NewGuid();
        var idUserCreator = Guid.NewGuid();
        var idProductUpdate = Guid.NewGuid();
        var idProductRemove = Guid.NewGuid();

        var client = new Client()
        {
            Name = "CodeDesignPlus",
            Id = Guid.NewGuid()
        };

        var eventSourcing = GetService();

        var orderExpected = OrderAggregateRoot.Create(idAggregator, idUserCreator, client);

        AddEvents(idProductUpdate, idProductRemove, orderExpected);

        var events = orderExpected.GetAndClearEvents();

        // Act
        foreach (var @event in events)
        {
            await eventSourcing.AppendEventAsync(orderExpected.Category, @event);
        }

        var numberEvents = await eventSourcing.CountEventsAsync(orderExpected.Category, orderExpected.Id);

        // Assert
        Assert.Equal(events.Count, numberEvents);
    }

    [Fact]
    public async Task AppendEventAsync_NullEvent_ThrowsArgumentNullException()
    {
        // Arrange        
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var loggerMock = new Mock<ILogger<EventStoreService>>();
        var options = MO.Options.Create(new EventSourcingOptions());
        var aggregate = OrderAggregateRoot.Create(Guid.NewGuid(), Guid.NewGuid(), new Client());

        var eventStoreService = new EventStoreService(eventStoreFactoryMock.Object, domainEventResolverService, loggerMock.Object, options);

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => eventStoreService.AppendEventAsync<OrderCreatedEvent>(aggregate.Category, null!));
    }

    [Fact]
    public async Task AppendEventAsync_GetVersion_Succes()
    {
        // Arrange        
        var idAggregator = Guid.NewGuid();
        var idUserCreator = Guid.NewGuid();
        var idProductUpdate = Guid.NewGuid();
        var idProductRemove = Guid.NewGuid();

        var client = new Client()
        {
            Name = "CodeDesignPlus",
            Id = Guid.NewGuid()
        };

        var eventSourcing = GetService();

        var orderExpected = OrderAggregateRoot.Create(idAggregator, idUserCreator, client);

        AddEvents(idProductUpdate, idProductRemove, orderExpected);

        var events = orderExpected.GetAndClearEvents();

        // Act
        foreach (var @event in events)
        {
            await eventSourcing.AppendEventAsync(orderExpected.Category, @event);
        }

        var version = await eventSourcing.GetVersionAsync(orderExpected.Category, orderExpected.Id);

        orderExpected.AddProduct(new Product()
        {
            Id = Guid.NewGuid(),
            Name = "TV Samsung",
            Price = 10000
        }, 1);

        await eventSourcing.AppendEventAsync(orderExpected.Category, orderExpected.GetAndClearEvents()[0], version);

        var versionEnd = await eventSourcing.GetVersionAsync(orderExpected.Category, orderExpected.Id);

        // Assert
        Assert.Equal(events.Count - 1, version);
        Assert.Equal(8, versionEnd);
    }

    [Fact]
    public async Task AppendEventAsync_CheckAppend_Rehydrate()
    {
        // Arrange        
        var idAggregator = Guid.NewGuid();
        var idUserCreator = Guid.NewGuid();

        var idProductUpdate = Guid.NewGuid();
        var idProductRemove = Guid.NewGuid();

        var client = new Client()
        {
            Name = "CodeDesignPlus",
            Id = Guid.NewGuid()
        };

        var eventSourcing = GetService();

        var orderExpected = OrderAggregateRoot.Create(idAggregator, idUserCreator, client);

        AddEvents(idProductUpdate, idProductRemove, orderExpected);

        var events = orderExpected.GetAndClearEvents();

        // Act
        foreach (var @event in events)
        {
            await eventSourcing.AppendEventAsync(orderExpected.Category, @event);
        }

        var allEvents = await eventSourcing.LoadEventsAsync(orderExpected.Category, orderExpected.Id);

        var order = Event.Sourcing.Abstractions.AggregateRoot.Rehydrate<OrderAggregateRoot>(orderExpected.Id, allEvents);

        // Assert
        Assert.Equal(orderExpected.Id, order.Id);
        Assert.Equal(orderExpected.Status, order.Status);
        Assert.Equal(orderExpected.CancellationDate, order.CancellationDate);
        Assert.Equal(orderExpected.DateCreated, order.DateCreated);
        Assert.NotNull(orderExpected.Client);
        Assert.NotNull(order.Client);
        Assert.Equal(orderExpected.Client.Id, order.Client.Id);
        Assert.Equal(orderExpected.Client.Name, order.Client.Name);
        Assert.Equal(events.Count - 1, order.Version);
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

        Assert.Empty(orderExpected.GetAndClearEvents());
    }


    [Fact]
    public async Task GetVersionAsync_CheckVersion_NotExist()
    {
        // Arrange        
        var idAggregator = Guid.NewGuid();
        var idUserCreator = Guid.NewGuid();


        var client = new Client()
        {
            Name = "CodeDesignPlus",
            Id = Guid.NewGuid()
        };

        var eventSourcing = GetService();

        var orderExpected = OrderAggregateRoot.Create(idAggregator, idUserCreator, client);

        var version = await eventSourcing.GetVersionAsync(orderExpected.Category, orderExpected.Id);

        // Assert
        Assert.Equal(-1, version);
    }

    [Fact]
    public async Task GetVersionAsync_CheckVersion_Same()
    {
        // Arrange        
        var idAggregator = Guid.NewGuid();
        var idUserCreator = Guid.NewGuid();

        var idProductUpdate = Guid.NewGuid();
        var idProductRemove = Guid.NewGuid();

        var client = new Client()
        {
            Name = "CodeDesignPlus",
            Id = Guid.NewGuid()
        };

        var eventSourcing = GetService();

        var orderExpected = OrderAggregateRoot.Create(idAggregator, idUserCreator, client);

        AddEvents(idProductUpdate, idProductRemove, orderExpected);

        var events = orderExpected.GetAndClearEvents();

        // Act
        foreach (var @event in events)
        {
            await eventSourcing.AppendEventAsync(orderExpected.Category, @event);
        }

        var version = await eventSourcing.GetVersionAsync(orderExpected.Category, orderExpected.Id);

        // Assert
        Assert.Equal(events.Count - 1, version);
    }

    [Fact]
    public async Task LoadEventsAsync_NullCategory_ThrowsArgumentNullException()
    {
        // Arrange        
        var loggerMock = new Mock<ILogger<EventStoreService>>();
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var connectionMock = new Mock<IEventStoreConnection>();
        var options = MO.Options.Create(new EventSourcingOptions());

        var eventStoreService = new EventStoreService(eventStoreFactoryMock.Object, domainEventResolverService, loggerMock.Object, options);

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => eventStoreService.LoadEventsAsync(null!, Guid.NewGuid()));
    }

    [Fact]
    public async Task LoadEventsAsync_EmptyAggregateId_ThrowsArgumentException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventStoreService>>();
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var connectionMock = new Mock<IEventStoreConnection>();
        var options = MO.Options.Create(new EventSourcingOptions());

        var eventStoreService = new EventStoreService(eventStoreFactoryMock.Object, domainEventResolverService, loggerMock.Object, options);

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(() => eventStoreService.LoadEventsAsync("YourCategory", Guid.Empty));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetVersionAsync_NullCategory_ThrowsArgumentNullException(string? category)
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventStoreService>>();
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var connectionMock = new Mock<IEventStoreConnection>();
        var options = MO.Options.Create(new EventSourcingOptions());

        var eventStoreService = new EventStoreService(eventStoreFactoryMock.Object, domainEventResolverService, loggerMock.Object, options);

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => eventStoreService.GetVersionAsync(category, Guid.NewGuid()));
    }

    [Fact]
    public async Task GetVersionAsync_EmptyAggregateId_ThrowsArgumentException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventStoreService>>();
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var connectionMock = new Mock<IEventStoreConnection>();
        var options = MO.Options.Create(new EventSourcingOptions());

        var eventStoreService = new EventStoreService(eventStoreFactoryMock.Object, domainEventResolverService, loggerMock.Object, options);

        // Act and Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => eventStoreService.GetVersionAsync("YourCategory", Guid.Empty));

        Assert.Equal("The provided aggregate ID cannot be an empty GUID. (Parameter 'aggregateId')", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task LoadSnapshotAsync_NullCategory_ThrowsArgumentNullException(string? category)
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventStoreService>>();
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var connectionMock = new Mock<IEventStoreConnection>();
        var options = MO.Options.Create(new EventSourcingOptions());

        var eventStoreService = new EventStoreService(eventStoreFactoryMock.Object, domainEventResolverService, loggerMock.Object, options);

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => eventStoreService.LoadSnapshotAsync<OrderAggregateRoot>(category, Guid.NewGuid()));
    }

    [Fact]
    public async Task LoadSnapshotAsync_EmptyAggregateId_ThrowsArgumentException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventStoreService>>();
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var connectionMock = new Mock<IEventStoreConnection>();
        var options = MO.Options.Create(new EventSourcingOptions());

        var eventStoreService = new EventStoreService(eventStoreFactoryMock.Object, domainEventResolverService, loggerMock.Object, options);

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(() => eventStoreService.LoadSnapshotAsync<OrderAggregateRoot>("YourCategory", Guid.Empty));
    }

    [Fact]
    public async Task SaveSnapshotAsync_NullAggregate_ThrowsArgumentNullException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventStoreService>>();
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var connectionMock = new Mock<IEventStoreConnection>();
        var options = MO.Options.Create(new EventSourcingOptions());

        var eventStoreService = new EventStoreService(eventStoreFactoryMock.Object, domainEventResolverService, loggerMock.Object, options);

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => eventStoreService.SaveSnapshotAsync<OrderAggregateRoot>(null!));
    }


    [Fact]
    public async Task SaveSnapshotAsync_LoadSnapshotAsync_Same()
    {
        // Arrange
        var idAggregator = Guid.NewGuid();
        var idUserCreator = Guid.NewGuid();

        var idProductUpdate = Guid.NewGuid();
        var idProductRemove = Guid.NewGuid();

        var client = new Client()
        {
            Name = "CodeDesignPlus",
            Id = Guid.NewGuid()
        };

        var eventSourcing = GetService();

        var orderExpected = OrderAggregateRoot.Create(idAggregator, idUserCreator, client);

        AddEvents(idProductUpdate, idProductRemove, orderExpected);

        var events = orderExpected.GetAndClearEvents();

        foreach (var @event in events)
        {
            await eventSourcing.AppendEventAsync(orderExpected.Category, @event);
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

        Assert.Empty(orderExpected.GetAndClearEvents());
    }

    [Fact]
    public async Task SearchEventsAsync_Generic_ProductAddedToOrderEvent()
    {
        // Arrange
        var idAggregator = Guid.NewGuid();
        var idUserCreator = Guid.NewGuid();

        var idProductUpdate = Guid.NewGuid();
        var idProductRemove = Guid.NewGuid();

        var client = new Client()
        {
            Name = "CodeDesignPlus",
            Id = Guid.NewGuid()
        };

        var eventSourcing = GetService();

        var orderExpected = OrderAggregateRoot.Create(idAggregator, idUserCreator, client);

        AddEvents(idProductUpdate, idProductRemove, orderExpected);

        var events = orderExpected.GetAndClearEvents();

        foreach (var @event in events)
        {
            await eventSourcing.AppendEventAsync(orderExpected.Category, @event);
        }

        // Act
        var productAddedToOrderEvent = await eventSourcing.SearchEventsAsync<ProductAddedToOrderEvent>();

        var order = await eventSourcing.LoadSnapshotAsync<OrderAggregateRoot>(orderExpected.Category, orderExpected.Id);

        // Assert
        foreach (var eventExpected in orderExpected.GetAndClearEvents())
        {
            if (eventExpected is ProductAddedToOrderEvent value)
            {
                var @event = productAddedToOrderEvent.FirstOrDefault(x => x.EventId == value.EventId);

                Assert.NotNull(@event);
                Assert.Equal(value.AggregateId, @event.AggregateId);
                Assert.Equal(value.EventId, @event.EventId);
                Assert.Equal(value.OccurredAt, @event.OccurredAt);
                Assert.Equal(value.Quantity, @event.Quantity);
                Assert.Equal(value.Product.Id, @event.Product.Id);
                Assert.Equal(value.Product.Name, @event.Product.Name);
                Assert.Equal(value.Product.Price, @event.Product.Price);
            }
        }

        Assert.Empty(orderExpected.GetAndClearEvents());
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task SearchEventsAsync_NullCategory_ThrowsArgumentNullException(string? category)
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventStoreService>>();
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var connectionMock = new Mock<IEventStoreConnection>();
        var options = MO.Options.Create(new EventSourcingOptions());

        var eventStoreService = new EventStoreService(eventStoreFactoryMock.Object, domainEventResolverService, loggerMock.Object, options);

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => eventStoreService.SearchEventsAsync<OrderCreatedEvent>(category));
    }

    [Fact]
    public async Task SearchEventsAsync_Category_ProductAddedToOrderEvent()
    {
        // Arrange
        var idAggregator = Guid.NewGuid();
        var idUserCreator = Guid.NewGuid();

        var idProductUpdate = Guid.NewGuid();
        var idProductRemove = Guid.NewGuid();

        var client = new Client()
        {
            Name = "CodeDesignPlus",
            Id = Guid.NewGuid()
        };

        var eventSourcing = GetService();

        var orderExpected = OrderAggregateRoot.Create(idAggregator, idUserCreator, client);

        AddEvents(idProductUpdate, idProductRemove, orderExpected);

        foreach (var @event in orderExpected.GetAndClearEvents())
        {
            await eventSourcing.AppendEventAsync(orderExpected.Category, @event);
        }

        // Act
        var allEvents = await eventSourcing.SearchEventsAsync<ProductAddedToOrderEvent>(orderExpected.Category);

        var order = await eventSourcing.LoadSnapshotAsync<OrderAggregateRoot>(orderExpected.Category, orderExpected.Id);

        // Assert
        foreach (var eventExpected in orderExpected.GetAndClearEvents())
        {
            if (eventExpected is ProductAddedToOrderEvent value)
            {
                var @event = allEvents.FirstOrDefault(x => x.EventId == value.EventId);


                Assert.NotNull(@event);
                Assert.Equal(value.AggregateId, @event.AggregateId);
                Assert.Equal(value.EventId, @event.EventId);
                Assert.Equal(value.OccurredAt, @event.OccurredAt);
                Assert.Equal(value.Quantity, @event.Quantity);
                Assert.Equal(value.Product.Id, @event.Product.Id);
                Assert.Equal(value.Product.Name, @event.Product.Name);
                Assert.Equal(value.Product.Price, @event.Product.Price);

            }
        }

        Assert.Empty(orderExpected.GetAndClearEvents());
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task SearchEventsAsync_NullStreamName_ThrowsArgumentNullException(string? stream)
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventStoreService>>();
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var connectionMock = new Mock<IEventStoreConnection>();
        var options = MO.Options.Create(new EventSourcingOptions());

        var eventStoreService = new EventStoreService(eventStoreFactoryMock.Object, domainEventResolverService, loggerMock.Object, options);

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => eventStoreService.SearchEventsAsync(stream));
    }

    [Fact]
    public async Task SearchEventsAsync_Stream_ReturnAllEvents()
    {
        // Arrange
        var idAggregator = Guid.NewGuid();
        var idUserCreator = Guid.NewGuid();

        var idProductUpdate = Guid.NewGuid();
        var idProductRemove = Guid.NewGuid();

        var client = new Client()
        {
            Name = "CodeDesignPlus",
            Id = Guid.NewGuid()
        };

        var eventSourcing = GetService();

        var orderExpected = OrderAggregateRoot.Create(idAggregator, idUserCreator, client);

        AddEvents(idProductUpdate, idProductRemove, orderExpected);

        var events = orderExpected.GetAndClearEvents();

        foreach (var @event in events)
        {
            await eventSourcing.AppendEventAsync(orderExpected.Category, @event);
        }

        // Act
        var allEvents = await eventSourcing.SearchEventsAsync($"{orderExpected.Category}-{orderExpected.Id}");

        var order = await eventSourcing.LoadSnapshotAsync<OrderAggregateRoot>(orderExpected.Category, orderExpected.Id);

        // Assert
        var eventsExpected = orderExpected.GetAndClearEvents();
        foreach (var eventExpected in eventsExpected)
        {
            var @event = allEvents.FirstOrDefault(x => x.EventId == eventExpected.EventId);

            Assert.NotNull(@event);
            Assert.Equal(eventExpected.AggregateId, @event.AggregateId);
            Assert.Equal(eventExpected.EventId, @event.EventId);
            Assert.Equal(eventExpected.OccurredAt, @event.OccurredAt);
        }

        Assert.Empty(orderExpected.GetAndClearEvents());
    }

    private IEventSourcing GetService()
    {
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            Core = new CoreOptions
            {
                Id = Guid.NewGuid(),
                AppName = "ms-test",
                TypeEntryPoint = "rest",
                Version = "v1",
                Business = "CodeDesignPlus",
                Description = "Description Test",
                Contact = new Contact
                {
                    Name = "CodeDesignPlus",
                    Email = "codedesignplus@outlook.com"
                }
            },
            EventSourcing = new
            {
                MainName = "aggregate",
                SnapshotSuffix = "snapshot"
            },
            EventStore = OptionsUtil.GetOptions("localhost", container.Port)
        });
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddLogging()
            .AddCore(configuration)
            .AddEventSourcing(configuration)
            .AddEventStore(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var eventSourcing = serviceProvider.GetRequiredService<IEventSourcing>();
        return eventSourcing;
    }

    private static void AddEvents(Guid idProductUpdate, Guid idProductRemove, OrderAggregateRoot orderExpected)
    {
        orderExpected.AddProduct(new Product()
        {
            Id = Guid.NewGuid(),
            Name = "TV",
            Price = 10000
        }, 1);

        orderExpected.AddProduct(new Product()
        {
            Id = Guid.NewGuid(),
            Name = "Phone",
            Price = 10000
        }, 3);

        orderExpected.AddProduct(new Product()
        {
            Id = idProductRemove,
            Name = "Monitor",
            Price = 10000
        }, 7);

        orderExpected.AddProduct(new Product()
        {
            Id = idProductUpdate,
            Name = "Mouse",
            Price = 10000
        }, 10);

        orderExpected.UpdateProductQuantity(idProductUpdate, 20);

        orderExpected.RemoveProduct(idProductRemove);

        orderExpected.CompleteOrder();
    }

}
