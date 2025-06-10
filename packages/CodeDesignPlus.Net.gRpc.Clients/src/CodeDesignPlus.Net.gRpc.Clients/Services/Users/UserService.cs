using CodeDesignPlus.Net.gRpc.Clients.Services.User;
using Microsoft.AspNetCore.Http;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Users;

public class UserService(CodeDesignPlus.Net.gRpc.Clients.Services.User.Users.UsersClient client, IHttpContextAccessor httpContextAccessor) : IUserGrpc
{
    public async Task AddTenantToUser(AddTenantRequest request, CancellationToken cancellationToken)
    {
        var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        var tenant = httpContextAccessor.HttpContext?.Request.Headers["X-Tenant"].ToString() ?? null!;

        if (string.IsNullOrEmpty(authorizationHeader))
            throw new InvalidOperationException("Authorization header is required.");

        await client.AddTenantToUserAsync(request, new Grpc.Core.Metadata
        {
            { "Authorization", authorizationHeader },
            { "X-Tenant", tenant }
        }, cancellationToken: cancellationToken);
    }

    public async Task AddGroupToUser(AddGroupRequest request, CancellationToken cancellationToken)
    {
        var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
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
