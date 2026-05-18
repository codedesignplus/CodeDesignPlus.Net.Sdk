using CodeDesignPlus.Net.Microservice.Licenses.Rest.Grpc;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Abstraction for the License gRPC client.
/// Used by the Security SDK LicenseMiddleware to retrieve the immutable
/// license snapshot (including modules) for a given tenant.
/// </summary>
public interface ILicenseGrpc
{
    /// <summary>
    /// Returns the immutable license snapshot purchased by the tenant,
    /// including the list of modules active at the time of purchase.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The license snapshot with modules, or null if not found.</returns>
    Task<GetTenantLicenseResponse> GetTenantLicenseAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
