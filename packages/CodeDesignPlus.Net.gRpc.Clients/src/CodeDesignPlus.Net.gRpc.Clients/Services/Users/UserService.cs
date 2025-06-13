using CodeDesignPlus.Net.gRpc.Clients.Services.User;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.AspNetCore.Http;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Users;

/// <summary>
/// Service to manage user-related operations.
/// </summary>
/// <param name="client">The gRPC client for user operations.</param>
/// <param name="userContext">The user context to access user-related information.</param>
public class UserService(User.Users.UsersClient client, IUserContext userContext) : IUserGrpc
{
    /// <summary>
    /// Asociates a user with a tenant.
    /// </summary>
    /// <param name="request">The request containing user and tenant information.</param>
    /// <param name="cancellationToken"> Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    public async Task AddTenantToUser(AddTenantRequest request, CancellationToken cancellationToken)
    {
        await client.AddTenantToUserAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", $"Bearer {userContext.AccessToken}" },
            { "X-Tenant", userContext.Tenant.ToString() }
        }, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Adds a group to a user.
    /// </summary>
    /// <param name="request">The request containing user and group information.</param>
    /// <param name="cancellationToken"> Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    public async Task AddGroupToUser(AddGroupRequest request, CancellationToken cancellationToken)
    {
        await client.AddGroupToUserAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", $"Bearer {userContext.AccessToken}" },
            { "X-Tenant", userContext.Tenant.ToString() }
        }, cancellationToken: cancellationToken);
    }

}
