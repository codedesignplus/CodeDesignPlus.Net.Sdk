using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Resources;

/// <summary>
/// Resource health check to verify that the resources have been registered.
/// </summary>
public class ResourceHealtCheck : IHealthCheck
{
    private volatile bool isReady;

    /// <summary>
    /// Gets or sets a value indicating whether the resources have been registered.
    /// </summary>
    public bool RegisterResourcesCompleted
    {
        get => isReady;
        set => isReady = value;
    }

    /// <summary>
    /// Check the health of the resource.
    /// </summary>
    /// <param name="context">Context information about the current health check operation.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the health check.</param>
    /// <returns>Returns the result of the health check.</returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (RegisterResourcesCompleted)
            return Task.FromResult(HealthCheckResult.Healthy("The Resource is ready."));

        return Task.FromResult(HealthCheckResult.Unhealthy("That Resource is not ready."));
    }
}
