using CodeDesignPlus.Net.gRpc.Clients.Abstractions;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.Extensions.Logging;
using Models = CodeDesignPlus.Net.Security.Abstractions.Models;
using UserProto = CodeDesignPlus.Net.gRpc.Clients.Services.User;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Cache;

/// <summary>
/// Builds the roles snapshot of a user straight from ms-users, used when the shared cache cannot
/// serve it. ms-users owns which roles a user holds in each tenant, so no other service is queried.
/// </summary>
public class RoleSnapshotFallback(IUserGrpc userGrpc, ILogger<RoleSnapshotFallback> logger) : IRoleSnapshotFallback
{
    /// <inheritdoc/>
    public async Task<Models.UserRoles> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        logger.LogWarning("The roles of user {UserId} are not available in the shared cache; falling back to ms-users", userId);

        var response = await userGrpc.GetUserRoles(
            new UserProto.GetUserRolesRequest { Id = userId.ToString() }, cancellationToken);

        if (response is null)
            return null;

        return new Models.UserRoles
        {
            UserId = userId,
            Platform = [.. response.Platform],
            Tenants = response.Tenants.ToDictionary(x => x.Tenant, x => x.Roles.ToArray())
        };
    }
}
