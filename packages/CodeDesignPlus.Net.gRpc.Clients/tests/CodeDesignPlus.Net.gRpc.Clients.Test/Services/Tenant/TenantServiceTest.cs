using System;
using System.Threading.Tasks;
using CodeDesignPlus.Net.gRpc.Clients.Services.Tenant;
using CodeDesignPlus.Net.gRpc.Clients.Services.Tenants;
using CodeDesignPlus.Net.Security.Abstractions;
using CodeDesignPlus.Net.xUnit.Extensions;
using Moq;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.Tenant;

public class TenantServiceTest
{
    [Fact]
    public async Task CreateTenantAsync_ShouldCreateTenant_WhenCalledWithValidRequest()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(uc => uc.AccessToken).Returns("test-access-token");
        userContextMock.Setup(uc => uc.Tenant).Returns(Guid.NewGuid());

        var mockCall = GrpcUtil.CreateAsyncUnaryCall(new Google.Protobuf.WellKnownTypes.Empty());

        var mockClient = new Mock<gRpc.Clients.Services.Tenant.Tenant.TenantClient>();
        mockClient
            .Setup(m => m.CreateTenantAsync(It.IsAny<CreateTenantRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);

        var tenantService = new TenantService(mockClient.Object, userContextMock.Object);
        var request = new CreateTenantRequest
        {
            Name = "Test Tenant",
            Domain = "testtenant.com",
            Email = "testtenant@example.com",
            IsActive = true,
            License = new License
            {
                Id = Guid.NewGuid().ToString(),
                EndDate = DateTime.UtcNow.AddYears(1).ToString(),
                StartDate = DateTime.UtcNow.ToString(),
            },
            Location = new Location
            {
                City = new City
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test City",
                    Timezone = "-5",
                },
                Country = new Country
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test Country",
                },
                Address = "123 Test St",
                PostalCode = "12345",
                State = new State
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test State",
                },
                Locality = new Locality
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test Locality",
                },
                Neighborhood = new Neighborhood
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test Neighborhood",
                },
            },
            Id = Guid.NewGuid().ToString(),
            NumbreDocument = "123456789",
            Phone = "123-456-7890",
            TypeDocument = new TypeDocument
            {
                Name = "Test Document Type",
                Code = "TD123",
            },
        };

        // Act
        await tenantService.CreateTenantAsync(request, CancellationToken.None);

        // Assert
        mockClient.Verify(m => m.CreateTenantAsync(It.IsAny<CreateTenantRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateTenantAsync_ShouldUpdateTenant_WhenCalledWithValidRequest()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(uc => uc.AccessToken).Returns("test-access-token");
        userContextMock.Setup(uc => uc.Tenant).Returns(Guid.NewGuid());

        var mockCall = GrpcUtil.CreateAsyncUnaryCall(new Google.Protobuf.WellKnownTypes.Empty());

        var mockClient = new Mock<gRpc.Clients.Services.Tenant.Tenant.TenantClient>();
        mockClient
            .Setup(m => m.UpdateTenantAsync(It.IsAny<UpdateTenantRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);

        var tenantService = new TenantService(mockClient.Object, userContextMock.Object);
        var request = new UpdateTenantRequest
        {
            Name = "Test Tenant",
            Domain = "testtenant.com",
            Email = "testtenant@example.com",
            IsActive = true,
            License = new License
            {
                Id = Guid.NewGuid().ToString(),
                EndDate = DateTime.UtcNow.AddYears(1).ToString(),
                StartDate = DateTime.UtcNow.ToString(),
            },
            Location = new Location
            {
                City = new City
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test City",
                    Timezone = "-5",
                },
                Country = new Country
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test Country",
                },
                Address = "123 Test St",
                PostalCode = "12345",
                State = new State
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test State",
                },
                Locality = new Locality
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test Locality",
                },
                Neighborhood = new Neighborhood
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test Neighborhood",
                },
            },
            Id = Guid.NewGuid().ToString(),
            NumbreDocument = "123456789",
            Phone = "123-456-7890",
            TypeDocument = new TypeDocument
            {
                Name = "Test Document Type",
                Code = "TD123",
            }
        };

        // Act
        await tenantService.UpdateTenantAsync(request, CancellationToken.None);

        // Assert
        mockClient.Verify(m => m.UpdateTenantAsync(It.IsAny<UpdateTenantRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteTenantAsync_ShouldDeleteTenant_WhenCalledWithValidRequest()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(uc => uc.AccessToken).Returns("test-access-token");
        userContextMock.Setup(uc => uc.Tenant).Returns(Guid.NewGuid());

        var mockCall = GrpcUtil.CreateAsyncUnaryCall(new Google.Protobuf.WellKnownTypes.Empty());
        var mockClient = new Mock<gRpc.Clients.Services.Tenant.Tenant.TenantClient>();
        mockClient
            .Setup(m => m.DeleteTenantAsync(It.IsAny<DeleteTenantRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);
        var tenantService = new TenantService(mockClient.Object, userContextMock.Object);
        var request = new DeleteTenantRequest
        {
            Id = Guid.NewGuid().ToString(),
        };

        // Act 
        await tenantService.DeleteTenantAsync(request, CancellationToken.None);

        // Assert
        mockClient.Verify(m => m.DeleteTenantAsync(It.IsAny<DeleteTenantRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetTenantByIdAsync_ShouldReturnTenant_WhenCalledWithValidRequest()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(uc => uc.AccessToken).Returns("test-access-token");
        userContextMock.Setup(uc => uc.Tenant).Returns(Guid.NewGuid());

        var mockCall = GrpcUtil.CreateAsyncUnaryCall(new GetTenantResponse
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Tenant",
            Domain = "testtenant.com",
            Email = "testtenant@example.com",
            IsActive = true,
            License = new License
            {
                Id = Guid.NewGuid().ToString(),
                EndDate = DateTime.UtcNow.AddYears(1).ToString(),
                StartDate = DateTime.UtcNow.ToString(),
            },
            Location = new Location
            {
                City = new City
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test City",
                    Timezone = "-5",
                },
                Country = new Country
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test Country",
                },
                Address = "123 Test St",
                PostalCode = "12345",
                State = new State
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test State",
                },
                Locality = new Locality
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test Locality",
                },
                Neighborhood = new Neighborhood
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test Neighborhood",
                },
            }
        });

        var mockClient = new Mock<gRpc.Clients.Services.Tenant.Tenant.TenantClient>();
        mockClient
            .Setup(m => m.GetTenantAsync(It.IsAny<GetTenantRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);

        var tenantService = new TenantService(mockClient.Object, userContextMock.Object);
        var request = new GetTenantRequest
        {
            Id = Guid.NewGuid().ToString(),
        };
        // Act
        var response = await tenantService.GetTenantByIdAsync(request, CancellationToken.None);

        // Assert
        mockClient.Verify(m => m.GetTenantAsync(It.IsAny<GetTenantRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None), Times.Once);

        Assert.NotNull(response);
    }
}
