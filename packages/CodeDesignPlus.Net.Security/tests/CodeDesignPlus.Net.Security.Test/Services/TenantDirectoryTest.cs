using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Security.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using M = CodeDesignPlus.Net.Security.Abstractions.Models;

namespace CodeDesignPlus.Net.Security.Test.Services;

public class TenantDirectoryTest
{
    private readonly Mock<ICacheManager> cacheManagerMock = new();
    private readonly Mock<ITenantSnapshotFallback> fallbackMock = new();
    private readonly IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

    [Fact]
    public async Task GetSnapshotAsync_EmptyTenant_ReturnsNullWithoutTouchingTheCache()
    {
        // Arrange
        var directory = BuildDirectory();

        // Act
        var result = await directory.GetSnapshotAsync(Guid.Empty);

        // Assert
        Assert.Null(result);
        cacheManagerMock.Verify(c => c.GetGlobalAsync<M.Tenant>(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetSnapshotAsync_ReadsSharedKey_AndCachesInMemory()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var snapshot = new M.Tenant { Id = tenantId };
        cacheManagerMock.Setup(c => c.GetGlobalAsync<M.Tenant>(TenantCacheKeys.Snapshot(tenantId))).ReturnsAsync(snapshot);

        var directory = BuildDirectory();

        // Act
        var first = await directory.GetSnapshotAsync(tenantId);
        var second = await directory.GetSnapshotAsync(tenantId);

        // Assert
        Assert.Same(snapshot, first);
        Assert.Same(snapshot, second);
        cacheManagerMock.Verify(c => c.GetGlobalAsync<M.Tenant>(TenantCacheKeys.Snapshot(tenantId)), Times.Once);
    }

    [Fact]
    public async Task GetSnapshotAsync_NotPublished_UsesFallback()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var snapshot = new M.Tenant { Id = tenantId };
        cacheManagerMock.Setup(c => c.GetGlobalAsync<M.Tenant>(It.IsAny<string>())).ReturnsAsync((M.Tenant)null!);
        fallbackMock.Setup(f => f.GetAsync(tenantId, It.IsAny<CancellationToken>())).ReturnsAsync(snapshot);

        var directory = BuildDirectory();

        // Act
        var result = await directory.GetSnapshotAsync(tenantId);

        // Assert
        Assert.Same(snapshot, result);
        fallbackMock.Verify(f => f.GetAsync(tenantId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetSnapshotAsync_CacheServerIsDown_UsesFallback()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var snapshot = new M.Tenant { Id = tenantId };
        cacheManagerMock.Setup(c => c.GetGlobalAsync<M.Tenant>(It.IsAny<string>())).ThrowsAsync(new InvalidOperationException("redis is down"));
        fallbackMock.Setup(f => f.GetAsync(tenantId, It.IsAny<CancellationToken>())).ReturnsAsync(snapshot);

        var directory = BuildDirectory();

        // Act
        var result = await directory.GetSnapshotAsync(tenantId);

        // Assert
        Assert.Same(snapshot, result);
    }

    [Fact]
    public async Task GetSnapshotAsync_EveryLevelFails_ReturnsNull()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        cacheManagerMock.Setup(c => c.GetGlobalAsync<M.Tenant>(It.IsAny<string>())).ReturnsAsync((M.Tenant)null!);
        fallbackMock.Setup(f => f.GetAsync(tenantId, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("ms-tenants is down"));

        var directory = BuildDirectory();

        // Act
        var result = await directory.GetSnapshotAsync(tenantId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetSnapshotAsync_WithoutFallbackRegistered_ReturnsNullOnMiss()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        cacheManagerMock.Setup(c => c.GetGlobalAsync<M.Tenant>(It.IsAny<string>())).ReturnsAsync((M.Tenant)null!);

        var directory = new TenantDirectory(cacheManagerMock.Object, memoryCache, Mock.Of<ILogger<TenantDirectory>>());

        // Act
        var result = await directory.GetSnapshotAsync(tenantId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetSnapshotAsync_RefreshFails_ServesTheStaleSnapshot()
    {
        // Arrange: se calienta la entrada y despues se cae todo. La copia retenida tiene que servir.
        var tenantId = Guid.NewGuid();
        var snapshot = new M.Tenant { Id = tenantId };
        var clock = new AdjustableTimeProvider();
        var directory = new TenantDirectory(cacheManagerMock.Object, memoryCache, Mock.Of<ILogger<TenantDirectory>>(), fallbackMock.Object, clock);

        cacheManagerMock.Setup(c => c.GetGlobalAsync<M.Tenant>(It.IsAny<string>())).ReturnsAsync(snapshot);
        await directory.GetSnapshotAsync(tenantId);

        clock.Advance(TimeSpan.FromMinutes(5));

        cacheManagerMock.Setup(c => c.GetGlobalAsync<M.Tenant>(It.IsAny<string>())).ThrowsAsync(new InvalidOperationException("redis is down"));
        fallbackMock.Setup(f => f.GetAsync(tenantId, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("ms-tenants is down"));

        // Act
        var result = await directory.GetSnapshotAsync(tenantId);

        // Assert
        Assert.Same(snapshot, result);
    }

    [Fact]
    public async Task GetSnapshotAsync_FreshWindowElapsed_RefreshesFromTheSharedCache()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var clock = new AdjustableTimeProvider();
        var directory = new TenantDirectory(cacheManagerMock.Object, memoryCache, Mock.Of<ILogger<TenantDirectory>>(), fallbackMock.Object, clock);

        cacheManagerMock.Setup(c => c.GetGlobalAsync<M.Tenant>(It.IsAny<string>())).ReturnsAsync(new M.Tenant { Id = tenantId });

        // Act
        await directory.GetSnapshotAsync(tenantId);
        clock.Advance(TimeSpan.FromMinutes(5));
        await directory.GetSnapshotAsync(tenantId);

        // Assert
        cacheManagerMock.Verify(c => c.GetGlobalAsync<M.Tenant>(TenantCacheKeys.Snapshot(tenantId)), Times.Exactly(2));
    }

    [Fact]
    public async Task GetActiveTenantsAsync_ReturnsParsedIdentifiers()
    {
        // Arrange
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();
        cacheManagerMock.Setup(c => c.GetGlobalSetMembersAsync(TenantCacheKeys.ActiveTenants))
            .ReturnsAsync([first.ToString(), "not-a-guid", second.ToString()]);

        var directory = BuildDirectory();

        // Act
        var result = await directory.GetActiveTenantsAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(first, result);
        Assert.Contains(second, result);
    }

    [Fact]
    public async Task GetActiveTenantsAsync_IndexIsEmpty_ReturnsEmpty()
    {
        // Arrange
        cacheManagerMock.Setup(c => c.GetGlobalSetMembersAsync(TenantCacheKeys.ActiveTenants)).ReturnsAsync([]);

        var directory = BuildDirectory();

        // Act
        var result = await directory.GetActiveTenantsAsync();

        // Assert
        Assert.Empty(result);
    }

    private TenantDirectory BuildDirectory() =>
        new(cacheManagerMock.Object, memoryCache, Mock.Of<ILogger<TenantDirectory>>(), fallbackMock.Object);

    private sealed class AdjustableTimeProvider : TimeProvider
    {
        private DateTimeOffset now = DateTimeOffset.UtcNow;

        public override DateTimeOffset GetUtcNow() => this.now;

        public void Advance(TimeSpan delta) => this.now = this.now.Add(delta);
    }
}
