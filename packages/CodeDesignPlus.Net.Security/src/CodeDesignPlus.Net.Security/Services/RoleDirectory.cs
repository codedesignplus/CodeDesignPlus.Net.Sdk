using CodeDesignPlus.Net.Cache.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Models = CodeDesignPlus.Net.Security.Abstractions.Models;

namespace CodeDesignPlus.Net.Security.Services;

/// <summary>
/// Reads the roles snapshots published by ms-users, walking L1 (in-process), the shared cache server
/// and finally <see cref="IRoleSnapshotFallback"/>.
/// </summary>
public class RoleDirectory(
    ICacheManager cacheManager,
    IMemoryCache memoryCache,
    ILogger<RoleDirectory> logger,
    IRoleSnapshotFallback fallback = null,
    TimeProvider timeProvider = null) : IRoleDirectory
{
    private readonly TimeProvider timeProvider = timeProvider ?? TimeProvider.System;

    /// <summary>
    /// How long an L1 entry is considered current before a refresh is attempted.
    /// </summary>
    /// <remarks>
    /// Invalidation is by expiry and not by event, as with the tenant directory. Making every service
    /// listen for role changes to drop a cache entry costs more than a role change being visible a
    /// minute later: roles change rarely and requests do not.
    /// </remarks>
    private static readonly TimeSpan FreshWindow = TimeSpan.FromSeconds(60);

    /// <summary>
    /// How long an L1 entry is retained so that it can still be served when every other level is
    /// unreachable. Must be comfortably longer than <see cref="FreshWindow"/>.
    /// </summary>
    private static readonly TimeSpan RetentionWindow = TimeSpan.FromMinutes(30);

    /// <inheritdoc/>
    public async Task<string[]> GetRolesAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            return [];

        var snapshot = await GetSnapshotAsync(userId, cancellationToken);

        if (snapshot is null)
        {
            // Sin instantanea se devuelve vacio y no se levanta el error: un conjunto vacio deniega, y
            // denegar de mas se ve enseguida. Inventar roles para que la peticion siga es lo unico que
            // no se puede hacer aqui.
            logger.LogWarning("No roles could be resolved for user {UserId}; treating the user as having none", userId);

            return [];
        }

        return snapshot.EffectiveIn(tenantId);
    }

    private async Task<Models.UserRoles> GetSnapshotAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cacheKey = RoleCacheKeys.Snapshot(userId);

        var now = this.timeProvider.GetUtcNow();

        if (memoryCache.TryGetValue<Entry>(cacheKey, out var entry) && now < entry.FreshUntil)
            return entry.Snapshot;

        var snapshot = await LoadAsync(userId, cacheKey, cancellationToken);

        if (snapshot is not null)
        {
            memoryCache.Set(cacheKey, new Entry(snapshot, now.Add(FreshWindow)), RetentionWindow);

            return snapshot;
        }

        // Ningun nivel respondio. Si quedaba una copia retenida se sirve vencida: se degrada la
        // frescura antes que la disponibilidad.
        if (entry is not null)
        {
            logger.LogWarning("Serving a stale roles snapshot for user {UserId}: no level could refresh it", userId);

            return entry.Snapshot;
        }

        return null;
    }

    private async Task<Models.UserRoles> LoadAsync(Guid userId, string cacheKey, CancellationToken cancellationToken)
    {
        try
        {
            var snapshot = await cacheManager.GetGlobalAsync<Models.UserRoles>(cacheKey);

            if (snapshot is not null)
                return snapshot;

            logger.LogDebug("The roles of user {UserId} are not published in the shared cache", userId);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "The shared cache could not be reached while resolving the roles of user {UserId}", userId);
        }

        if (fallback is null)
            return null;

        try
        {
            return await fallback.GetAsync(userId, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "The fallback could not resolve the roles of user {UserId}", userId);

            return null;
        }
    }

    private sealed class Entry(Models.UserRoles snapshot, DateTimeOffset freshUntil)
    {
        public Models.UserRoles Snapshot { get; } = snapshot;

        public DateTimeOffset FreshUntil { get; } = freshUntil;
    }
}
