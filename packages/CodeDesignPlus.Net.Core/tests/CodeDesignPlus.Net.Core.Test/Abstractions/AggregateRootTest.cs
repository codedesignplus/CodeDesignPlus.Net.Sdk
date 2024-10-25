using CodeDesignPlus.Net.Core.Test.Helpers.Domain;
using CodeDesignPlus.Net.Serializers;
using System.Reflection;

namespace CodeDesignPlus.Net.Core.Test.Abstractions;

public class AggregateRootTest
{
    [Fact]
    public void Constructor_Empty_Success()
    {
        // Arrange
        var orderAggregate = new OrderAggregate();

        // Act
        var events = orderAggregate.GetAndClearEvents();

        // Assert
        Assert.Empty(events);
    }

    [Fact]
    public async Task AggregateRoot_ValidateState_Success()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createBy = Guid.NewGuid();
        var updatedBy = Guid.NewGuid();
        var tenant = Guid.NewGuid();

        var orderAggregate = OrderAggregate.Create(id, "Test", "Test Description", 10, tenant, createBy);

        // Act
        orderAggregate.Update("Test 2", "Test Description 2", 20, updatedBy);

        await Task.Delay(100);

        // Assert
        Assert.Equal(id, orderAggregate.Id);
        Assert.Equal("Test 2", orderAggregate.Name);
        Assert.Equal("Test Description 2", orderAggregate.Description);
        Assert.Equal(20, orderAggregate.Price);
        Assert.True(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() > orderAggregate.CreatedAt);
        Assert.True(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() > orderAggregate.UpdatedAt);
        Assert.True(orderAggregate.IsActive);
        Assert.Equal(tenant, orderAggregate.Tenant);

        await Task.Delay(100);

        orderAggregate.Delete();

        await Task.Delay(100);

        Assert.True(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() > orderAggregate.UpdatedAt);

        var events = orderAggregate.GetAndClearEvents();

        Assert.Equal(3, events.Count);
        Assert.Empty(orderAggregate.GetAndClearEvents());
    }

    [Fact]
    public void Event_SerializeAndDeserialize_Success()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createBy = Guid.NewGuid();
        var tenant = Guid.NewGuid();
        var orderAggregate = OrderAggregate.Create(id, "Test", "Test Description", 10, tenant, createBy);

        // Act
        var domainEvent = orderAggregate.GetAndClearEvents()[0] as OrderCreatedDomainEvent;

        // Assert
        var json = JsonSerializer.Serialize(domainEvent);

        var @event = JsonSerializer.Deserialize<OrderCreatedDomainEvent>(json);

        Assert.NotNull(@event);
        Assert.Equal(domainEvent!.EventId, @event.EventId);
        Assert.Equal(domainEvent.OccurredAt, @event.OccurredAt);
        Assert.Equal(domainEvent.AggregateId, @event.AggregateId);
        Assert.Equal(domainEvent.Name, @event.Name);
        Assert.Equal(domainEvent.Description, @event.Description);
        Assert.Equal(domainEvent.Price, @event.Price);
        Assert.Equal(domainEvent.CreatedAt, @event.CreatedAt);
        Assert.Equal(domainEvent.UpdatedAt, @event.UpdatedAt);
        Assert.Equal(domainEvent.CreateBy, @event.CreateBy);

        foreach (var item in domainEvent.Metadata)
        {
            Assert.True(@event.Metadata.ContainsKey(item.Key));
            Assert.Equal(item.Value, @event.Metadata[item.Key]);
        }
    }

    [Fact]
    public void Aggregate_DomainEventsIsNull_ReturnEmpty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createBy = Guid.NewGuid();
        var tenant = Guid.NewGuid();

        var orderAggregate = OrderAggregate.Create(id, "Test", "Test Description", 10, tenant, createBy);

        var domainEventsField = typeof(AggregateRootBase).GetField("domainEvents", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

        domainEventsField!.SetValue(orderAggregate, null);

        // Act
        var events = orderAggregate.GetAndClearEvents();

        // Assert
        Assert.Empty(events);
    }

    [Fact]
    public void Aggregate_AddEventWhenDomainEventIsNull_Success()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createBy = Guid.NewGuid();
        var tenant = Guid.NewGuid();

        var orderAggregate = OrderAggregate.Create(id, "Test", "Test Description", 10, tenant, createBy);

        var domainEventsField = typeof(AggregateRootBase).GetField("domainEvents", BindingFlags.NonPublic | BindingFlags.Instance);
        domainEventsField!.SetValue(orderAggregate, null);

        // Act
        orderAggregate.Delete();

        // Assert
        var events = orderAggregate.GetAndClearEvents();

        Assert.NotEmpty(events);
    }

}