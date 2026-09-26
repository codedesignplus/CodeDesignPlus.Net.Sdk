
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
        var resources = rbacService.GetType().GetField("resources", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(rbacService) as IReadOnlyList<RbacResource>;

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
        var result = await rbacService.IsAuthorizedAsync("TestController", "TestAction", "GET", roles);

        // Assert
        Assert.True(result);
        loggerMock.VerifyLogging("Role 'Admin' is authorized to access the resource 'TestController/TestAction' with the method 'GET'", LogLevel.Debug, Times.Once());
    }

    [Fact]
    public async Task IsAuthorizedAsync_ShouldReturnFalse_WhenUserIsNotAuthorized()
    {
        // Arrange
        var roles = new[] { "User" };

        ConfigResponseMock();

        // Act
        var result = await rbacService.IsAuthorizedAsync("TestController", "TestAction", "GET", roles);

        // Assert
        Assert.False(result);
        loggerMock.VerifyLogging("Role 'User' is not authorized to access the resource 'TestController/TestAction' with the method 'GET'", LogLevel.Debug, Times.Once());
    }

    [Fact]
    public async Task IsAuthorizedAsync_ActionNotFound_ShouldReturnFalse()
    {
        // Arrange
        var roles = new[] { "Admin" };

        ConfigResponseMock();

        // Act
        var result = await rbacService.IsAuthorizedAsync("TestController", "NonExistentAction", "GET", roles);

        // Assert
        Assert.False(result);
        loggerMock.VerifyLogging("Role 'Admin' is not authorized to access the resource 'TestController/NonExistentAction' with the method 'GET'", LogLevel.Debug, Times.Once());
    }

    [Fact]
    public async Task IsAuthorizedAsync_ControllerNotFound_ShouldReturnFalse()
    {
        // Arrange
        var roles = new[] { "Admin" };

        ConfigResponseMock();

        // Act
        var result = await rbacService.IsAuthorizedAsync("NonExistentController", "TestAction", "GET", roles);

        // Assert
        Assert.False(result);
        loggerMock.VerifyLogging("Role 'Admin' is not authorized to access the resource 'NonExistentController/TestAction' with the method 'GET'", LogLevel.Debug, Times.Once());
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
        var result = await rbacService.IsAuthorizedAsync("TestController", "TestAction", "GET", roles);

        // Assert
        Assert.False(result);
        loggerMock.VerifyLogging("Role 'InvalidRole' is not authorized to access the resource 'TestController/TestAction' with the method 'GET'", LogLevel.Debug, Times.Once());
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

    /// <summary>
    /// ASP.NET entrega el metodo en mayusculas ("GET"). Antes se convertia con un TryParse que distinguia mayusculas y
    /// "GET" no casaba con Get: todo quedaba en None y no se autorizaba nada (plan 031 de pendings).
    /// </summary>
    [Theory]
    [InlineData("GET", gRpc.HttpMethod.Get)]
    [InlineData("get", gRpc.HttpMethod.Get)]
    [InlineData("POST", gRpc.HttpMethod.Post)]
    [InlineData("PUT", gRpc.HttpMethod.Put)]
    [InlineData("PATCH", gRpc.HttpMethod.Patch)]
    [InlineData("DELETE", gRpc.HttpMethod.Delete)]
    [InlineData("OPTIONS", gRpc.HttpMethod.None)]
    [InlineData("", gRpc.HttpMethod.None)]
    [InlineData(null!, gRpc.HttpMethod.None)]
    public void ToHttpMethod_TraduceElMetodoDeLaPeticion(string? input, gRpc.HttpMethod expected)
    {
        Assert.Equal(expected, S.Rbac.ToHttpMethod(input!));
    }

    [Fact]
    public async Task IsAuthorizedAsync_AutorizaConElMetodoEnMayusculasComoLlegaDeAspNet()
    {
        ConfigResponseMock();
        await rbacService.LoadRbacAsync(CancellationToken.None);

        Assert.True(await rbacService.IsAuthorizedAsync("TestController", "TestAction", "GET", ["Admin"]));
        Assert.False(await rbacService.IsAuthorizedAsync("TestController", "TestAction", "POST", ["Admin"]));
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
