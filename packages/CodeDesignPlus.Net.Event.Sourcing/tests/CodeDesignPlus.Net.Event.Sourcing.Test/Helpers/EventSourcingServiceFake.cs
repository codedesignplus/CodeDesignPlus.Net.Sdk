using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Helpers;

public class EventSourcingServiceFake : IEventSourcingService
{
    public void AppendEvent(IDomainEvent @event)
    {
        throw new NotImplementedException();
    }

    public void AppendEvent<TMetadata>(IDomainEvent<TMetadata> @event) where TMetadata : class
    {
        throw new NotImplementedException();
    }

    public int GetAggregateVersion(Guid aggregateId)
    {
        throw new NotImplementedException();
    }

    public long GetEventStreamPosition()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IDomainEvent> LoadEventsForAggregate(Guid aggregateId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IDomainEvent<TMetadata>> LoadEventsForAggregate<TMetadata>(Guid aggregateId) where TMetadata : class
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IDomainEvent> LoadEventsFromPosition(long position)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IDomainEvent<TMetadata>> LoadEventsFromPosition<TMetadata>(long position) where TMetadata : class
    {
        throw new NotImplementedException();
    }

    public TAggregate LoadSnapshotForAggregate<TAggregate>(Guid aggregateId) where TAggregate : IAggregateRoot
    {
        throw new NotImplementedException();
    }

    public TAggregate LoadSnapshotForAggregate<TAggregate, TKey, TUserKey>(Guid aggregateId) where TAggregate : IAggregateRoot<TKey, TUserKey>
    {
        throw new NotImplementedException();
    }

    public void SaveSnapshot<TAggregate>(TAggregate aggregate) where TAggregate : IAggregateRoot
    {
        throw new NotImplementedException();
    }

    public void SaveSnapshot<TAggregate, TKey, TUserKey>(TAggregate aggregate) where TAggregate : IAggregateRoot<TKey, TUserKey>
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IDomainEvent> SearchEvents(object criteria)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IDomainEvent<TMetadata>> SearchEvents<TMetadata>(object criteria) where TMetadata : class
    {
        throw new NotImplementedException();
    }
}
