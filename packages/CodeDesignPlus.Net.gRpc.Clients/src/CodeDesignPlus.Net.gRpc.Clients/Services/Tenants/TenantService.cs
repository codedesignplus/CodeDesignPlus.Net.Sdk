using CodeDesignPlus.Net.gRpc.Clients.Services.Tenant;
using Microsoft.AspNetCore.Http;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Tenants;

public class TenantService(CodeDesignPlus.Net.gRpc.Clients.Services.Tenant.Tenant.TenantClient client, IHttpContextAccessor httpContextAccessor) : ITenantGrpc
{

    public async Task CreateTenantAsync(CreateTenantRequest request, CancellationToken cancellationToken)
    {
        var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        var tenant = httpContextAccessor.HttpContext?.Request.Headers["X-Tenant"].ToString() ?? null!;

        if (string.IsNullOrEmpty(authorizationHeader))
            throw new InvalidOperationException("Authorization header is required.");

        await client.CreateTenantAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", authorizationHeader },
            { "X-Tenant", tenant }
        }, cancellationToken: cancellationToken);
    }

    public async Task<GetTenantResponse> GetTenantByIdAsync(GetTenantRequest request, CancellationToken cancellationToken)
    {
        var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        var tenant = httpContextAccessor.HttpContext?.Request.Headers["X-Tenant"].ToString() ?? null!;

        if (string.IsNullOrEmpty(authorizationHeader))
            throw new InvalidOperationException("Authorization header is required.");

        var response = await client.GetTenantAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", authorizationHeader },
            { "X-Tenant", tenant }
        }, cancellationToken: cancellationToken);

        return response;
    }

}
