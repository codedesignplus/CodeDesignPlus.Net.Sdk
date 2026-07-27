namespace CodeDesignPlus.Net.Security.Abstractions;

/// <summary>
/// Keys used to publish and read the tenant snapshot. They live in a global scope, outside the
/// per-application namespace, so that the snapshot published by ms-tenants is visible to every
/// service pointing at the same cache server.
/// </summary>
public static class TenantCacheKeys
{
    /// <summary>
    /// Prefix shared by every tenant key.
    /// </summary>
    public const string Prefix = "CodeDesignPlus:Shared:";

    /// <summary>
    /// Key of the set holding the identifiers of the active tenants.
    /// </summary>
    public const string ActiveTenants = Prefix + "Tenants:Active";

    /// <summary>
    /// Builds the key of the snapshot of a given tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <returns>The key of the snapshot.</returns>
    public static string Snapshot(Guid tenantId) => $"{Prefix}Tenant:{tenantId}";
}
