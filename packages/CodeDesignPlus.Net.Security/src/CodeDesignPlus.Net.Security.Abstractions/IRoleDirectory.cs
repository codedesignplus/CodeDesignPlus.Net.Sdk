namespace CodeDesignPlus.Net.Security.Abstractions;

/// <summary>
/// Resolves the roles a user has <b>in a given tenant</b>. Resolution walks three levels: an in-process
/// cache, the shared cache server, and finally <see cref="IRoleSnapshotFallback"/>. When every level
/// fails but a previously loaded snapshot is still retained in memory, the stale snapshot is used
/// instead of an error, so that an outage of the cache server degrades freshness rather than
/// availability.
/// </summary>
/// <remarks>
/// <b>This is not the same as <c>IUserContext.Roles</c>, and the difference matters.</b> That property
/// reads the <c>groups</c> claim, which the identity provider fills with every group of the user across
/// the whole directory: the provider does not know what a tenant is. Deciding anything per tenant from
/// that claim grants someone who administers one tenant the same powers in every other tenant they
/// belong to.
/// <para>
/// The claim remains useful for the frontend, which paints menus before knowing which tenant is being
/// looked at. Every decision the backend makes goes through here instead.
/// </para>
/// </remarks>
public interface IRoleDirectory
{
    /// <summary>
    /// Gets the roles of a user in a tenant, as identifiers of the identity provider groups.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="tenantId">The tenant the roles are asked for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The effective roles, which include the platform ones. Empty when the user cannot be resolved
    /// from any level: an empty set denies, it never grants.
    /// </returns>
    Task<string[]> GetRolesAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
}
