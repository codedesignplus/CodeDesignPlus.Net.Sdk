using CodeDesignPlus.Net.Event.Sourcing.Abstractions;

namespace CodeDesignPlus.Net.EventStore.Abstractions;

public interface IEventStoreService<TKey>: IEventSourcingService<TKey>
{
}
