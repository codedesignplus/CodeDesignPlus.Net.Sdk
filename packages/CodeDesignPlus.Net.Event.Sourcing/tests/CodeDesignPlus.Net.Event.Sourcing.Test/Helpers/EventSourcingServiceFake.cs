using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Helpers;

public class EventSourcingServiceFake : IEventSourcingService<Guid>
{
    public Task AppendEventAsync<TDomainEvent>(TDomainEvent @event, Metadata<Guid> metadata) where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task<long> GetAggregateVersionAsync(Guid aggregateId)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetEventStreamPositionAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(IDomainEvent, Metadata<Guid>)>> LoadEventsForAggregateAsync(Guid aggregateId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(IDomainEvent, Metadata<Guid>)>> LoadEventsFromPositionAsync(long position)
    {
        throw new NotImplementedException();
    }

    public Task<TAggregate> LoadSnapshotForAggregateAsync<TAggregate>(Guid aggregateId) where TAggregate : IAggregateRoot<Guid>
    {
        throw new NotImplementedException();
    }

    public Task SaveSnapshotAsync<TAggregate>(TAggregate aggregate) where TAggregate : IAggregateRoot<Guid>
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(TDomainEvent, Metadata<Guid>)>> SearchEventsByCategoryAsync<TDomainEvent>(string category) where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(TDomainEvent, Metadata<Guid>)>> SearchEventsByEventTypeAsync<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(IDomainEvent, Metadata<Guid>)>> SearchEventsByStreamAsync(string streamName)
    {
        throw new NotImplementedException();
    }
}
