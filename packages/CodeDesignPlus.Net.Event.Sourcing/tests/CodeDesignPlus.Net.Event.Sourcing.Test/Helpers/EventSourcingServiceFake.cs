
namespace CodeDesignPlus.Net.Event.Sourcing.Test.Helpers;

public class EventSourcingServiceFake<TUserKey> : IEventSourcingService<TUserKey>
{
    public Task AppendEventAsync<TDomainEvent>(TDomainEvent @event, Metadata<TUserKey> metadata) where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task<long> GetVersionAsync(string category, Guid aggregateId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(IDomainEvent, Metadata<TUserKey>)>> LoadEventsAsync(string category, Guid aggregateId)
    {
        throw new NotImplementedException();
    }

    public Task<TAggregate> LoadSnapshotAsync<TAggregate>(string category, Guid aggregateId) where TAggregate : IAggregateRoot<TUserKey>
    {
        throw new NotImplementedException();
    }

    public Task SaveSnapshotAsync<TAggregate>(TAggregate aggregate) where TAggregate : IAggregateRoot<TUserKey>
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(IDomainEvent, Metadata<TUserKey>)>> SearchEventsAsync(string streamName)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(TDomainEvent, Metadata<TUserKey>)>> SearchEventsAsync<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<(TDomainEvent, Metadata<TUserKey>)>> SearchEventsAsync<TDomainEvent>(string category) where TDomainEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }
}
