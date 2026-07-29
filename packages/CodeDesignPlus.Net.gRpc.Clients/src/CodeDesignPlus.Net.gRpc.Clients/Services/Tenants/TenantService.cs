using CodeDesignPlus.Net.gRpc.Clients.Services.Tenant;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.AspNetCore.Http;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Tenants;

/// <summary>
/// Service to manage tenant-related operations.
/// </summary>
/// <param name="client">The gRPC client for tenant operations.</param>
/// <param name="userContext">The user context to access user-related information.</param>
public class TenantService(Tenant.Tenant.TenantClient client, IUserContext userContext) : ITenantGrpc
{
    /// <summary>
    /// Deadline applied to the read operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Only the reads carry it, and on purpose: they sit on the critical path of <b>every</b>
    /// authenticated request, because <c>TenantContextMiddleware</c> resolves the tenant before
    /// anything else runs. A gRPC call without a deadline waits forever, so one bad minute in
    /// ms-tenants turned into six-minute requests across the whole platform.
    /// </para>
    /// <para>
    /// Three seconds is generous for a lookup that is normally served from cache, and short enough
    /// that the user gets an error instead of a spinner. The writes are left alone: they are
    /// user-initiated, one at a time, and a slow one is not the same kind of problem.
    /// </para>
    /// </remarks>
    private static readonly TimeSpan ReadDeadline = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Creates a new tenant.
    /// </summary>
    /// <param name="request">The request containing tenant information.</param>
    /// <param name="cancellationToken"> Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception>
    public async Task CreateTenantAsync(CreateTenantRequest request, CancellationToken cancellationToken)
    {
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
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception
    public async Task UpdateTenantAsync(UpdateTenantRequest request, CancellationToken cancellationToken)
    {
        await client.UpdateTenantAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", $"Bearer {userContext.AccessToken}" },
            { "X-Tenant", userContext.Tenant.ToString() }
        }, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes an existing tenant.
    /// </summary>
    /// <param name="request">The request containing tenant information.</param>
    /// <param name="cancellationToken"> Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the authorization header is missing.</exception
    public async Task DeleteTenantAsync(DeleteTenantRequest request, CancellationToken cancellationToken)
    {
        await client.DeleteTenantAsync(request, new Grpc.Core.Metadata
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
        var response = await client.GetTenantAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", $"Bearer {userContext.AccessToken}" },
            { "X-Tenant", userContext.Tenant.ToString() }
        }, deadline: DateTime.UtcNow.Add(ReadDeadline), cancellationToken: cancellationToken);

        return response;
    }

    /// <summary>
    /// Checks if a tenant exists.
    /// </summary>
    /// <param name="id">The ID of the tenant.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation with a boolean indicating if the tenant exists.</returns>
    public async Task<bool> ExistTenantAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = new ExistTenantRequest
        {
            Id = id.ToString()
        };

        var response = await client.ExistTenantAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", $"Bearer {userContext.AccessToken}" },
            { "X-Tenant", userContext.Tenant.ToString() }
        }, deadline: DateTime.UtcNow.Add(ReadDeadline), cancellationToken: cancellationToken);

        return response.Value;
    }

}
