using System.Net;
using CodeDesignPlus.Net.Security.Middlewares;
using Microsoft.AspNetCore.Http;
using Moq;

namespace CodeDesignPlus.Net.Security.Test.Middlewares;

public class LicenseMiddlewareTest
{
    [Fact]
    public async Task InvokeAsync_ValidLicense_CallsNextMiddleware()
    {
        // Arrange
        var tenantMock = new Mock<ITenant>();
        tenantMock.SetupGet(t => t.IsLoaded).Returns(true);
        tenantMock.Setup(t => t.LicenseIsValid()).Returns(true);

        var context = BuildContext(tenantMock);
        var nextCalled = false;

        Task next(HttpContext ctx)
        {
            nextCalled = true;
            return Task.CompletedTask;
        }

        var middleware = new LicenseMiddleware((RequestDelegate)next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        tenantMock.Verify(t => t.LicenseIsValid(), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_InvalidLicense_SetsForbiddenStatusCode()
    {
        // Arrange
        var tenantMock = new Mock<ITenant>();
        tenantMock.SetupGet(t => t.IsLoaded).Returns(true);
        tenantMock.Setup(t => t.LicenseIsValid()).Returns(false);

        var context = BuildContext(tenantMock);

        static Task next(HttpContext ctx) => Task.CompletedTask;

        var middleware = new LicenseMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.Forbidden, context.Response.StatusCode);
        tenantMock.Verify(t => t.LicenseIsValid(), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_TenantWasNeverLoaded_SetsForbiddenWithoutCheckingLicense()
    {
        // Arrange: pasa cuando el request no traia X-Tenant, asi que TenantContextMiddleware no cargo nada.
        var tenantMock = new Mock<ITenant>();
        tenantMock.SetupGet(t => t.IsLoaded).Returns(false);

        var context = BuildContext(tenantMock);
        var nextCalled = false;

        Task next(HttpContext ctx)
        {
            nextCalled = true;
            return Task.CompletedTask;
        }

        var middleware = new LicenseMiddleware((RequestDelegate)next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.False(nextCalled);
        Assert.Equal((int)HttpStatusCode.Forbidden, context.Response.StatusCode);
        tenantMock.Verify(t => t.LicenseIsValid(), Times.Never);
    }

    private static DefaultHttpContext BuildContext(Mock<ITenant> tenantMock)
    {
        var services = new ServiceCollection();
        services.AddSingleton(tenantMock.Object);

        return new DefaultHttpContext { RequestServices = services.BuildServiceProvider() };
    }
}
