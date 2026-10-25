using CodeDesignPlus.Net.Security.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using M = CodeDesignPlus.Net.Security.Abstractions.Models;

namespace CodeDesignPlus.Net.Security.Test.Services;

/// <summary>
/// La copia solo descarta la foto de una copropiedad que el directorio confirma como inexistente; si no pudo consultarlo,
/// la aplica (pendings/467).
/// </summary>
public class SnapshotTenantFilterTest
{
    private readonly Mock<ITenantDirectory> directory = new();

    [Fact]
    public async Task IsMissingAsync_DirectoryConfirmsItDoesNotExist_ReturnsTrue()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        this.directory.Setup(d => d.LookupAsync(tenantId, It.IsAny<CancellationToken>())).ReturnsAsync(M.TenantLookup.NotFound);

        var filter = new SnapshotTenantFilter(this.directory.Object, NullLogger<SnapshotTenantFilter>.Instance);

        // Act
        var missing = await filter.IsMissingAsync(tenantId, CancellationToken.None);

        // Assert
        Assert.True(missing);
    }

    [Fact]
    public async Task IsMissingAsync_DirectoryUnavailable_ReturnsFalse()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        this.directory.Setup(d => d.LookupAsync(tenantId, It.IsAny<CancellationToken>())).ReturnsAsync(M.TenantLookup.Unavailable);

        var filter = new SnapshotTenantFilter(this.directory.Object, NullLogger<SnapshotTenantFilter>.Instance);

        // Act
        var missing = await filter.IsMissingAsync(tenantId, CancellationToken.None);

        // Assert
        Assert.False(missing);
    }
}
