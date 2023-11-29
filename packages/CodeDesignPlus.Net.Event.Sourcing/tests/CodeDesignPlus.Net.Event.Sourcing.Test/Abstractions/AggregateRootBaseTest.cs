namespace CodeDesignPlus.Net.Event.Sourcing.Test;
public class AggregateRootBaseTest
{
    [Fact]
    public void OrderCreated_UpdateState_Success()
    {
        // Arrange
        var idUser = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();
        var name = "TV";

        var aggregate = new OrderAggregateRoot(aggregateId, name, idUser);

        // Act
        var uncommittedEvent = aggregate.UncommittedEvents[0];
        var metadata = uncommittedEvent.Metadata;

        // Assert
        Assert.Single(aggregate.UncommittedEvents);
        Assert.Equal("Order", aggregate.Category);

        Assert.Equal(aggregateId, metadata.AggregateId);
        Assert.Equal(idUser, metadata.UserId);
        Assert.Equal(0, metadata.Version);

        Assert.Equal(aggregateId, aggregate.Id);
        Assert.Equal(name, aggregate.Name);
        Assert.Equal(-1, aggregate.Version);
    }

    [Fact]
    public void ApplyEvent_UpdateState_Success()
    {
        // Arrange
        var idUser = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();
        var name = "TV";

        // Crear un evento de ejemplo
        var @event = new OrderCreatedEvent(aggregateId, name);
        var metadata = new Metadata<Guid>(aggregateId, 0, idUser, "Order");
        var aggregate = new OrderAggregateRoot();

        // Act
        aggregate.ApplyEvent(@event, metadata);

        // Assert
        Assert.Equal(aggregateId, aggregate.Id);
        Assert.Equal(name, aggregate.Name);
        Assert.Equal(-1, aggregate.Version);
    }

    [Fact]
    public void Rehydrate_RestoreState_Success()
    {
        // Arrange
        var idUser = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();
        var name = "TV";

        // Crear eventos de ejemplo
        var events = new List<(IDomainEvent, Metadata<Guid>)>
        {
            (new OrderCreatedEvent(aggregateId, name), new Metadata<Guid>(aggregateId, 0, idUser, "Order")),
        };

        // Act
        var rehydratedAggregate = OrderAggregateRoot.Rehydrate<OrderAggregateRoot>(events);

        // Assert
        Assert.Equal(aggregateId, rehydratedAggregate.Id);
        Assert.Equal(name, rehydratedAggregate.Name);
        Assert.Equal(0, rehydratedAggregate.Version);
        Assert.Equal("Order", rehydratedAggregate.Category);
    }
    [Fact]
    public void CreateOrGetDelegate_CreateNewDelegate_ReturnsTrue()
    {
        // Act
        var delegate1 = OrderAggregateRoot.CreateOrGetDelegate<OrderAggregateRoot>();
        Assert.NotNull(delegate1);

        var delegate2 = OrderAggregateRoot.CreateOrGetDelegate<OrderAggregateRoot>();
        Assert.NotNull(delegate2);

        // Assert
        Assert.Equal(delegate1, delegate2);
    }

    [Fact]
    public void UncommittedEventsAndClear_UncommittedEventsCleared_Success()
    {
        // Arrange
        var idUser = Guid.NewGuid();
        var aggregateId = Guid.NewGuid();
        var name = "TV";

        var @event = new OrderCreatedEvent(aggregateId, name);
        var aggregate = new OrderAggregateRoot();

        aggregate.ApplyChange(@event, idUser);

        // Act
        var uncommittedEvents = aggregate.UncommittedEvents.ToList();
        aggregate.ClearUncommittedEvents();

        // Assert
        Assert.Single(uncommittedEvents);
        var uncommittedEvent = uncommittedEvents[0];
        var metadata = uncommittedEvent.Metadata;
        Assert.Equal(@event, uncommittedEvent.Event);
        Assert.Equal(aggregateId, metadata.AggregateId);
        Assert.Equal(idUser, metadata.UserId);
        Assert.Equal(0, metadata.Version);
        Assert.Equal(aggregate.Category, metadata.Category);
        Assert.Empty(aggregate.UncommittedEvents);
    }
}
