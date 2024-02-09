using Newtonsoft.Json.Linq;

namespace CodeDesignPlus.Net.Core;

/// <summary>
/// Convert the <see cref="Event{TDomainEvent}"/> to <see cref="IDomainEvent"/>
/// </summary>
public class EventJsonConvert : JsonConverter<IDomainEvent>
{
    /// <summary>
    /// The settings to serialize and deserialize the <see cref="IDomainEvent"/>
    /// </summary>
    private readonly JsonSerializerSettings settings = new()
    {
        ContractResolver = new DomainEventContractResolver()
    };

    /// <summary>
    /// Determines whether this instance can convert the specified object type.
    /// </summary>
    /// <param name="reader">The Newtonsoft.Json.JsonReader to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read. If there is no existing value then null will be used.</param>
    /// <param name="hasExistingValue">The existing value has a value.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>The object value.</returns>
    public override IDomainEvent ReadJson(JsonReader reader, Type objectType, IDomainEvent existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var eventType = typeof(Event<>).MakeGenericType(objectType);

        var json = JRaw.Create(reader).ToString();

        var @event = JsonConvert.DeserializeObject(json, eventType, this.settings);

        if (@event is null)
            return null;

        var data = eventType.GetProperty("Data")?.GetValue(@event);
        var domainEvent = data?.GetType().GetProperty("Attributes")?.GetValue(data) as IDomainEvent;

        return domainEvent;
    }

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    /// <param name="writer">The Newtonsoft.Json.JsonWriter to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, IDomainEvent value, JsonSerializer serializer)
    {
        var typeData = typeof(EventData<>).MakeGenericType(value.GetType());

        var eventData = Activator.CreateInstance(typeData, value.EventId, value.EventType, value, value.OccurredAt);

        var typeEvent = typeof(Event<>).MakeGenericType(value.GetType());

        var @event = Activator.CreateInstance(typeEvent, eventData, value.Metadata);

        var json = JsonConvert.SerializeObject(@event, this.settings);

        writer.WriteRawValue(json);
    }
}
