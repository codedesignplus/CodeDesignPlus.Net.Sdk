using System;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.gRpc.Clients.Abstractions;
using CodeDesignPlus.Net.gRpc.Clients.Services.Cache;
using CodeDesignPlus.Net.gRpc.Clients.Services.Tenant;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.Cache;

public class TenantSnapshotFallbackTest
{
    // El contrato del respaldo devuelve null solo cuando el tenant no existe. Cualquier otro fallo tiene que
    // propagarse, para que el directorio lo trate como "no disponible" (503) y no como "no existe" (400).
    // pendings/028.

    [Fact]
    public async Task GetAsync_TenantsAnswersNotFound_ReturnsNull()
    {
        // Arrange
        var grpc = new Mock<ITenantGrpc>();
        grpc.Setup(g => g.GetTenantByIdAsync(It.IsAny<GetTenantRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RpcException(new Status(StatusCode.NotFound, "Tenant not found")));

        var fallback = new TenantSnapshotFallback(grpc.Object, Mock.Of<ILogger<TenantSnapshotFallback>>());

        // Act
        var result = await fallback.GetAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(StatusCode.Unavailable)]
    [InlineData(StatusCode.DeadlineExceeded)]
    [InlineData(StatusCode.Internal)]
    [InlineData(StatusCode.FailedPrecondition)]
    public async Task GetAsync_AnyOtherFailure_Propagates(StatusCode status)
    {
        // Arrange
        var grpc = new Mock<ITenantGrpc>();
        grpc.Setup(g => g.GetTenantByIdAsync(It.IsAny<GetTenantRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RpcException(new Status(status, "boom")));

        var fallback = new TenantSnapshotFallback(grpc.Object, Mock.Of<ILogger<TenantSnapshotFallback>>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() => fallback.GetAsync(Guid.NewGuid()));
        Assert.Equal(status, exception.StatusCode);
    }
}
