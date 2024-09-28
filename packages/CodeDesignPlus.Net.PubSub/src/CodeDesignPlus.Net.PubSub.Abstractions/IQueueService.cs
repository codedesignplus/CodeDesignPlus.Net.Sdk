namespace CodeDesignPlus.Net.PubSub.Abstractions;

/// <summary>
/// Service that allows managing a queue of events.
/// </summary>
/// <typeparam name="TEvent">The domain event to manage.</typeparam>
public interface IQueueService<in TEvent>
    where TEvent : IDomainEvent
{
    /// <summary>
    /// Adds an object to the end of the concurrent queue.
    /// </summary>
    /// <param name="event">The object to add to the concurrent queue. The value can be a null reference (Nothing in Visual Basic) for reference types.</param>
    void Enqueue(TEvent @event);

    /// <summary>
    /// Tries to remove and return the object at the beginning of the concurrent queue.
    /// </summary>
    /// <param name="token">The cancellation token that will be assigned to the new task.</param>
    /// <returns>A task that represents the asynchronous dequeue operation.</returns>
    Task DequeueAsync(CancellationToken token);
}