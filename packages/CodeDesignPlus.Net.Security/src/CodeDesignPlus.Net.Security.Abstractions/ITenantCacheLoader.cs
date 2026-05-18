namespace CodeDesignPlus.Net.Security.Abstractions;

/// <summary>
/// Loads and caches the full tenant snapshot (location + license + modules) on first access.
/// Implement this in the gRPC Clients layer so the Security SDK stays transport-agnostic.
/// </summary>
public interface ITenantCacheLoader
{
    /// <summary>
    /// Ensures the tenant snapshot exists in the cache, loading it from upstream services if missing.
    /// Idempotent: safe to call on every request.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task EnsureCachedAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
