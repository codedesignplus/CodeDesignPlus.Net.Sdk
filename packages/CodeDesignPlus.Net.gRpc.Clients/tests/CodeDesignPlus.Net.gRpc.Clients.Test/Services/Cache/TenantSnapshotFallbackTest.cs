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

    [Fact]
    public async Task GetAsync_LocationWithoutLocalityAndNeighborhood_KeepsThemNull()
    {
        // La mayoría de los municipios no tiene localidades ni barrios (pendings/130). Antes el respaldo les inventaba
        // un id y un nombre vacío, y el objeto de valor lanzaba al crearse.
        var response = new GetTenantResponse
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Conjunto Chía",
            Location = new Location
            {
                Country = new Country
                {
                    Id = Guid.NewGuid().ToString(), Name = "Colombia", Alpha2 = "CO", Alpha3 = "COL", Code = 170,
                    PhoneCode = "+57", Timezone = "America/Bogota",
                    Currency = new Currency { Id = Guid.NewGuid().ToString(), Code = "COP", Name = "Peso colombiano", Symbol = "$", DecimalDigits = 2, NumericCode = 170 }
                },
                State = new State { Id = Guid.NewGuid().ToString(), Name = "Cundinamarca", Code = "CUN" },
                City = new City { Id = Guid.NewGuid().ToString(), Name = "Chía" },
                Address = "Calle 10 # 5-20",
                PostalCode = "250001"
            }
        };

        var grpc = new Mock<ITenantGrpc>();
        grpc.Setup(g => g.GetTenantByIdAsync(It.IsAny<GetTenantRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var fallback = new TenantSnapshotFallback(grpc.Object, Mock.Of<ILogger<TenantSnapshotFallback>>());

        // Act
        var result = await fallback.GetAsync(Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Chía", result.Location.City.Name);
        Assert.Null(result.Location.Locality);
        Assert.Null(result.Location.Neighborhood);
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
