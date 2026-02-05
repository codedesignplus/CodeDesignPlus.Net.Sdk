using CodeDesignPlus.Net.gRpc.Clients.Services.Tenant;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Service to manage tenant-related operations.
/// </summary>
public interface ITenantGrpc
{
    /// <summary>
    /// Creates a new tenant.
    /// </summary>
    /// <param name="request">The request containing tenant information.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    Task CreateTenantAsync(CreateTenantRequest request, CancellationToken cancellationToken);
    /// <summary>
    /// Updates an existing tenant.
    /// </summary>
    /// <param name="request">The request containing tenant information.</param>
    /// <param name="cancellationToken"> Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    Task UpdateTenantAsync(UpdateTenantRequest request, CancellationToken cancellationToken);
    /// <summary>
    /// Deletes an existing tenant.
    /// </summary>
    /// <param name="request">The request containing tenant information.</param>
    /// <param name="cancellationToken"> Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    Task DeleteTenantAsync(DeleteTenantRequest request, CancellationToken cancellationToken);
    /// <summary>
    /// Updates an existing tenant.
    /// </summary>
    /// <param name="request">The request containing tenant information.</param>
    /// <param name="cancellationToken"> Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>Returns a task representing the asynchronous operation with the tenant information.</returns>
    Task<GetTenantResponse> GetTenantByIdAsync(GetTenantRequest request, CancellationToken cancellationToken);
}
