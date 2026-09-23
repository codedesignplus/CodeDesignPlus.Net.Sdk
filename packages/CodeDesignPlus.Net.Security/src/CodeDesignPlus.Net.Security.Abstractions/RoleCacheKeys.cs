namespace CodeDesignPlus.Net.Security.Abstractions;

/// <summary>
/// Keys used to publish and read the roles snapshot of a user. They live in a global scope, outside
/// the per-application namespace, so that the snapshot published by ms-users is visible to every
/// service pointing at the same cache server.
/// </summary>
public static class RoleCacheKeys
{
    /// <summary>
    /// Prefix shared by every roles key.
    /// </summary>
    public const string Prefix = "CodeDesignPlus:Shared:";

    /// <summary>
    /// Builds the key of the roles snapshot of a given user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The key of the snapshot.</returns>
    /// <remarks>
    /// The key is per user and not per user and tenant: one write covers every tenant of that user,
    /// and switching tenant costs nothing.
    /// </remarks>
    public static string Snapshot(Guid userId) => $"{Prefix}UserRoles:{userId}";
}
