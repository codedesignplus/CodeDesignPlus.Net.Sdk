
using System.Diagnostics.CodeAnalysis;

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
        var updatedAt = DateTime.UtcNow;
        var deleteAt = DateTime.UtcNow;
        var orderAggregate = OrderAggregate.Create(id, "Test", "Test Description", 10, createAt);

        // Act
        var events = orderAggregate.GetAndClearEvents();

        foreach (var domainEvent in events)
        {

            // Assert
            var settings = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new EventJsonConvert() }
            };

            var json = JsonSerializer.Serialize(@domainEvent, settings);

            var @event2 = JsonSerializer.Deserialize<OrderCreatedDomainEvent>(json, settings);

        }
    }

}

/// <summary>
/// Convert the <see cref="Event{TDomainEvent}"/> to <see cref="IDomainEvent"/>
/// </summary>
public class EventJsonConvert : JsonConverter<IDomainEvent>
{
    /// <summary>
    /// The settings to serialize and deserialize the <see cref="IDomainEvent"/>
    /// </summary>
    private readonly JsonSerializerOptions settings = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true
    };

    /// <summary>
    /// Determines whether the specified object can be converted to the current type.
    /// </summary>
    /// <param name="objectType">The type of the object to convert.</param>
    /// <returns>true if the specified object can be converted to the current type; otherwise, false.</returns>
    public override bool CanConvert(Type objectType)
    {
        return typeof(IDomainEvent).IsAssignableFrom(objectType);
    }

    /// <summary>
    /// Reads and converts the JSON to the <see cref="IDomainEvent"/>
    /// </summary>
    /// <param name="reader">The reader to use</param>
    /// <param name="typeToConvert">The type to convert</param>
    /// <param name="options">The options to use</param>
    /// <returns>The <see cref="IDomainEvent"/> converted</returns>
    public override IDomainEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var eventType = typeof(EventData<>).MakeGenericType(typeToConvert);

        var @event = JsonSerializer.Deserialize(ref reader, eventType, this.settings);

        if (@event is null)
            return null;

        var domainEvent = (IDomainEvent)@event.GetType().GetProperty("Attributes")?.GetValue(@event)!;

        return domainEvent;
    }

    /// <summary>
    /// Writes the <see cref="IDomainEvent"/> to the JSON
    /// </summary>
    /// <param name="writer">The writer to use</param>
    /// <param name="value">The value to write</param>
    /// <param name="options">The options to use</param>
    public override void Write(Utf8JsonWriter writer, IDomainEvent value, JsonSerializerOptions options)
    {
        var type = typeof(EventData<>).MakeGenericType(value.GetType());

        var @event = Activator.CreateInstance(type, value.EventId, value.GetEventType(), value, value.OccurredAt);

        var json = JsonSerializer.Serialize(@event, this.settings);

        writer.WriteRawValue(json);
    }
}
