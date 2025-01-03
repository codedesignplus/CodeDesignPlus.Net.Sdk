namespace CodeDesignPlus.Net.EventStore.Abstractions;

/// <summary>
/// Interface that defines the methods to be implemented by the event store service.
/// </summary>
/// <remarks>
/// This interface extends the <see cref="IEventSourcing"/> interface to provide additional methods specific to EventStore.
/// </remarks>
public interface IEventStore : IEventSourcing
{
}