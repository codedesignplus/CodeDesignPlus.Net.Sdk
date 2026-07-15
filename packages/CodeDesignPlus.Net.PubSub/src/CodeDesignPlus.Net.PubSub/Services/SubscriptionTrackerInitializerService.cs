namespace CodeDesignPlus.Net.PubSub.Services;

/// <summary>
/// Configuration object to pass the expected handler count to the initializer service.
/// </summary>
public class SubscriptionTrackerInitializer
{
    /// <summary>
    /// Gets or sets the expected number of event handler subscriptions.
    /// </summary>
    public int ExpectedCount { get; set; }
}

/// <summary>
/// Initializes the subscription tracker with the expected handler count at startup.
/// </summary>
public class SubscriptionTrackerInitializerService(
    ISubscriptionTracker tracker,
    IOptions<SubscriptionTrackerInitializer> options
) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        tracker.SetExpectedCount(options.Value.ExpectedCount);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
