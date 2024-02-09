namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents the metadata of the event.
/// </summary>
/// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
public class Event<TDomainEvent> where TDomainEvent : DomainEvent
{
    /// <summary>
    /// The data of the event.
    /// </summary>
    public EventData<TDomainEvent> Data { get; private set; }
    /// <summary>
    /// The metadata of the event.
    /// </summary>
    public Dictionary<string, object> Metadata { get; private set; }    

    /// <summary>
    /// Initializes a new instance of <see cref="Event{TDomainEvent}"/>.
    /// </summary>
    /// <param name="data">The data of the event.</param>
    /// <param name="metadata">The metadata of the event.</param>
    public Event(EventData<TDomainEvent> data, Dictionary<string, object> metadata)
    {
        data.Attributes.Metadata = metadata;
        data.Attributes.EventId = data.Id;
        data.Attributes.OccurredAt = data.OccurredAt;

        this.Data = data;
        this.Metadata = metadata;
    }
}


/// <summary>
/// Represents a domain event.
/// </summary>
/// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
/// <param name="id">The identifier of the event.</param>
/// <param name="type">The type of the event.</param>
/// <param name="attributes">The attributes of the event.</param>
/// <param name="occurredAt">The date and time when the event occurred.</param>
public class EventData<TDomainEvent>(Guid id, string type, TDomainEvent attributes, DateTime occurredAt) where TDomainEvent : DomainEvent
{
    /// <summary>
    /// Initializes a new instance of <see cref="EventData{TDomainEvent}"/>.
    /// </summary>
    public Guid Id { get; private set; } = id;
    /// <summary>
    /// The type of the event.
    /// </summary>
    public string Type { get; private set; } = type;
    /// <summary>
    /// The attributes of the event.
    /// </summary>
    public TDomainEvent Attributes { get; private set; } = attributes;
    /// <summary>
    /// The date and time when the event occurred.
    /// </summary>
    public DateTime OccurredAt { get; private set; } = occurredAt;
}