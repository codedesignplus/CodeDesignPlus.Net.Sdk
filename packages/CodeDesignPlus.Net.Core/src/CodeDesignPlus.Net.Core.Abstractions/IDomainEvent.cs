namespace CodeDesignPlus.Net.Core.Abstractions;

public interface IDomainEvent
{
    public Guid EventId { get; }
    public string EventType { get; }
    public DateTime OccurredAt { get; }
    public Guid Id { get; }
}
