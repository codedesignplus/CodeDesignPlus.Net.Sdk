namespace CodeDesignPlus.Net.Core.Abstractions;

public interface IAggregateRoot
{
    Guid Id { get; }
    void AddEvent(IDomainEvent @event);
    IReadOnlyList<IDomainEvent> GetAndClearEvents();
}