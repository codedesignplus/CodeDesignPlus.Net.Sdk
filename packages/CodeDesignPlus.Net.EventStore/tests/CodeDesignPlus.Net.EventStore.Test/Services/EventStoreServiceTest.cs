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
    private readonly EventStoreContainer _fixture;

    public EventStoreServiceTest(EventStoreContainer fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AppendEventAsync_CheckAppend_SameAggregate()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            EventSourcing = new
            {
                MainName = "aggregate",
                SnapshotSuffix = "snapshot"
            },
            EventStore = OptionsUtil.EventStoreOptions
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


        //----------------------------------------------------------------------

        var allEvents = await eventSourcing.LoadEventsAsync(orderExpected.Category, orderExpected.Id);

        var order = OrderAggregateRoot.Rehydrate<OrderAggregateRoot>(allEvents);

        Assert.Equal(orderExpected.Id, order.Id);
        Assert.Equal(orderExpected.Status, order.Status);
        Assert.Equal(orderExpected.CancellationDate, order.CancellationDate);
        Assert.Equal(orderExpected.DateCreated, order.DateCreated);
        Assert.NotNull(orderExpected.Client);
        Assert.NotNull(order.Client);
        Assert.Equal(orderExpected.Client.Id, order.Client.Id);
        Assert.Equal(orderExpected.Client.Name, order.Client.Name);
        Assert.Equal(orderExpected.UncommittedEvents.Count() - 1, order.Version);
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

        // Snapshot    

        await eventSourcing.SaveSnapshotAsync(order);

        var orderSnapshot = await eventSourcing.LoadSnapshotAsync<OrderAggregateRoot>(orderExpected.Category, order.Id);

        var productAddedToOrderEvent = await eventSourcing.SearchEventsAsync<ProductAddedToOrderEvent>();

        var productAddedToOrderEvent2 = await eventSourcing.SearchEventsAsync<ProductAddedToOrderEvent>(order.Category);

        EventStoreService<Guid> eventStore = (EventStoreService<Guid>)eventSourcing;
    }
    
}



