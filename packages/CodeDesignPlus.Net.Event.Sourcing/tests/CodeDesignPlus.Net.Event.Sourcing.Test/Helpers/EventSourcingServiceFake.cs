
using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Helpers;

public class EventSourcingServiceFake : IEventSourcingService
{
    public Task AppendEventAsync<TDomainEvent>(string category, TDomainEvent @event, long? version = null, CancellationToken cancellationToken = default) where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task<long> CountEventsAsync(string category, Guid aggregateId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetVersionAsync(string category, Guid aggregateId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<IDomainEvent>> LoadEventsAsync(string category, Guid aggregateId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TAggregate> LoadSnapshotAsync<TAggregate>(string category, Guid aggregateId, CancellationToken cancellationToken = default) where TAggregate : Sourcing.Abstractions.IAggregateRoot
    {
        throw new NotImplementedException();
    }

    public Task SaveSnapshotAsync<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken = default) where TAggregate : Sourcing.Abstractions.IAggregateRoot
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<IDomainEvent>> SearchEventsAsync(string streamName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TDomainEvent>> SearchEventsAsync<TDomainEvent>(CancellationToken cancellationToken = default) where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TDomainEvent>> SearchEventsAsync<TDomainEvent>(string category, CancellationToken cancellationToken = default) where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }
}
