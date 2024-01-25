


namespace CodeDesignPlus.Net.Core.Abstractions;

public abstract class DomainEvent(Guid eventId, string type, DateTime occurredAt, Guid id) : IDomainEvent
{
    public Guid EventId { get; private set; } = eventId;
    public string EventType { get; private set; } = type;
    public DateTime OccurredAt { get; private set; } = occurredAt;
    public Guid Id { get; private set; } = id;
}