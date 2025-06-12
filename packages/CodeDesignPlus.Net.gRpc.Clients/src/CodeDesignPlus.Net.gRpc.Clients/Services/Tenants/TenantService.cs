using CodeDesignPlus.Net.gRpc.Clients.Services.Tenant;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.AspNetCore.Http;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Tenants;

/// <summary>
/// Service to manage tenant-related operations.
/// </summary>
/// <param name="client">The gRPC client for tenant operations.</param>
/// <param name="userContext">The user context to access user-related information.</param>
/// <param name="logger">The logger for logging operations.</param>
public class TenantService(CodeDesignPlus.Net.gRpc.Clients.Services.Tenant.Tenant.TenantClient client, IUserContext userContext, ILogger<TenantService> logger) : ITenantGrpc
{
    /// <summary>
    /// Creates a new tenant.
    /// </summary>
    /// <param name="request">The request containing tenant information.</param>
    /// <param name="cancellationToken"> Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    public async Task CreateTenantAsync(CreateTenantRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating tenant for user {UserId} with Tenant {TenantId}", userContext.IdUser, userContext.Tenant);

        await client.CreateTenantAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", $"Bearer {userContext.AccessToken}" },
            { "X-Tenant", userContext.Tenant.ToString() }
        }, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing tenant.
    /// </summary>
    /// <param name="request">The request containing tenant information.</param>
    /// <param name="cancellationToken"> Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation with the tenant information.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    public async Task<GetTenantResponse> GetTenantByIdAsync(GetTenantRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving tenant for user {UserId} with Tenant {TenantId}", userContext.IdUser, userContext.Tenant);
        
        var response = await client.GetTenantAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", $"Bearer {userContext.AccessToken}" },
            { "X-Tenant", userContext.Tenant.ToString() }
        }, cancellationToken: cancellationToken);

        return response;
    }

}
