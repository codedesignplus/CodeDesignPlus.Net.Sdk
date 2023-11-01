namespace CodeDesignPlus.Net.Event.Sourcing.Test;

public class DomainEventBaseTest
{
    [Fact]
    public void DomainEventBase_AggregateId_SetCorrectly()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();

        // Act
        var domainEvent = new OrderCreatedEvent(aggregateId, "Temp");

        // Assert
        Assert.Equal(aggregateId, domainEvent.AggregateId);
    }

    [Fact]
    public void DomainEventBase_EventType_IsCorrect()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();

        // Act
        var domainEvent = new OrderCreatedEvent(aggregateId, "Temp");

        // Assert
        Assert.Equal("OrderCreatedEvent", domainEvent.EventType);
    }
}