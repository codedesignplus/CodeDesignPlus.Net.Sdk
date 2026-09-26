using System.Net;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Security.MIddlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace CodeDesignPlus.Net.Security.Test.Middlewares;

public class RbacMiddlewareTest
{
    private readonly Mock<RequestDelegate> nextMock;
    private readonly Mock<IUserContext> userContextMock;
    private readonly Mock<IOptions<CoreOptions>> coreOptionsMock;
    private readonly Mock<IRbac> rbacServiceMock;
    private readonly Mock<IRoleDirectory> roleDirectoryMock;
    private readonly DefaultHttpContext httpContext;

    public RbacMiddlewareTest()
    {
        nextMock = new Mock<RequestDelegate>();
        userContextMock = new Mock<IUserContext>();
        coreOptionsMock = new Mock<IOptions<CoreOptions>>();
        rbacServiceMock = new Mock<IRbac>();
        roleDirectoryMock = new Mock<IRoleDirectory>();
        httpContext = new DefaultHttpContext();

        var services = new ServiceCollection();
        services.AddSingleton(userContextMock.Object);
        services.AddSingleton(coreOptionsMock.Object);
        services.AddSingleton(rbacServiceMock.Object);
        services.AddSingleton(roleDirectoryMock.Object);
        httpContext.RequestServices = services.BuildServiceProvider();
        httpContext.GetRouteData().Values["controller"] = "TestController";
        httpContext.GetRouteData().Values["action"] = "TestAction";
    }

    /// <summary>
    /// /health/live no es de ningun controller: no tiene recurso que autorizar. Antes lanzaba
    /// NullReferenceException y la sonda de arranque dejaba el pod sin arrancar (plan 036 de pendings).
    /// </summary>
    [Fact]
    public async Task InvokeAsync_RutaSinController_PasaSinAutorizar()
    {
        var context = new DefaultHttpContext { RequestServices = httpContext.RequestServices };
        context.Request.Path = "/health/live";

        await new RbacMiddleware(nextMock.Object).InvokeAsync(context);

        nextMock.Verify(x => x(context), Times.Once);
        rbacServiceMock.Verify(x => x.IsAuthorizedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Never);
        Assert.NotEqual((int)HttpStatusCode.Forbidden, context.Response.StatusCode);
    }

    /// <summary>
    /// Una accion [AllowAnonymous] es publica por diseno: el RBAC no decide sobre ella.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_AccionAnonima_PasaSinAutorizar()
    {
        httpContext.SetEndpoint(new Endpoint(_ => Task.CompletedTask, new EndpointMetadataCollection(new Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute()), "publico"));
        rbacServiceMock
            .Setup(x => x.IsAuthorizedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()))
            .ReturnsAsync(false);

        await new RbacMiddleware(nextMock.Object).InvokeAsync(httpContext);

        nextMock.Verify(x => x(httpContext), Times.Once);
        Assert.NotEqual((int)HttpStatusCode.Forbidden, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_UserNotAuthorized_ReturnsForbidden()
    {
        // Arrange
        var middleware = new RbacMiddleware(nextMock.Object);

        rbacServiceMock
            .Setup(x => x.IsAuthorizedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()))
            .ReturnsAsync(false);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal((int)HttpStatusCode.Forbidden, httpContext.Response.StatusCode);

        nextMock.Verify(x => x(It.IsAny<HttpContext>()), Times.Never);
        rbacServiceMock.Verify(x => x.IsAuthorizedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_UserAuthorized_CallsNextMiddleware()
    {
        // Arrange
        var middleware = new RbacMiddleware(nextMock.Object);
        rbacServiceMock
            .Setup(x => x.IsAuthorizedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()))
            .ReturnsAsync(true);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.NotEqual((int)HttpStatusCode.Forbidden, httpContext.Response.StatusCode);
        nextMock.Verify(x => x(It.IsAny<HttpContext>()), Times.Once);
        rbacServiceMock.Verify(x => x.IsAuthorizedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_AuthorizesWithTheRolesOfTheTenant_NotWithTheClaim()
    {
        // Arrange: el claim trae la union de los grupos del usuario en todo el directorio; el directorio
        // devuelve los de esta copropiedad. Autorizar con el claim le daria a quien administra otra
        // copropiedad los mismos permisos aqui.
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        userContextMock.SetupGet(x => x.IdUser).Returns(userId);
        userContextMock.SetupGet(x => x.Tenant).Returns(tenantId);
        userContextMock.SetupGet(x => x.Roles).Returns(["administrador-en-otra-copropiedad"]);

        roleDirectoryMock
            .Setup(x => x.GetRolesAsync(userId, tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(["residente-aqui"]);

        string[] usados = null;

        rbacServiceMock
            .Setup(x => x.IsAuthorizedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()))
            .Callback<string, string, string, string[]>((_, _, _, roles) => usados = roles)
            .ReturnsAsync(true);

        var middleware = new RbacMiddleware(nextMock.Object);

        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        Assert.Equal(["residente-aqui"], usados);
        Assert.DoesNotContain("administrador-en-otra-copropiedad", usados);
    }
}
