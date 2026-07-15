namespace CodeDesignPlus.Net.PubSub.Abstractions;

/// <summary>
/// Tracks the subscription status of event handlers.
/// </summary>
public interface ISubscriptionTracker
{
    /// <summary>
    /// Gets the total number of handlers expected to subscribe.
    /// </summary>
    int ExpectedCount { get; }

    /// <summary>
    /// Gets the number of handlers that have successfully subscribed.
    /// </summary>
    int ReadyCount { get; }

    /// <summary>
    /// Gets a value indicating whether all expected handlers are subscribed.
    /// </summary>
    bool AllReady { get; }

    /// <summary>
    /// Sets the total number of expected handler subscriptions.
    /// </summary>
    /// <param name="count">The number of expected handlers.</param>
    void SetExpectedCount(int count);

    /// <summary>
    /// Marks a handler as successfully subscribed.
    /// </summary>
    /// <param name="handlerName">The name of the handler that subscribed.</param>
    void MarkSubscribed(string handlerName);

    /// <summary>
    /// Marks a handler as failed after exhausting retries.
    /// </summary>
    /// <param name="handlerName">The name of the handler that failed.</param>
    void MarkFailed(string handlerName);

    /// <summary>
    /// Gets the list of handler names that have failed to subscribe.
    /// </summary>
    IReadOnlyCollection<string> FailedHandlers { get; }
}
