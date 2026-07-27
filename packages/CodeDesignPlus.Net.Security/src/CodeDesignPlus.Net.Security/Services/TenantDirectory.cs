using CodeDesignPlus.Net.Cache.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Models = CodeDesignPlus.Net.Security.Abstractions.Models;

namespace CodeDesignPlus.Net.Security.Services;

/// <summary>
/// Reads the tenant snapshots published by ms-tenants, walking L1 (in-process), the shared cache
/// server and finally <see cref="ITenantSnapshotFallback"/>.
/// </summary>
public class TenantDirectory(
    ICacheManager cacheManager,
    IMemoryCache memoryCache,
    ILogger<TenantDirectory> logger,
    ITenantSnapshotFallback fallback = null,
    TimeProvider timeProvider = null) : ITenantDirectory
{
    private readonly TimeProvider timeProvider = timeProvider ?? TimeProvider.System;

    /// <summary>
    /// How long an L1 entry is considered current before a refresh is attempted.
    /// </summary>
    private static readonly TimeSpan FreshWindow = TimeSpan.FromSeconds(60);

    /// <summary>
    /// How long an L1 entry is retained so that it can still be served when every other level is
    /// unreachable. Must be comfortably longer than <see cref="FreshWindow"/>.
    /// </summary>
    private static readonly TimeSpan RetentionWindow = TimeSpan.FromMinutes(30);

    /// <inheritdoc/>
    public async Task<Models.Tenant> GetSnapshotAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty)
            return null;

        var cacheKey = TenantCacheKeys.Snapshot(tenantId);

        var now = this.timeProvider.GetUtcNow();

        if (memoryCache.TryGetValue<Entry>(cacheKey, out var entry) && now < entry.FreshUntil)
            return entry.Snapshot;

        var snapshot = await LoadAsync(tenantId, cacheKey, cancellationToken);

        if (snapshot is not null)
        {
            memoryCache.Set(cacheKey, new Entry(snapshot, now.Add(FreshWindow)), RetentionWindow);

            return snapshot;
        }

        // Ningun nivel respondio. Si quedaba una copia retenida se sirve vencida: se degrada la
        // frescura antes que la disponibilidad.
        if (entry is not null)
        {
            logger.LogWarning("Serving a stale snapshot for tenant {TenantId}: no level could refresh it", tenantId);

            return entry.Snapshot;
        }

        return null;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Guid>> GetActiveTenantsAsync(CancellationToken cancellationToken = default)
    {
        var members = await cacheManager.GetGlobalSetMembersAsync(TenantCacheKeys.ActiveTenants);

        if (members.Length == 0)
        {
            logger.LogWarning("The active tenants index {Key} is empty", TenantCacheKeys.ActiveTenants);

            return [];
        }

        return [.. members.Where(member => Guid.TryParse(member, out _)).Select(Guid.Parse)];
    }

    private async Task<Models.Tenant> LoadAsync(Guid tenantId, string cacheKey, CancellationToken cancellationToken)
    {
        try
        {
            var snapshot = await cacheManager.GetGlobalAsync<Models.Tenant>(cacheKey);

            if (snapshot is not null)
                return snapshot;

            logger.LogDebug("Tenant {TenantId} is not published in the shared cache", tenantId);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "The shared cache could not be reached while resolving tenant {TenantId}", tenantId);
        }

        if (fallback is null)
            return null;

        try
        {
            return await fallback.GetAsync(tenantId, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "The fallback could not resolve tenant {TenantId}", tenantId);

            return null;
        }
    }

    private sealed class Entry(Models.Tenant snapshot, DateTimeOffset freshUntil)
    {
        public Models.Tenant Snapshot { get; } = snapshot;

        public DateTimeOffset FreshUntil { get; } = freshUntil;
    }
}
