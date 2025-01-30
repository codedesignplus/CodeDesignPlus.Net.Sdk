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
        var createdAt = SystemClock.Instance.GetCurrentInstant();
        var createBy = Guid.NewGuid();
        var updatedAt = SystemClock.Instance.GetCurrentInstant();
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
        Assert.Equal(name, domainEvent.Name);
        Assert.Equal(description, domainEvent.Description);
        Assert.Equal(price, domainEvent.Price);
        Assert.Equal(createdAt, domainEvent.CreatedAt);
        Assert.Equal(createBy, domainEvent.CreateBy);
        Assert.Equal(updatedAt, domainEvent.UpdatedAt);
        Assert.True(SystemClock.Instance.GetCurrentInstant() > domainEvent.OccurredAt);
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
        var createdAt = SystemClock.Instance.GetCurrentInstant();
        var createBy = Guid.NewGuid();
        var updatedAt = SystemClock.Instance.GetCurrentInstant();
        var eventId = Guid.NewGuid();
        var occurredAt = SystemClock.Instance.GetCurrentInstant();
        var metadata = new Dictionary<string, object>
        {
            { "key", "value" }
        };

        // Act
        var domainEvent = new OrderCreatedDomainEvent(aggregateId, name, description, price, createdAt, createBy, updatedAt, eventId, occurredAt, metadata);

        // Assert
        Assert.Equal(eventId, domainEvent.EventId);
        Assert.Equal(aggregateId, domainEvent.AggregateId);
        Assert.Equal(name, domainEvent.Name);
        Assert.Equal(description, domainEvent.Description);
        Assert.Equal(price, domainEvent.Price);
        Assert.Equal(createdAt, domainEvent.CreatedAt);
        Assert.Equal(createBy, domainEvent.CreateBy);
        Assert.Equal(updatedAt, domainEvent.UpdatedAt);
        Assert.Equal(occurredAt, domainEvent.OccurredAt);
        Assert.Equal(metadata, domainEvent.Metadata);
    }
}