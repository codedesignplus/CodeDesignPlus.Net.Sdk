using System.Net;
using CodeDesignPlus.Net.Security.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeDesignPlus.Net.Security.Test.Middlewares;

public class TenantContextMiddlewareTest
{
    [Fact]
    public async Task InvokeAsync_TenantIsPresent_LoadsItAndContinues()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenantMock = new Mock<ITenant>();
        var context = BuildContext(tenantMock, tenantId);
        var nextCalled = false;

        Task next(HttpContext ctx)
        {
            nextCalled = true;
            return Task.CompletedTask;
        }

        var middleware = new TenantContextMiddleware((RequestDelegate)next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        tenantMock.Verify(t => t.SetAsync(tenantId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithoutTenant_ContinuesWithoutLoading()
    {
        // Arrange: es el caso normal de un entrypoint gRPC al que no le mandan X-Tenant.
        var tenantMock = new Mock<ITenant>();
        var context = BuildContext(tenantMock, Guid.Empty);
        var nextCalled = false;

        Task next(HttpContext ctx)
        {
            nextCalled = true;
            return Task.CompletedTask;
        }

        var middleware = new TenantContextMiddleware((RequestDelegate)next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
        tenantMock.Verify(t => t.SetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_TenantCannotBeResolved_SetsServiceUnavailable()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenantMock = new Mock<ITenant>();
        tenantMock.Setup(t => t.SetAsync(tenantId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new SecurityException("boom"));

        var context = BuildContext(tenantMock, tenantId);
        var nextCalled = false;

        Task next(HttpContext ctx)
        {
            nextCalled = true;
            return Task.CompletedTask;
        }

        var middleware = new TenantContextMiddleware((RequestDelegate)next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.False(nextCalled);
        Assert.Equal((int)HttpStatusCode.ServiceUnavailable, context.Response.StatusCode);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("1234")]
    [InlineData("not-a-guid-at-all")]
    public async Task InvokeAsync_TenantHeaderIsNotAGuid_RejectsTheRequest(string header)
    {
        // Arrange: X-Tenant siempre es un GUID. Antes se leia como Guid.Empty y la peticion seguia sin tenant
        // (pendings/028).
        var tenantMock = new Mock<ITenant>();
        var context = BuildContext(tenantMock, Guid.Empty);
        context.Request.Headers["X-Tenant"] = header;
        var nextCalled = false;

        Task next(HttpContext ctx)
        {
            nextCalled = true;
            return Task.CompletedTask;
        }

        var middleware = new TenantContextMiddleware((RequestDelegate)next);

        // Act & Assert: sale como error de catalogo, que el ExceptionMiddleware responde con 400.
        var exception = await Assert.ThrowsAsync<CodeDesignPlus.Net.Exceptions.CodeDesignPlusException>(() => middleware.InvokeAsync(context));

        Assert.Equal(Errors.InvalidTenantHeader.GetCode(), exception.Code);
        Assert.False(nextCalled);
        tenantMock.Verify(t => t.SetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_TenantHeaderIsAGuid_IsAccepted()
    {
        // Arrange: el mismo GUID en la cabecera y en el contexto, que es lo que manda el frontend.
        var tenantId = Guid.NewGuid();
        var tenantMock = new Mock<ITenant>();
        var context = BuildContext(tenantMock, tenantId);
        context.Request.Headers["X-Tenant"] = tenantId.ToString();

        var middleware = new TenantContextMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        tenantMock.Verify(t => t.SetAsync(tenantId, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static DefaultHttpContext BuildContext(Mock<ITenant> tenantMock, Guid tenantId)
    {
        var userContextMock = new Mock<IUserContext>();
        userContextMock.SetupGet(uc => uc.Tenant).Returns(tenantId);

        var services = new ServiceCollection();
        services.AddSingleton(tenantMock.Object);
        services.AddSingleton(userContextMock.Object);
        services.AddSingleton(Mock.Of<ILogger<TenantContextMiddleware>>());

        return new DefaultHttpContext { RequestServices = services.BuildServiceProvider() };
    }
}
