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
        var context = new DefaultHttpContext();
        var tenantMock = new Mock<ITenant>();
        var userContextMock = new Mock<IUserContext>();
        var nextCalled = false;

        tenantMock.Setup(t => t.LicenseIsValid()).Returns(true);
        tenantMock.Setup(t => t.SetTenantAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);
        userContextMock.SetupGet(uc => uc.Tenant).Returns(Guid.NewGuid());

        var services = new ServiceCollection();
        services.AddSingleton(tenantMock.Object);
        services.AddSingleton(userContextMock.Object);
        context.RequestServices = services.BuildServiceProvider();

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
        tenantMock.Verify(t => t.SetTenantAsync(It.IsAny<Guid>()), Times.Once);
        tenantMock.Verify(t => t.LicenseIsValid(), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_InvalidLicense_SetsForbiddenStatusCode()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var tenantMock = new Mock<ITenant>();
        var userContextMock = new Mock<IUserContext>();

        tenantMock.Setup(t => t.LicenseIsValid()).Returns(false);
        tenantMock.Setup(t => t.SetTenantAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);
        userContextMock.SetupGet(uc => uc.Tenant).Returns(Guid.NewGuid());

        var services = new ServiceCollection();
        services.AddSingleton(tenantMock.Object);
        services.AddSingleton(userContextMock.Object);
        context.RequestServices = services.BuildServiceProvider();

        static Task next(HttpContext ctx) => Task.CompletedTask;

        var middleware = new LicenseMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.Forbidden, context.Response.StatusCode);
        tenantMock.Verify(t => t.SetTenantAsync(It.IsAny<Guid>()), Times.Once);
        tenantMock.Verify(t => t.LicenseIsValid(), Times.Once);
    }
}