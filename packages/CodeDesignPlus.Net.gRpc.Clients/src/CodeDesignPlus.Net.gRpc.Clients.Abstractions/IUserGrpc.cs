using CodeDesignPlus.Net.gRpc.Clients.Services.User;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Service to manage user-related operations.
/// </summary>
public interface IUserGrpc
{
    /// <summary>
    /// Asociates a user with a tenant
    /// </summary>
    /// <param name="request">The request containing user and tenant information.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    Task AddTenantToUser(AddTenantRequest request, CancellationToken cancellationToken);
    /// <summary>
    /// Associates a user with a group
    /// </summary>
    /// <param name="request">The request containing user and group information.</param>
    /// <param name="cancellationToken"> Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    Task AddGroupToUser(AddGroupRequest request, CancellationToken cancellationToken);
    /// <summary>
    /// Removes a user from a group
    /// </summary>
    /// <param name="request">The request containing user and group information.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    Task RemoveGroupFromUser(RemoveGroupRequest request, CancellationToken cancellationToken);
    /// <summary>
    /// Gets every role of a user, both the platform ones and those of each tenant.
    /// </summary>
    /// <remarks>
    /// Last resort for <c>IRoleDirectory</c>, used when the shared cache cannot serve the snapshot.
    /// It returns the whole map and not the roles of a single tenant so that a user switching tenant
    /// within a session does not cause one call per switch.
    /// </remarks>
    /// <param name="request">The request containing the user identifier.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The roles of the user, or <c>null</c> when the user does not exist.</returns>
    Task<GetUserRolesResponse> GetUserRoles(GetUserRolesRequest request, CancellationToken cancellationToken);
}
