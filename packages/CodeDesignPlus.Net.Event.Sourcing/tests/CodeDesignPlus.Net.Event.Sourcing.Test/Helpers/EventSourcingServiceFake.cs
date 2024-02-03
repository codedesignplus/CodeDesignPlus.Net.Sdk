
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Event.Sourcing.Abstractions;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Helpers;

public class EventSourcingServiceFake<TUserKey> : IEventSourcingService
{
    public Task AppendEventAsync<TDomainEvent>(TDomainEvent @event, Metadata metadata) where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task<long> CountEventsAsync(string category, Guid aggregateId)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetVersionAsync(string category, Guid aggregateId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(IDomainEvent, Metadata)>> LoadEventsAsync(string category, Guid aggregateId)
    {
        throw new NotImplementedException();
    }

    public Task<TAggregate> LoadSnapshotAsync<TAggregate>(string category, Guid aggregateId) where TAggregate : Abstractions.IAggregateRoot
    {
        throw new NotImplementedException();
    }

    public Task SaveSnapshotAsync<TAggregate>(TAggregate aggregate) where TAggregate : Abstractions.IAggregateRoot
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(IDomainEvent, Metadata)>> SearchEventsAsync(string streamName)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(TDomainEvent, Metadata)>> SearchEventsAsync<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(TDomainEvent, Metadata)>> SearchEventsAsync<TDomainEvent>(string category) where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }
}
