namespace CodeDesignPlus.Net.Security.Abstractions;

/// <summary>
/// Last resort source for a user's roles, used when the shared cache cannot serve the snapshot.
/// Implement this in the gRPC Clients layer so the Security SDK stays transport-agnostic.
/// Registration is optional: without it, a cache miss simply yields no roles.
/// </summary>
public interface IRoleSnapshotFallback
{
    /// <summary>
    /// Builds the roles snapshot of a user by querying its owner service.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The snapshot, or <c>null</c> when the user does not exist.</returns>
    Task<Models.UserRoles> GetAsync(Guid userId, CancellationToken cancellationToken = default);
}
