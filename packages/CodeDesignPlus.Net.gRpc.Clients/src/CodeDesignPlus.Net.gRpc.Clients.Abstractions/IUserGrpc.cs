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
}
