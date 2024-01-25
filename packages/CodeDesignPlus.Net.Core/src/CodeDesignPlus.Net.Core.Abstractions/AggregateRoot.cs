
namespace CodeDesignPlus.Net.Core.Abstractions;

public abstract class AggregateRoot(Guid id) : IAggregateRoot
{
    public Guid Id { get; private set; } = id;

    private List<IDomainEvent> _domainEvents = [];

    public void AddEvent(IDomainEvent @event)
    {
        _domainEvents.Add(@event);
    }

    public IReadOnlyList<IDomainEvent> GetAndClearEvents()
    {
        var domainEvents = _domainEvents;

        _domainEvents = [];

        return domainEvents.AsReadOnly();
    }
}
