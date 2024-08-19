namespace CodeDesignPlus.Net.PubSub.Abstractions;

/// <summary>
/// Interface for managing a queue of domain events.
/// </summary>
public interface IEventQueueService
{
    /// <summary>
    /// Adds an event to the end of the concurrent queue.
    /// </summary>
    /// <param name="event">The domain event to add to the queue. Can be null for reference types.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous enqueue operation.</returns>
    Task EnqueueAsync(IDomainEvent @event, CancellationToken cancellationToken);

    /// <summary>
    /// Tries to remove and return the event at the beginning of the concurrent queue.
    /// </summary>
    /// <param name="token">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous dequeue operation.</returns>
    Task DequeueAsync(CancellationToken token);
}