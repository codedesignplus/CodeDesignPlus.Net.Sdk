using System;
using CodeDesignPlus.Net.gRpc.Clients.Services.User;
using CodeDesignPlus.Net.gRpc.Clients.Services.Users;
using CodeDesignPlus.Net.Security.Abstractions;
using CodeDesignPlus.Net.xUnit.Extensions;
using Moq;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.Users;

public class UserServiceTest
{
    [Fact]
    public async Task AddTenantToUser_ShouldAddTenant_WhenCalledWithValidRequest()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(uc => uc.AccessToken).Returns("test-access-token");
        userContextMock.Setup(uc => uc.Tenant).Returns(Guid.NewGuid());

        var mockCall = GrpcUtil.CreateAsyncUnaryCall(new Google.Protobuf.WellKnownTypes.Empty());

        var mockClient = new Mock<gRpc.Clients.Services.User.Users.UsersClient>();
        mockClient
            .Setup(m => m.AddTenantToUserAsync(It.IsAny<AddTenantRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);

        var userService = new UserService(mockClient.Object, userContextMock.Object);
        var request = new AddTenantRequest
        {
            Id = Guid.NewGuid().ToString(),
            Tenant = new Clients.Services.User.Tenant()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Tenant"
            }
        };

        // Act
        await userService.AddTenantToUser(request, CancellationToken.None);

        // Assert
        mockClient.Verify(m => m.AddTenantToUserAsync(It.IsAny<AddTenantRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task AddGroupToUser_ShouldAddGroup_WhenCalledWithValidRequest()
    {
        // Arrange
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(uc => uc.AccessToken).Returns("test-access-token");
        userContextMock.Setup(uc => uc.Tenant).Returns(Guid.NewGuid());

        var mockCall = GrpcUtil.CreateAsyncUnaryCall(new Google.Protobuf.WellKnownTypes.Empty());

        var mockClient = new Mock<gRpc.Clients.Services.User.Users.UsersClient>();
        mockClient
            .Setup(m => m.AddGroupToUserAsync(It.IsAny<AddGroupRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None))
            .Returns(mockCall);

        var userService = new UserService(mockClient.Object, userContextMock.Object);
        var request = new AddGroupRequest
        {
            Id = Guid.NewGuid().ToString(),
            Role = "administrator"
        };

        // Act
        await userService.AddGroupToUser(request, CancellationToken.None);

        // Assert
        mockClient.Verify(m => m.AddGroupToUserAsync(It.IsAny<AddGroupRequest>(), It.IsAny<Grpc.Core.Metadata>(), It.IsAny<DateTime?>(), CancellationToken.None), Times.Once);
    }
}
