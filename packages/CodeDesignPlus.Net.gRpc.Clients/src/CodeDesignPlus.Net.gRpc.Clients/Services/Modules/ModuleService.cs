using CodeDesignPlus.Net.Microservice.Modules.gRpc;
using CodeDesignPlus.Net.Security.Abstractions;
using ModuleGrpc = CodeDesignPlus.Net.Microservice.Modules.gRpc.Module;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Modules;

/// <summary>
/// gRPC client service for module operations.
/// </summary>
public class ModuleClientService(ModuleGrpc.ModuleClient client, IUserContext userContext) : IModuleGrpc
{
    private Grpc.Core.Metadata GetHeaders() => new()
    {
        { "Authorization", $"Bearer {userContext.AccessToken}" },
        { "X-Tenant", userContext.Tenant.ToString() }
    };

    public async Task CreateModuleAsync(CreateModuleRequest request, CancellationToken cancellationToken = default)
    {
        await client.CreateModuleAsync(request, GetHeaders(), cancellationToken: cancellationToken);
    }

    public async Task UpdateModuleAsync(UpdateModuleRequest request, CancellationToken cancellationToken = default)
    {
        await client.UpdateModuleAsync(request, GetHeaders(), cancellationToken: cancellationToken);
    }

    public async Task DeleteModuleAsync(string id, CancellationToken cancellationToken = default)
    {
        await client.DeleteModuleAsync(new DeleteModuleRequest { Id = id }, GetHeaders(), cancellationToken: cancellationToken);
    }

    public async Task<GetModuleResponse> GetModuleByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await client.GetModuleByIdAsync(new GetModuleByIdRequest { Id = id }, GetHeaders(), cancellationToken: cancellationToken);
    }

    public async Task<GetAllModulesResponse> GetAllModulesAsync(string filters = null, int limit = 100, CancellationToken cancellationToken = default)
    {
        var request = new GetAllModulesRequest
        {
            Filters = filters ?? string.Empty,
            Limit = limit
        };

        return await client.GetAllModulesAsync(request, GetHeaders(), cancellationToken: cancellationToken);
    }

    public async Task AddServiceAsync(AddServiceRequest request, CancellationToken cancellationToken = default)
    {
        await client.AddServiceAsync(request, GetHeaders(), cancellationToken: cancellationToken);
    }

    public async Task RemoveServiceAsync(string moduleId, string serviceId, CancellationToken cancellationToken = default)
    {
        await client.RemoveServiceAsync(new RemoveServiceRequest
        {
            ModuleId = moduleId,
            ServiceId = serviceId
        }, GetHeaders(), cancellationToken: cancellationToken);
    }
}
