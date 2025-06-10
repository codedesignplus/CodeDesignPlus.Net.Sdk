using System;
using CodeDesignPlus.Net.gRpc.Clients.Services.Tenant;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

public interface ITenantGrpc
{
    Task CreateTenantAsync(CreateTenantRequest request, CancellationToken cancellationToken);
    Task<GetTenantResponse> GetTenantByIdAsync(GetTenantRequest request, CancellationToken cancellationToken);
}
