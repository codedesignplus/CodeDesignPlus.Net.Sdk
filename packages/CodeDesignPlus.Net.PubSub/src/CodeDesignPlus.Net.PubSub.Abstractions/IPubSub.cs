namespace CodeDesignPlus.Net.PubSub.Abstractions;

/// <summary>
/// Interface for publishing domain events.
/// </summary>
public interface IPubSub
{
    /// <summary>
    /// Publishes a single domain event asynchronously.
    /// </summary>
    /// <param name="event">The domain event to publish.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken);

    /// <summary>
    /// Publishes a list of domain events asynchronously.
    /// </summary>
    /// <param name="event">The list of domain events to publish.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken);
}