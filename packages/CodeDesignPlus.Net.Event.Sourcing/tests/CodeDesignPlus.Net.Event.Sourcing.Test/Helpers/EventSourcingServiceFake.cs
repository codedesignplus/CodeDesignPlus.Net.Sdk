using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Helpers;

public class EventSourcingServiceFake : IEventSourcingService
{
    public Task AppendEventAsync<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task AppendEventAsync<TDomainEvent, TMetadata>(TDomainEvent @event)
        where TDomainEvent : IDomainEvent<TMetadata>
        where TMetadata : class
    {
        throw new NotImplementedException();
    }

    public Task<int> GetAggregateVersionAsync(Guid aggregateId)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetEventStreamPositionAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<IDomainEvent>> LoadEventsForAggregateAsync(Guid aggregateId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TDomainEvent>> LoadEventsForAggregateAsync<TDomainEvent, TMetadata>(Guid aggregateId)
        where TDomainEvent : IDomainEvent<TMetadata>
        where TMetadata : class
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TDomainEvent>> LoadEventsFromPositionAsync<TDomainEvent>(long position) where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TDomainEvent>> LoadEventsFromPositionAsync<TDomainEvent, TMetadata>(long position)
        where TDomainEvent : IDomainEvent<TMetadata>
        where TMetadata : class
    {
        throw new NotImplementedException();
    }

    public Task<TAggregate> LoadSnapshotForAggregateAsync<TAggregate, TKey>(Guid aggregateId) where TAggregate : IAggregateRoot<TKey>
    {
        throw new NotImplementedException();
    }

    public Task<TAggregate> LoadSnapshotForAggregateAsync<TAggregate, TKey, TUserKey>(Guid aggregateId) where TAggregate : IAggregateRoot<TKey, TUserKey>
    {
        throw new NotImplementedException();
    }

    public Task SaveSnapshotAsync<TAggregate, TKey>(TAggregate aggregate) where TAggregate : IAggregateRoot<TKey>
    {
        throw new NotImplementedException();
    }

    public Task SaveSnapshotAsync<TAggregate, TKey, TUserKey>(TAggregate aggregate) where TAggregate : IAggregateRoot<TKey, TUserKey>
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TDomainEvent>> SearchEventsByCategoryAsync<TDomainEvent>(string category) where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TDomainEvent>> SearchEventsByEventTypeAsync<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<IDomainEvent>> SearchEventsByStreamAsync(string streamName)
    {
        throw new NotImplementedException();
    }
}
