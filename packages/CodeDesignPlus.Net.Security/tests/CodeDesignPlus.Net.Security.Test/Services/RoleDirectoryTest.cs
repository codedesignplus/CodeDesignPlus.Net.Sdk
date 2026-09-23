using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Security.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using M = CodeDesignPlus.Net.Security.Abstractions.Models;

namespace CodeDesignPlus.Net.Security.Test.Services;

public class RoleDirectoryTest
{
    private readonly Mock<ICacheManager> cacheManagerMock = new();
    private readonly Mock<IRoleSnapshotFallback> fallbackMock = new();
    private readonly IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

    private static readonly string Administrator = "1a43656c-f457-4695-8bfd-903be4b66097";
    private static readonly string Resident = "d13dc2fd-59ce-4462-ae03-2a830a243c56";
    private static readonly string Platform = "26d7d461-412c-4e02-923e-29f450b20145";

    [Fact]
    public async Task GetRolesAsync_EmptyUser_ReturnsNoneWithoutTouchingTheCache()
    {
        // Arrange
        var directory = BuildDirectory();

        // Act
        var roles = await directory.GetRolesAsync(Guid.Empty, Guid.NewGuid());

        // Assert
        Assert.Empty(roles);
        cacheManagerMock.Verify(c => c.GetGlobalAsync<M.UserRoles>(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetRolesAsync_ReadsSharedKey_AndCachesInMemory()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        Published(userId, tenantId, [Administrator]);

        var directory = BuildDirectory();

        // Act
        var first = await directory.GetRolesAsync(userId, tenantId);
        var second = await directory.GetRolesAsync(userId, tenantId);

        // Assert
        Assert.Equal([Administrator], first);
        Assert.Equal([Administrator], second);
        cacheManagerMock.Verify(c => c.GetGlobalAsync<M.UserRoles>(RoleCacheKeys.Snapshot(userId)), Times.Once);
    }

    [Fact]
    public async Task GetRolesAsync_NotPublished_UsesFallback()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        cacheManagerMock
            .Setup(c => c.GetGlobalAsync<M.UserRoles>(RoleCacheKeys.Snapshot(userId)))
            .ReturnsAsync((M.UserRoles)null);

        fallbackMock
            .Setup(f => f.GetAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Snapshot(userId, tenantId, [Resident]));

        var directory = BuildDirectory();

        // Act
        var roles = await directory.GetRolesAsync(userId, tenantId);

        // Assert
        Assert.Equal([Resident], roles);
    }

    [Fact]
    public async Task GetRolesAsync_NothingResolves_DeniesInsteadOfGranting()
    {
        // Arrange
        var userId = Guid.NewGuid();

        cacheManagerMock
            .Setup(c => c.GetGlobalAsync<M.UserRoles>(It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("el cache compartido no responde"));

        fallbackMock
            .Setup(f => f.GetAsync(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("ms-users no responde"));

        var directory = BuildDirectory();

        // Act
        var roles = await directory.GetRolesAsync(userId, Guid.NewGuid());

        // Assert: un conjunto vacio deniega. Devolver algo para que la peticion siga seria conceder
        // permisos porque un servicio esta caido.
        Assert.Empty(roles);
    }

    [Fact]
    public async Task GetRolesAsync_EveryLevelDown_ServesTheRetainedSnapshot()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var clock = new AdjustableTimeProvider();

        cacheManagerMock
            .SetupSequence(c => c.GetGlobalAsync<M.UserRoles>(RoleCacheKeys.Snapshot(userId)))
            .ReturnsAsync(Snapshot(userId, tenantId, [Administrator]))
            .ThrowsAsync(new InvalidOperationException("el cache compartido se cayo"));

        fallbackMock
            .Setup(f => f.GetAsync(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("ms-users tambien"));

        var directory = BuildDirectory(clock);

        // Act
        var antes = await directory.GetRolesAsync(userId, tenantId);

        clock.Advance(TimeSpan.FromMinutes(5));

        var despues = await directory.GetRolesAsync(userId, tenantId);

        // Assert
        Assert.Equal([Administrator], antes);
        Assert.Equal([Administrator], despues);
    }

    [Fact]
    public async Task GetRolesAsync_RolesOfOneTenantDoNotLeakIntoAnother()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var administra = Guid.NewGuid();
        var solamenteReside = Guid.NewGuid();

        var snapshot = new M.UserRoles
        {
            UserId = userId,
            Tenants = new Dictionary<string, string[]>
            {
                [administra.ToString()] = [Administrator],
                [solamenteReside.ToString()] = [Resident],
            }
        };

        cacheManagerMock
            .Setup(c => c.GetGlobalAsync<M.UserRoles>(RoleCacheKeys.Snapshot(userId)))
            .ReturnsAsync(snapshot);

        var directory = BuildDirectory();

        // Act
        var enLaQueAdministra = await directory.GetRolesAsync(userId, administra);
        var enLaQueReside = await directory.GetRolesAsync(userId, solamenteReside);

        // Assert: este es el defecto que el directorio viene a cerrar. Con el claim del token, las dos
        // llamadas devolvian lo mismo.
        Assert.Equal([Administrator], enLaQueAdministra);
        Assert.Equal([Resident], enLaQueReside);
    }

    [Fact]
    public async Task GetRolesAsync_UnknownTenant_LeavesOnlyThePlatformRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var suya = Guid.NewGuid();

        var snapshot = Snapshot(userId, suya, [Administrator]);
        snapshot.Platform = [Platform];

        cacheManagerMock
            .Setup(c => c.GetGlobalAsync<M.UserRoles>(RoleCacheKeys.Snapshot(userId)))
            .ReturnsAsync(snapshot);

        var directory = BuildDirectory();

        // Act
        var roles = await directory.GetRolesAsync(userId, Guid.NewGuid());

        // Assert
        Assert.Equal([Platform], roles);
    }

    [Fact]
    public async Task GetRolesAsync_PlatformRolesApplyInEveryTenant()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var snapshot = Snapshot(userId, tenantId, [Resident]);
        snapshot.Platform = [Platform];

        cacheManagerMock
            .Setup(c => c.GetGlobalAsync<M.UserRoles>(RoleCacheKeys.Snapshot(userId)))
            .ReturnsAsync(snapshot);

        var directory = BuildDirectory();

        // Act
        var roles = await directory.GetRolesAsync(userId, tenantId);

        // Assert
        Assert.Equal([Platform, Resident], roles);
    }

    [Fact]
    public void EffectiveIn_MatchesTheTenantWhateverTheKeyLooksLike()
    {
        // Arrange: la instantanea viaja en JSON y el formato del identificador depende de quien la
        // escribio. Comparar cadenas dejaria al usuario sin roles en silencio.
        var tenantId = Guid.NewGuid();

        var snapshot = new M.UserRoles
        {
            Tenants = new Dictionary<string, string[]>
            {
                [tenantId.ToString().ToUpperInvariant()] = [Administrator],
            }
        };

        // Act
        var roles = snapshot.EffectiveIn(tenantId);

        // Assert
        Assert.Equal([Administrator], roles);
    }

    private void Published(Guid userId, Guid tenantId, string[] roles) =>
        cacheManagerMock
            .Setup(c => c.GetGlobalAsync<M.UserRoles>(RoleCacheKeys.Snapshot(userId)))
            .ReturnsAsync(Snapshot(userId, tenantId, roles));

    private static M.UserRoles Snapshot(Guid userId, Guid tenantId, string[] roles) => new()
    {
        UserId = userId,
        Tenants = new Dictionary<string, string[]> { [tenantId.ToString()] = roles }
    };

    private RoleDirectory BuildDirectory(TimeProvider timeProvider = null) =>
        new(cacheManagerMock.Object, memoryCache, Mock.Of<ILogger<RoleDirectory>>(), fallbackMock.Object, timeProvider);

    private sealed class AdjustableTimeProvider : TimeProvider
    {
        private DateTimeOffset now = DateTimeOffset.UtcNow;

        public override DateTimeOffset GetUtcNow() => this.now;

        public void Advance(TimeSpan delta) => this.now = this.now.Add(delta);
    }
}
