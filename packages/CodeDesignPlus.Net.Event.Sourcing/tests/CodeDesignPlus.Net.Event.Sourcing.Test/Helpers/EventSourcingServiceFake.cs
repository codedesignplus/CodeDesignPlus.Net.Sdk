using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Helpers;

public class EventSourcingServiceFake : IEventSourcingService<Guid>
{
    public Task AppendEventAsync<TDomainEvent>(TDomainEvent @event, Metadata<Guid> metadata) where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task<long> GetVersionAsync(string category, Guid aggregateId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(IDomainEvent, Metadata<Guid>)>> LoadEventsAsync(string category, Guid aggregateId)
    {
        throw new NotImplementedException();
    }

    public Task<TAggregate> LoadSnapshotAsync<TAggregate>(string category, Guid aggregateId) where TAggregate : IAggregateRoot<Guid>
    {
        throw new NotImplementedException();
    }

    public Task SaveSnapshotAsync<TAggregate>(TAggregate aggregate) where TAggregate : IAggregateRoot<Guid>
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(IDomainEvent, Metadata<Guid>)>> SearchEventsAsync(string streamName)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(TDomainEvent, Metadata<Guid>)>> SearchEventsAsync<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(TDomainEvent, Metadata<Guid>)>> SearchEventsAsync<TDomainEvent>(string category) where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }
}
