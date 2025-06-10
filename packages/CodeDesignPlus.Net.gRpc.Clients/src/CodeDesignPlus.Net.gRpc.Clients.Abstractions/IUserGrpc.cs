using System;
using CodeDesignPlus.Net.gRpc.Clients.Services.User;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

public interface IUserGrpc
{
    Task AddTenantToUser(AddTenantRequest request, CancellationToken cancellationToken);
    Task AddGroupToUser(AddGroupRequest request, CancellationToken cancellationToken);
}
