namespace CodeDesignPlus.Net.Core.Test;

public class AggregateRootTest
{
    [Fact]
    public void AggregateRoot()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow;
        var deleteAt = DateTime.UtcNow;
        var orderAggregate = OrderAggregate.Create(id, "Test", "Test Description", 10, createAt);

        // Act
        orderAggregate.Update("Test 2", "Test Description 2", 20, updatedAt);

        // Assert
        Assert.Equal(id, orderAggregate.Id);
        Assert.Equal("Test 2", orderAggregate.Name);
        Assert.Equal("Test Description 2", orderAggregate.Description);
        Assert.Equal(20, orderAggregate.Price);
        Assert.Equal(createAt, orderAggregate.CreatedAt);
        Assert.Equal(updatedAt, orderAggregate.UpdatedAt);

        orderAggregate.Delete(deleteAt);

        Assert.Equal(deleteAt, orderAggregate.UpdatedAt);

        var events = orderAggregate.GetAndClearEvents();

        Assert.Equal(3, events.Count);
        Assert.Empty(orderAggregate.GetAndClearEvents());
    }

    [Fact]
    public void EventBase()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createAt = DateTime.UtcNow;
        var orderAggregate = OrderAggregate.Create(id, "Test", "Test Description", 10, createAt);

        // Act
        var domainEvent = (OrderCreatedDomainEvent)orderAggregate.GetAndClearEvents().FirstOrDefault()!;


        // Assert
        var json = JsonConvert.SerializeObject(domainEvent, new EventJsonConvert());

        var @event = JsonConvert.DeserializeObject<OrderCreatedDomainEvent>(json, new EventJsonConvert());

        Assert.NotNull(@event);
        Assert.Equal(domainEvent.EventId, @event.EventId);
        Assert.Equal(domainEvent.OccurredAt, @event.OccurredAt);
        Assert.Equal(domainEvent.AggregateId, @event.AggregateId);
        Assert.Equal(domainEvent.EventType, @event.EventType);
        Assert.Equal(domainEvent.Name, @event.Name);
        Assert.Equal(domainEvent.Description, @event.Description);
        Assert.Equal(domainEvent.Price, @event.Price);
        Assert.Equal(domainEvent.CreatedAt, @event.CreatedAt);
        Assert.Equal(domainEvent.UpdatedAt, @event.UpdatedAt);

        foreach (var item in domainEvent.Metadata)
        {
            Assert.True(@event.Metadata.ContainsKey(item.Key));
            Assert.Equal(item.Value, @event.Metadata[item.Key]);
        }
    }

}