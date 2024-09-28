namespace CodeDesignPlus.Net.EventStore.Abstractions;

/// <summary>
/// Interface that defines the methods to be implemented by the event store service.
/// </summary>
/// <remarks>
/// This interface extends the <see cref="IEventSourcingService"/> interface to provide additional methods specific to EventStore.
/// </remarks>
public interface IEventStoreService : IEventSourcingService
{
}