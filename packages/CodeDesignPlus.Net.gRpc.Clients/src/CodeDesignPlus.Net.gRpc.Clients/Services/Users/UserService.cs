using CodeDesignPlus.Net.gRpc.Clients.Services.User;
using Microsoft.AspNetCore.Http;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Users;

/// <summary>
/// Service to manage user-related operations.
/// </summary>
/// <param name="client">The gRPC client for user operations.</param>
/// <param name="httpContextAccessor">The HTTP context accessor to retrieve request information.</param>
public class UserService(CodeDesignPlus.Net.gRpc.Clients.Services.User.Users.UsersClient client, IHttpContextAccessor httpContextAccessor) : IUserGrpc
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
        var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        var tenant = httpContextAccessor.HttpContext?.Request.Headers["X-Tenant"].ToString() ?? null!;

        if (string.IsNullOrEmpty(authorizationHeader))
            throw new InvalidOperationException("Authorization header is required.");

        await client.AddTenantToUserAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", authorizationHeader },
            { "X-Tenant", tenant }
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
        var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        var tenant = httpContextAccessor.HttpContext?.Request.Headers["X-Tenant"].ToString() ?? null!;

        if (string.IsNullOrEmpty(authorizationHeader))
            throw new InvalidOperationException("Authorization header is required.");

        await client.AddGroupToUserAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", authorizationHeader },
            { "X-Tenant", tenant }
        }, cancellationToken: cancellationToken);
    }

}
