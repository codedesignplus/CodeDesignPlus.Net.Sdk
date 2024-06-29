using CodeDesignPlus.Net.Core.Test.Helpers.Domain;

namespace CodeDesignPlus.Net.Core.Test.Abstractions;

public class DomainEventTest
{
    [Fact]
    public void DomainEvent_DefaultInitialization_Success()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();
        var name = "Name";
        var description = "Description";
        var price = 100;
        var createdAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var createBy = Guid.NewGuid();
        var updatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var metadata = new Dictionary<string, object>
        {
            { "key", "value" }
        };

        // Act
        var domainEvent = new OrderCreatedDomainEvent(aggregateId, name, description, price, createdAt, createBy, updatedAt, metadata: metadata);

        // Assert
        Assert.NotEqual(Guid.Empty, domainEvent.EventId);
        Assert.NotEqual(Guid.Empty, domainEvent.EventId);
        Assert.Equal(aggregateId, domainEvent.AggregateId);
        Assert.Equal("v1.orderaggregate.created", domainEvent.EventType);
        Assert.Equal(name, domainEvent.Name);
        Assert.Equal(description, domainEvent.Description);
        Assert.Equal(price, domainEvent.Price);
        Assert.Equal(createdAt, domainEvent.CreatedAt);
        Assert.Equal(createBy, domainEvent.CreateBy);
        Assert.Equal(updatedAt, domainEvent.UpdatedAt);
        Assert.True(DateTime.UtcNow > domainEvent.OccurredAt);
        Assert.Equal(metadata, domainEvent.Metadata);
    }

    [Fact]
    public void DomainEvent_CustomInitialization_Success()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();
        var name = "Name";
        var description = "Description";
        var price = 100;
        var createdAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var createBy = Guid.NewGuid();
        var updatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var eventId = Guid.NewGuid();
        var occurredAt = DateTime.UtcNow;
        var metadata = new Dictionary<string, object>
        {
            { "key", "value" }
        };

        // Act
        var domainEvent = new OrderCreatedDomainEvent(aggregateId, name, description, price, createdAt, createBy, updatedAt, eventId, occurredAt, metadata);

        // Assert
        Assert.Equal(eventId, domainEvent.EventId);
        Assert.Equal(aggregateId, domainEvent.AggregateId);
        Assert.Equal("v1.orderaggregate.created", domainEvent.EventType);
        Assert.Equal(name, domainEvent.Name);
        Assert.Equal(description, domainEvent.Description);
        Assert.Equal(price, domainEvent.Price);
        Assert.Equal(createdAt, domainEvent.CreatedAt);
        Assert.Equal(createBy, domainEvent.CreateBy);
        Assert.Equal(updatedAt, domainEvent.UpdatedAt);
        Assert.Equal(occurredAt, domainEvent.OccurredAt);
        Assert.Equal(metadata, domainEvent.Metadata);
    }

    [Fact]
    public void EventType_AttributeNotExist_ThrowCoreException()
    {
        // Arrange
        var aggregateId = Guid.NewGuid();
        var @event = new DaminEventWithEventKey(aggregateId);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => @event.EventType);

        Assert.Equal("The event DaminEventWithEventKey does not have the EventKeyAttribute attribute.", exception.Message);
    }
}