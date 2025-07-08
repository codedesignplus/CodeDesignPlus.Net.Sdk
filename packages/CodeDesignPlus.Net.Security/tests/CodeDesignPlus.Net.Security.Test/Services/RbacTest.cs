
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Security.gRpc;
using Moq;
using S = CodeDesignPlus.Net.Security.Services;
using CodeDesignPlus.Net.xUnit.Extensions;
using System.Reflection;
using System.Collections.Concurrent;

namespace CodeDesignPlus.Net.Security.Test.Services;

public class RbacTest
{
    private readonly Mock<ILogger<S.Rbac>> loggerMock;
    private readonly IOptions<CoreOptions> coreOptions;
    private readonly Mock<gRpc.Rbac.RbacClient> clientMock;
    private readonly S.Rbac rbacService;

    public RbacTest()
    {
        loggerMock = new Mock<ILogger<S.Rbac>>();
        clientMock = new Mock<gRpc.Rbac.RbacClient>();

        coreOptions = Microsoft.Extensions.Options.Options.Create(OptionsUtil.CoreOptions);

        rbacService = new S.Rbac(loggerMock.Object, coreOptions, clientMock.Object);
    }

    [Fact]
    public async Task LoadRbacAsync_ShouldLoadResources()
    {
        // Arrange
        ConfigResponseMock();

        // Act
        await rbacService.LoadRbacAsync(CancellationToken.None);

        // Assert
        var resources = rbacService.GetType().GetField("resources", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(rbacService) as ConcurrentBag<RbacResource>;

        Assert.NotNull(resources);
        Assert.NotEmpty(resources);
        Assert.Contains(resources, x => x.Controller == "TestController" && x.Action == "TestAction" && x.Method == gRpc.HttpMethod.Get && x.Role == "Admin");

        loggerMock.VerifyLogging("RbacService loaded, number of resources: 1", LogLevel.Information, Times.Once());
    }

    [Fact]
    public async Task IsAuthorizedAsync_ShouldReturnTrue_WhenUserIsAuthorized()
    {
        // Arrange
        var roles = new[] { "Admin" };

        ConfigResponseMock();

        await rbacService.LoadRbacAsync(CancellationToken.None);

        // Act
        var result = await rbacService.IsAuthorizedAsync("TestController", "TestAction", "Get", roles);

        // Assert
        Assert.True(result);
        loggerMock.VerifyLogging("Role 'Admin' is authorized to access the resource 'TestController/TestAction' with the method 'Get'", LogLevel.Debug, Times.Once());
    }

    [Fact]
    public async Task IsAuthorizedAsync_ShouldReturnFalse_WhenUserIsNotAuthorized()
    {
        // Arrange
        var roles = new[] { "User" };

        ConfigResponseMock();

        // Act
        var result = await rbacService.IsAuthorizedAsync("TestController", "TestAction", "Get", roles);

        // Assert
        Assert.False(result);
        loggerMock.VerifyLogging("Role 'User' is not authorized to access the resource 'TestController/TestAction' with the method 'Get'", LogLevel.Debug, Times.Once());
    }

    [Fact]
    public async Task IsAuthorizedAsync_ActionNotFound_ShouldReturnFalse()
    {
        // Arrange
        var roles = new[] { "Admin" };

        ConfigResponseMock();

        // Act
        var result = await rbacService.IsAuthorizedAsync("TestController", "NonExistentAction", "Get", roles);

        // Assert
        Assert.False(result);
        loggerMock.VerifyLogging("Role 'Admin' is not authorized to access the resource 'TestController/NonExistentAction' with the method 'Get'", LogLevel.Debug, Times.Once());
    }

    [Fact]
    public async Task IsAuthorizedAsync_ControllerNotFound_ShouldReturnFalse()
    {
        // Arrange
        var roles = new[] { "Admin" };

        ConfigResponseMock();

        // Act
        var result = await rbacService.IsAuthorizedAsync("NonExistentController", "TestAction", "Get", roles);

        // Assert
        Assert.False(result);
        loggerMock.VerifyLogging("Role 'Admin' is not authorized to access the resource 'NonExistentController/TestAction' with the method 'Get'", LogLevel.Debug, Times.Once());
    }

    [Fact]
    public async Task IsAuthorizedAsync_InvalidHttpMethod_ShouldReturnFalse()
    {
        // Arrange
        var roles = new[] { "Admin" };

        ConfigResponseMock();

        // Act
        var result = await rbacService.IsAuthorizedAsync("TestController", "TestAction", "InvalidMethod", roles);

        // Assert
        Assert.False(result);
        loggerMock.VerifyLogging("Role 'Admin' is not authorized to access the resource 'TestController/TestAction' with the method 'InvalidMethod'", LogLevel.Debug, Times.Once());
    }

    
    [Fact]
    public async Task IsAuthorizedAsync_InvalidRoles_ShouldReturnFalse()
    {
        // Arrange
        var roles = new[] { "InvalidRole" };

        ConfigResponseMock();

        // Act
        var result = await rbacService.IsAuthorizedAsync("TestController", "TestAction", "Get", roles);

        // Assert
        Assert.False(result);
        loggerMock.VerifyLogging("Role 'InvalidRole' is not authorized to access the resource 'TestController/TestAction' with the method 'Get'", LogLevel.Debug, Times.Once());
    }

    [Fact]    
    public async Task IsAuthorizedAsync_InvalidAllValues_ShouldReturnFalse()
    {
        // Arrange
        var roles = new[] { "InvalidRole" };

        ConfigResponseMock();

        // Act
        var result = await rbacService.IsAuthorizedAsync("InvalidController", "InvalidAction", "InvalidMethod", roles);

        // Assert
        Assert.False(result);
        loggerMock.VerifyLogging("Role 'InvalidRole' is not authorized to access the resource 'InvalidController/InvalidAction' with the method 'InvalidMethod'", LogLevel.Debug, Times.Once());
    }

    [Theory]
    [InlineData("Get", gRpc.HttpMethod.Get)]
    [InlineData("Post", gRpc.HttpMethod.Post)]
    [InlineData("Put", gRpc.HttpMethod.Put)]
    [InlineData("Delete", gRpc.HttpMethod.Delete)]
    [InlineData("InvalidMethod", gRpc.HttpMethod.None)]
    [InlineData("", gRpc.HttpMethod.None)]
    [InlineData(null!, gRpc.HttpMethod.None)]
    public void ConvertToEnum_ShouldReturnExpectedEnum(string? input, gRpc.HttpMethod expected)
    {
        var method = typeof(S.Rbac).GetMethod("ConvertToEnum", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var result = (gRpc.HttpMethod)method.Invoke(null, [input])!;

        Assert.Equal(expected, result);
    }

    private void ConfigResponseMock()
    {
        var response = new GetRbacResponse
        {
            Resources = { new RbacResource { Controller = "TestController", Action = "TestAction", Method = gRpc.HttpMethod.Get, Role = "Admin" } }
        };

        var mockCall = GrpcUtil.CreateAsyncUnaryCall(response);

        clientMock
           .Setup(m => m.GetRbacAsync(It.IsAny<GetRbacRequest>(), null, null, CancellationToken.None))
           .Returns(mockCall);
    }



}
