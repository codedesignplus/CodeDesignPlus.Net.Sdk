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
    private readonly DefaultHttpContext httpContext;

    public RbacMiddlewareTest()
    {
        nextMock = new Mock<RequestDelegate>();
        userContextMock = new Mock<IUserContext>();
        coreOptionsMock = new Mock<IOptions<CoreOptions>>();
        rbacServiceMock = new Mock<IRbac>();
        httpContext = new DefaultHttpContext();

        var services = new ServiceCollection();
        services.AddSingleton(userContextMock.Object);
        services.AddSingleton(coreOptionsMock.Object);
        services.AddSingleton(rbacServiceMock.Object);
        httpContext.RequestServices = services.BuildServiceProvider();
        httpContext.GetRouteData().Values["controller"] = "TestController";
        httpContext.GetRouteData().Values["action"] = "TestAction";
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
}
