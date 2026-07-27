namespace CodeDesignPlus.Net.Security.Abstractions;

/// <summary>
/// Last resort source for a tenant snapshot, used when the shared cache cannot serve it.
/// Implement this in the gRPC Clients layer so the Security SDK stays transport-agnostic.
/// Registration is optional: without it, a cache miss simply yields no snapshot.
/// </summary>
public interface ITenantSnapshotFallback
{
    /// <summary>
    /// Builds the snapshot of a tenant by querying its owner service.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The snapshot, or <c>null</c> when the tenant does not exist.</returns>
    Task<Models.Tenant> GetAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
