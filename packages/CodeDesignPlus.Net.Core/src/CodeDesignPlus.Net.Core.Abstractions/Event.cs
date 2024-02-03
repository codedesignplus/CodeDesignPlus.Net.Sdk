namespace CodeDesignPlus.Net.Core.Abstractions;

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