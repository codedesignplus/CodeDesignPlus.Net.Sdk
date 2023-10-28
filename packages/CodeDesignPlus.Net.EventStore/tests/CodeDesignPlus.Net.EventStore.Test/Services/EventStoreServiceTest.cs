using CodeDesignPlus.Net.Event.Sourcing.Abstractions;
using CodeDesignPlus.Net.Event.Sourcing.Extensions;
using CodeDesignPlus.Net.Event.Sourcing.Options;
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
    public async Task AppendEventAsync_CheckAppend_SameEvent()
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

        var eventSourcing = serviceProvider.GetRequiredService<IEventSourcingService>();

        try
        {

            var order = new OrderAggregateRoot(DateTime.UtcNow, new Client("CodeDesignPlus"), Guid.NewGuid());

            order.Version = await eventSourcing.GetAggregateVersionAsync(order.Id);

            order.AddProduct(new Product("TV", 10000), 1);
            order.AddProduct(new Product("Phone", 20000), 1);
            order.AddProduct(new Product("Xbox X", 30000), 1);

            order.CompleteOrder();

            foreach (var @event in order.GetUncommittedEvents())
            {
                await eventSourcing.AppendEventAsync(@event);
            }

            var allEvents = await eventSourcing.LoadEventsForAggregateAsync(order.Id);

            Assert.NotEmpty(allEvents);
        }
        catch (System.Exception ex)
        {

            throw;
        }

    }
}



