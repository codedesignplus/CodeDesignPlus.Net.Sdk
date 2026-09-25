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

    /// <summary>
    /// How long "this tenant does not exist" is remembered. Without it, a client that insists on an unknown
    /// tenant hits ms-tenants on every request. Short, so that a tenant created meanwhile shows up soon.
    /// </summary>
    private static readonly TimeSpan NotFoundWindow = TimeSpan.FromSeconds(60);

    /// <inheritdoc/>
    public async Task<Models.Tenant> GetSnapshotAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var lookup = await LookupAsync(tenantId, cancellationToken);

        return lookup.Snapshot;
    }

    /// <inheritdoc/>
    public async Task<Models.TenantLookup> LookupAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty)
            return Models.TenantLookup.NotFound;

        var cacheKey = TenantCacheKeys.Snapshot(tenantId);
        var missingKey = $"{cacheKey}:missing";

        var now = this.timeProvider.GetUtcNow();

        if (memoryCache.TryGetValue<Entry>(cacheKey, out var entry) && now < entry.FreshUntil)
            return Models.TenantLookup.Found(entry.Snapshot);

        if (memoryCache.TryGetValue<DateTimeOffset>(missingKey, out var missingUntil) && now < missingUntil)
            return Models.TenantLookup.NotFound;

        var lookup = await LoadAsync(tenantId, cacheKey, cancellationToken);

        switch (lookup.Status)
        {
            case Models.TenantLookupStatus.Found:
                memoryCache.Set(cacheKey, new Entry(lookup.Snapshot, now.Add(FreshWindow)), RetentionWindow);
                memoryCache.Remove(missingKey);

                return lookup;

            case Models.TenantLookupStatus.NotFound:
                // Confirmado por ms-tenants: la copia retenida, si la habia, es de un tenant que ya no existe
                // y no se sirve. Se recuerda un minuto para no volver a preguntar en cada peticion.
                memoryCache.Remove(cacheKey);
                memoryCache.Set(missingKey, now.Add(NotFoundWindow), NotFoundWindow);

                return lookup;

            default:
                // Ningun nivel respondio. Si quedaba una copia retenida se sirve vencida: se degrada la
                // frescura antes que la disponibilidad.
                if (entry is not null)
                {
                    logger.LogWarning("Serving a stale snapshot for tenant {TenantId}: no level could refresh it", tenantId);

                    return Models.TenantLookup.Found(entry.Snapshot);
                }

                return lookup;
        }
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

    /// <summary>
    /// Walks the shared cache and then the fallback. Only the fallback can say that a tenant does not exist
    /// (its contract returns <c>null</c> for that); a miss in the cache only means it was not published.
    /// </summary>
    private async Task<Models.TenantLookup> LoadAsync(Guid tenantId, string cacheKey, CancellationToken cancellationToken)
    {
        try
        {
            var snapshot = await cacheManager.GetGlobalAsync<Models.Tenant>(cacheKey);

            if (snapshot is not null)
                return Models.TenantLookup.Found(snapshot);

            logger.LogDebug("Tenant {TenantId} is not published in the shared cache", tenantId);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "The shared cache could not be reached while resolving tenant {TenantId}", tenantId);
        }

        // Sin respaldo no hay quien confirme que el tenant no existe: es no disponible, no inexistente.
        if (fallback is null)
            return Models.TenantLookup.Unavailable;

        try
        {
            var snapshot = await fallback.GetAsync(tenantId, cancellationToken);

            return snapshot is null ? Models.TenantLookup.NotFound : Models.TenantLookup.Found(snapshot);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "The fallback could not resolve tenant {TenantId}", tenantId);

            return Models.TenantLookup.Unavailable;
        }
    }

    private sealed class Entry(Models.Tenant snapshot, DateTimeOffset freshUntil)
    {
        public Models.Tenant Snapshot { get; } = snapshot;

        public DateTimeOffset FreshUntil { get; } = freshUntil;
    }
}
