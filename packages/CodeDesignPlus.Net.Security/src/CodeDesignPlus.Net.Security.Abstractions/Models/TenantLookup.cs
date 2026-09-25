namespace CodeDesignPlus.Net.Security.Abstractions.Models;

/// <summary>
/// How the lookup of a tenant ended.
/// </summary>
public enum TenantLookupStatus
{
    /// <summary>The snapshot was found, fresh or retained.</summary>
    Found,

    /// <summary>ms-tenants answered that the tenant does not exist.</summary>
    NotFound,

    /// <summary>No level could answer: the cache failed and ms-tenants could not be reached.</summary>
    Unavailable
}

/// <summary>
/// The result of looking up a tenant. Unlike a bare <c>null</c>, it tells a tenant that does not exist
/// apart from a tenant that could not be resolved, which callers must answer differently: the first is a
/// wrong request, the second an outage.
/// </summary>
/// <param name="Status">How the lookup ended.</param>
/// <param name="Snapshot">The snapshot when <paramref name="Status"/> is <see cref="TenantLookupStatus.Found"/>; otherwise <c>null</c>.</param>
public sealed record TenantLookup(TenantLookupStatus Status, Tenant Snapshot)
{
    /// <summary>A lookup that found the tenant.</summary>
    public static TenantLookup Found(Tenant snapshot) => new(TenantLookupStatus.Found, snapshot);

    /// <summary>A lookup that confirmed the tenant does not exist.</summary>
    public static readonly TenantLookup NotFound = new(TenantLookupStatus.NotFound, null);

    /// <summary>A lookup that could not reach any source.</summary>
    public static readonly TenantLookup Unavailable = new(TenantLookupStatus.Unavailable, null);
}
