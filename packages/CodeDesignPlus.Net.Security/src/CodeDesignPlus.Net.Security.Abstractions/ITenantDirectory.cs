namespace CodeDesignPlus.Net.Security.Abstractions;

/// <summary>
/// Reads the tenant snapshots published by ms-tenants. Resolution walks three levels: an in-process
/// cache, the shared cache server, and finally <see cref="ITenantSnapshotFallback"/>. When every
/// level fails but a previously loaded snapshot is still retained in memory, the stale snapshot is
/// returned instead of an error, so that an outage of the cache server degrades freshness rather
/// than availability.
/// </summary>
public interface ITenantDirectory
{
    /// <summary>
    /// Gets the snapshot of a tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The snapshot, or <c>null</c> when the tenant cannot be resolved from any level.</returns>
    Task<Models.Tenant> GetSnapshotAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the identifiers of the active tenants. Intended for recurring jobs, which have no
    /// ambient tenant and need to iterate over all of them.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The identifiers of the active tenants.</returns>
    Task<IReadOnlyList<Guid>> GetActiveTenantsAsync(CancellationToken cancellationToken = default);
}
