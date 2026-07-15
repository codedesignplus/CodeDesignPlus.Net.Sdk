using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CodeDesignPlus.Net.RabbitMQ.Services;

/// <summary>
/// Health check that verifies all event handler subscriptions are active.
/// Reports unhealthy if any handler failed to subscribe after retries.
/// </summary>
public class SubscriptionReadinessHealthCheck(ISubscriptionTracker tracker) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (tracker.ExpectedCount == 0)
            return Task.FromResult(HealthCheckResult.Healthy("No event handlers registered."));

        var failedHandlers = tracker.FailedHandlers;

        if (failedHandlers.Count > 0)
        {
            var description = $"{failedHandlers.Count} handler(s) failed to subscribe: {string.Join(", ", failedHandlers)}";
            return Task.FromResult(HealthCheckResult.Unhealthy(description));
        }

        if (tracker.AllReady)
            return Task.FromResult(HealthCheckResult.Healthy($"All {tracker.ReadyCount}/{tracker.ExpectedCount} consumers subscribed."));

        var pending = tracker.ExpectedCount - tracker.ReadyCount;
        return Task.FromResult(HealthCheckResult.Degraded($"{tracker.ReadyCount}/{tracker.ExpectedCount} consumers subscribed. {pending} pending."));
    }
}
