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
        Assert.Contains(events, x => x.EventType == "OrderCreated");
        Assert.Contains(events, x => x.EventType == "OrderUpdated");
        Assert.Contains(events, x => x.EventType == "OrderDeleted");

        Assert.Empty(orderAggregate.GetAndClearEvents());
    }

    [Fact]
    public void EventBase()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow;
        var deleteAt = DateTime.UtcNow;
        var orderAggregate = OrderAggregate.Create(id, "Test", "Test Description", 10, createAt);

        // Act
        orderAggregate.Update("Test 2", "Test Description 2", 20, updatedAt);
        orderAggregate.Delete(deleteAt);

        var events = orderAggregate.GetAndClearEvents();

        foreach (var domainEvent in events)
        {
            // Assert
            var @event = new Event(domainEvent, null);

            var json = JsonSerializer.Serialize(@event);

            

        }


    }
}
