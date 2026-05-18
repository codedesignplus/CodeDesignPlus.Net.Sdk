using CodeDesignPlus.Net.Security.Abstractions;
using LicenseGrpc = CodeDesignPlus.Net.Microservice.Licenses.Rest.Grpc.LicenseService;
using GetTenantLicenseRequest = CodeDesignPlus.Net.Microservice.Licenses.Rest.Grpc.GetTenantLicenseRequest;
using GetTenantLicenseResponse = CodeDesignPlus.Net.Microservice.Licenses.Rest.Grpc.GetTenantLicenseResponse;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Licenses;

/// <summary>
/// gRPC client service for license operations.
/// </summary>
/// <param name="client">The generated gRPC client.</param>
/// <param name="userContext">The user context for authentication headers.</param>
public class LicenseService(LicenseGrpc.LicenseServiceClient client, IUserContext userContext) : ILicenseGrpc
{
    /// <summary>
    /// Retrieves the immutable license snapshot for a given tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The license snapshot with modules, or null if not found.</returns>
    public async Task<GetTenantLicenseResponse> GetTenantLicenseAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await client.GetTenantLicenseAsync(
                new GetTenantLicenseRequest { TenantId = tenantId.ToString() },
                new Grpc.Core.Metadata
                {
                    { "Authorization", $"Bearer {userContext.AccessToken}" }
                },
                cancellationToken: cancellationToken);
        }
        catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return new GetTenantLicenseResponse();
        }
    }
}
