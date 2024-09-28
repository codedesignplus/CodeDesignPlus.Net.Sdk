namespace CodeDesignPlus.Net.PubSub.Abstractions;

/// <summary>
/// Base interface for implementing an event handler for a defined event.
/// </summary>
/// <typeparam name="TEvent">Integration Event</typeparam>
public interface IEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    /// <summary>
    /// Invoked by the event bus when an event to which it is subscribed is detected.
    /// </summary>
    /// <param name="data">Event information</param>
    /// <param name="token">Cancellation Token</param>
    /// <returns>A Task representing the asynchronous operation</returns>
    Task HandleAsync(TEvent data, CancellationToken token);
}