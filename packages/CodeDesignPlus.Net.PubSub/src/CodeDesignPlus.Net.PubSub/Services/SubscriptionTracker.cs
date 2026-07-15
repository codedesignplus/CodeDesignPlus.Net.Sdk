namespace CodeDesignPlus.Net.PubSub.Services;

/// <summary>
/// Tracks the subscription status of event handlers using thread-safe counters.
/// </summary>
public class SubscriptionTracker : ISubscriptionTracker
{
    private int expectedCount;
    private int readyCount;
    private readonly ConcurrentBag<string> failedHandlers = [];

    /// <inheritdoc/>
    public int ExpectedCount => expectedCount;

    /// <inheritdoc/>
    public int ReadyCount => readyCount;

    /// <inheritdoc/>
    public bool AllReady => readyCount >= expectedCount && expectedCount > 0;

    /// <inheritdoc/>
    public IReadOnlyCollection<string> FailedHandlers => failedHandlers.ToArray();

    /// <inheritdoc/>
    public void SetExpectedCount(int count)
    {
        Interlocked.Exchange(ref expectedCount, count);
    }

    /// <inheritdoc/>
    public void MarkSubscribed(string handlerName)
    {
        Interlocked.Increment(ref readyCount);
    }

    /// <inheritdoc/>
    public void MarkFailed(string handlerName)
    {
        failedHandlers.Add(handlerName);
    }
}
