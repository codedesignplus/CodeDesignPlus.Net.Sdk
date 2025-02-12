using System.Net;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Security.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.Security.Test.Middlewares;

public class RbacMiddlewareTest
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<IOptions<CoreOptions>> _coreOptionsMock;
    private readonly Mock<IRbac> _rbacServiceMock;
    private readonly DefaultHttpContext _httpContext;

    public RbacMiddlewareTest()
    {
        _nextMock = new Mock<RequestDelegate>();
        _userContextMock = new Mock<IUserContext>();
        _coreOptionsMock = new Mock<IOptions<CoreOptions>>();
        _rbacServiceMock = new Mock<IRbac>();
        _httpContext = new DefaultHttpContext();

        var services = new ServiceCollection();
        services.AddSingleton(_userContextMock.Object);
        services.AddSingleton(_coreOptionsMock.Object);
        services.AddSingleton(_rbacServiceMock.Object);
        _httpContext.RequestServices = services.BuildServiceProvider();
    }

    [Fact]
    public async Task InvokeAsync_UserNotAuthorized_ReturnsForbidden()
    {
        // Arrange
        var middleware = new RbacMiddleware(_nextMock.Object);
        _rbacServiceMock.Setup(x => x.IsAuthorizedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()))
            .ReturnsAsync(false);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.Equal((int)HttpStatusCode.Forbidden, _httpContext.Response.StatusCode);
        _nextMock.Verify(x => x(It.IsAny<HttpContext>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_UserAuthorized_CallsNextMiddleware()
    {
        // Arrange
        var middleware = new RbacMiddleware(_nextMock.Object);
        _rbacServiceMock.Setup(x => x.IsAuthorizedAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()))
            .ReturnsAsync(true);

        // Act
        await middleware.InvokeAsync(_httpContext);

        // Assert
        Assert.NotEqual((int)HttpStatusCode.Forbidden, _httpContext.Response.StatusCode);
        _nextMock.Verify(x => x(It.IsAny<HttpContext>()), Times.Once);
    }
}
