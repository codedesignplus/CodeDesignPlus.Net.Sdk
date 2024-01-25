namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions;


public interface IDomainEvent
{
    Guid AggregateId { get; }
}
