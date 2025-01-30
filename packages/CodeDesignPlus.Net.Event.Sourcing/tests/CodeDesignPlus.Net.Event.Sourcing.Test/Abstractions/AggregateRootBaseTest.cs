using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Event.Sourcing.Test.Helpers.Aggregates;
using CodeDesignPlus.Net.Event.Sourcing.Test.Helpers.Events;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Abstractions;
public class AggregateRootBaseTest
{
    [Fact]
    public void OrderCreated_UpdateState_Success()
    {
        // Arrange
        var idUser = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();
        var name = "TV";

        var aggregate = OrderAggregateRoot.Create(aggregateId, name, idUser);

        // Act
        var domainEvent = aggregate.GetAndClearEvents()[0];

        // Assert
        Assert.Equal("Order", aggregate.Category);

        Assert.Equal(idUser, aggregate.IdUser);
        Assert.Equal(aggregateId, aggregate.Id);
        Assert.Equal(name, aggregate.Name);
        Assert.Equal(0, aggregate.Version);
        Assert.Empty(aggregate.Products);

        Assert.NotNull(domainEvent);
        Assert.Equal(aggregateId, domainEvent.AggregateId);
        Assert.True(Guid.Empty != domainEvent.EventId);
        Assert.True(SystemClock.Instance.GetCurrentInstant() != domainEvent.OccurredAt);
        Assert.Equal(0, (long)domainEvent.Metadata["Version"]);
        Assert.Equal("Order", domainEvent.Metadata["Category"]);
    }

    [Fact]
    public void ApplyEvent_UpdateName_Success()
    {
        // Arrange
        var idUser = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();
        var name = "Andrew Reyes";

        var aggregate = OrderAggregateRoot.Create(aggregateId, name, idUser);

        // Act
        aggregate.UpdateName("Cristofher Reyes");

        var domainEvents = aggregate.GetAndClearEvents();

        var orderCreated = domainEvents.Where(x => x.GetType() == typeof(OrderCreatedDomainEvent)).FirstOrDefault();
        var nameUpdated = domainEvents.Where(x => x.GetType() == typeof(NameUpdatedDomainEvent)).FirstOrDefault();


        // Assert
        Assert.Equal(aggregateId, aggregate.Id);
        Assert.Equal("Cristofher Reyes", aggregate.Name);
        Assert.Equal(1, aggregate.Version);
        Assert.Equal("Order", aggregate.Category);
        Assert.Empty(aggregate.Products);

        Assert.NotNull(orderCreated);
        Assert.Equal(aggregateId, orderCreated.AggregateId);
        Assert.True(Guid.Empty != orderCreated.EventId);
        Assert.True(SystemClock.Instance.GetCurrentInstant() != orderCreated.OccurredAt);
        Assert.Equal(0, (long)orderCreated.Metadata["Version"]);
        Assert.Equal("Order", orderCreated.Metadata["Category"]);
        Assert.Equal(name, ((OrderCreatedDomainEvent)orderCreated).Name);

        Assert.NotNull(nameUpdated);
        Assert.Equal(aggregateId, nameUpdated.AggregateId);
        Assert.True(Guid.Empty != nameUpdated.EventId);
        Assert.True(SystemClock.Instance.GetCurrentInstant() != nameUpdated.OccurredAt);
        Assert.Equal(1, (long)nameUpdated.Metadata["Version"]);
        Assert.Equal("Order", nameUpdated.Metadata["Category"]);
        Assert.Equal("Cristofher Reyes", ((NameUpdatedDomainEvent)nameUpdated).Name);
    }

    [Fact]
    public void Rehydrate_RestoreState_Success()
    {
        // Arrange
        var idUser = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();
        var name = "Andrew Reyes";

        // Crear eventos de ejemplo
        var events = new List<IDomainEvent>
        {
            new OrderCreatedDomainEvent(aggregateId, name, idUser, Guid.NewGuid(), SystemClock.Instance.GetCurrentInstant(), new Dictionary<string, object>() {
                { "Version", 0 },
                { "Category", "Order" }
            }),
            new NameUpdatedDomainEvent(aggregateId, "Cristofher Reyes", Guid.NewGuid(), SystemClock.Instance.GetCurrentInstant(), new Dictionary<string, object>() {
                { "Version", 1 },
                { "Category", "Order" }
            }),
            new ProductAddedDomainEvent(aggregateId, "TV", Guid.NewGuid(), SystemClock.Instance.GetCurrentInstant(), new Dictionary<string, object>() {
                { "Version", 2 },
                { "Category", "Order" }
            })
        };

        // Act
        var rehydratedAggregate = Sourcing.Abstractions.AggregateRoot.Rehydrate<OrderAggregateRoot>(aggregateId, events);

        // Assert
        Assert.Equal(aggregateId, rehydratedAggregate.Id);
        Assert.Equal("Cristofher Reyes", rehydratedAggregate.Name);
        Assert.Equal(idUser, rehydratedAggregate.IdUser);
        Assert.Equal(2, rehydratedAggregate.Version);
        Assert.Equal("Order", rehydratedAggregate.Category);
        Assert.NotEmpty(rehydratedAggregate.Products);
        Assert.Contains(rehydratedAggregate.Products, x => x == "TV");
    }
}
