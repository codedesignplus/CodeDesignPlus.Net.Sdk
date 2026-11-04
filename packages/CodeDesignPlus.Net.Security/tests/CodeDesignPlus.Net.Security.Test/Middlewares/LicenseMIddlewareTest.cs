using System.IO;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Security.Abstractions.Models;
using CodeDesignPlus.Net.Security.Middlewares;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json.Linq;
using NodaTime;

namespace CodeDesignPlus.Net.Security.Test.Middlewares;

public class LicenseMiddlewareTest
{
    private static readonly Guid TenantId = Guid.NewGuid();

    private static readonly Instant Expiration = Instant.FromUtc(2026, 11, 4, 5, 0);

    [Fact]
    public async Task InvokeAsync_ActiveLicense_CallsNextWithoutLicenseHeaders()
    {
        // Arrange
        var context = BuildContext(Standing(LicenseStatus.Active), HttpMethods.Post);
        var nextCalled = false;

        var middleware = new LicenseMiddleware(_ => { nextCalled = true; return Task.CompletedTask; }, Options());

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        Assert.False(context.Response.Headers.ContainsKey(LicenseHeaders.Status));
    }

    [Fact]
    public async Task InvokeAsync_LicenseInGrace_CallsNextAndMarksTheResponse()
    {
        // Arrange: una escritura dentro de la gracia pasa, con la franja de aviso.
        var context = BuildContext(Standing(LicenseStatus.Grace), HttpMethods.Post);
        var nextCalled = false;

        var middleware = new LicenseMiddleware(_ => { nextCalled = true; return Task.CompletedTask; }, Options());

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("grace", context.Response.Headers["X-License-Status"].ToString());
        Assert.Equal("2026-11-19T05:00:00Z", context.Response.Headers["X-License-Grace-Ends"].ToString());
    }

    [Theory]
    [InlineData("GET")]
    [InlineData("HEAD")]
    [InlineData("OPTIONS")]
    public async Task InvokeAsync_ReadOnlyLicenseAndRead_CallsNextMarkedAsReadOnly(string method)
    {
        // Arrange
        var context = BuildContext(Standing(LicenseStatus.ReadOnly), method);
        var nextCalled = false;

        var middleware = new LicenseMiddleware(_ => { nextCalled = true; return Task.CompletedTask; }, Options());

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("read-only", context.Response.Headers["X-License-Status"].ToString());
    }

    [Theory]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("PATCH")]
    [InlineData("DELETE")]
    public async Task InvokeAsync_ReadOnlyLicenseAndWrite_RespondsForbiddenWithLicenseExpired(string method)
    {
        // Arrange
        var context = BuildContext(Standing(LicenseStatus.ReadOnly), method, core: OptionsUtil.CoreOptions);
        var nextCalled = false;

        var middleware = new LicenseMiddleware(_ => { nextCalled = true; return Task.CompletedTask; }, Options());

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.False(nextCalled);
        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        Assert.Equal("read-only", context.Response.Headers["X-License-Status"].ToString());

        var body = ReadBody(context);

        Assert.Equal(403, body["status"]!.Value<int>());
        Assert.Equal("9402", body["extensions"]!["error_code"]!.Value<string>());
        Assert.Equal("None", body["extensions"]!["layer"]!.Value<string>());
        Assert.Equal("https://api.codedesignplus.com/docs/ms-test/none-9402", body["type"]!.Value<string>());
        Assert.Equal("The property's license has expired: it is read-only. Renew it in Settings › Organization.", body["detail"]!.Value<string>());
        Assert.Equal("License expired", body["title"]!.Value<string>());
    }

    [Fact]
    public async Task InvokeAsync_ReadOnlyLicenseAndSpanishRequest_RespondsInSpanish()
    {
        // Arrange
        var context = BuildContext(Standing(LicenseStatus.ReadOnly), HttpMethods.Post);

        var middleware = new LicenseMiddleware(_ => Task.CompletedTask, Options());

        ErrorLanguage.Current = "es";

        try
        {
            // Act
            await middleware.InvokeAsync(context);
        }
        finally
        {
            ErrorLanguage.Current = null;
        }

        // Assert
        var body = ReadBody(context);

        Assert.Equal("La licencia de la copropiedad venció: está en solo lectura. Renuévala en Configuración › Organización.", body["detail"]!.Value<string>());
        Assert.Equal("Licencia vencida", body["title"]!.Value<string>());
    }

    [Theory]
    [InlineData("/api/Subscription/renew")]
    [InlineData("/API/subscription")]
    public async Task InvokeAsync_ReadOnlyLicenseAndExemptPath_CallsNext(string path)
    {
        // Arrange: lo que necesita la copropiedad para pagar la renovacion no se bloquea.
        var tenant = new Mock<ITenant>();
        tenant.SetupGet(t => t.IsLoaded).Returns(true);
        tenant.Setup(t => t.GetLicenseStanding(It.IsAny<int>())).Returns(Standing(LicenseStatus.ReadOnly));

        var context = BuildContext(tenant, HttpMethods.Post, path);
        var nextCalled = false;

        var middleware = new LicenseMiddleware(_ => { nextCalled = true; return Task.CompletedTask; }, Options(exemptPaths: ["/api/Subscription"]));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        tenant.Verify(t => t.GetLicenseStanding(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_PathThatOnlySharesThePrefixText_IsNotExempt()
    {
        // Arrange: el prefijo se compara por segmentos, no por texto.
        var context = BuildContext(Standing(LicenseStatus.ReadOnly), HttpMethods.Post, "/api/SubscriptionPlans");
        var nextCalled = false;

        var middleware = new LicenseMiddleware(_ => { nextCalled = true; return Task.CompletedTask; }, Options(exemptPaths: ["/api/Subscription"]));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.False(nextCalled);
        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithoutTenant_CallsNextWithoutCheckingTheLicense()
    {
        // Arrange: la peticion no traia X-Tenant, asi que TenantContextMiddleware no cargo nada.
        var tenant = new Mock<ITenant>();
        tenant.SetupGet(t => t.IsLoaded).Returns(false);

        var context = BuildContext(tenant, HttpMethods.Post);
        var nextCalled = false;

        var middleware = new LicenseMiddleware(_ => { nextCalled = true; return Task.CompletedTask; }, Options());

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        tenant.Verify(t => t.GetLicenseStanding(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_ExemptTenant_CallsNextWithoutCheckingTheLicense()
    {
        // Arrange: el tenant de la plataforma nunca queda en solo lectura.
        var tenant = new Mock<ITenant>();
        tenant.SetupGet(t => t.IsLoaded).Returns(true);
        tenant.Setup(t => t.GetLicenseStanding(It.IsAny<int>())).Returns(Standing(LicenseStatus.ReadOnly));

        var context = BuildContext(tenant, HttpMethods.Post);
        var nextCalled = false;

        var middleware = new LicenseMiddleware(_ => { nextCalled = true; return Task.CompletedTask; }, Options(exemptTenants: [TenantId]));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        tenant.Verify(t => t.GetLicenseStanding(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_GrpcCall_CallsNextEvenIfReadOnly()
    {
        // Arrange: gRPC es POST, pero lo llama otro micro y no la pantalla.
        var tenant = new Mock<ITenant>();
        tenant.SetupGet(t => t.IsLoaded).Returns(true);
        tenant.Setup(t => t.GetLicenseStanding(It.IsAny<int>())).Returns(Standing(LicenseStatus.ReadOnly));

        var context = BuildContext(tenant, HttpMethods.Post, "/Licenses.Tenant/GetLicense");
        context.Request.ContentType = "application/grpc";
        var nextCalled = false;

        var middleware = new LicenseMiddleware(_ => { nextCalled = true; return Task.CompletedTask; }, Options());

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        Assert.False(context.Response.Headers.ContainsKey(LicenseHeaders.Status));
    }

    [Fact]
    public async Task InvokeAsync_ConfiguredGraceDays_AreThoseAskedToTheTenant()
    {
        // Arrange
        var tenant = new Mock<ITenant>();
        tenant.SetupGet(t => t.IsLoaded).Returns(true);
        tenant.Setup(t => t.GetLicenseStanding(30)).Returns(Standing(LicenseStatus.Active));

        var context = BuildContext(tenant, HttpMethods.Get);

        var middleware = new LicenseMiddleware(_ => Task.CompletedTask, Options(graceDays: 30));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        tenant.Verify(t => t.GetLicenseStanding(30), Times.Once);
    }

    private static LicenseStanding Standing(LicenseStatus status)
    {
        return new LicenseStanding(status, Expiration, Expiration + Duration.FromDays(15));
    }

    private static IOptions<SecurityOptions> Options(string[]? exemptPaths = null, Guid[]? exemptTenants = null, int graceDays = 15)
    {
        return Microsoft.Extensions.Options.Options.Create(new SecurityOptions
        {
            GraceDays = graceDays,
            LicenseExemptPaths = exemptPaths ?? [],
            LicenseExemptTenants = exemptTenants ?? []
        });
    }

    private static DefaultHttpContext BuildContext(LicenseStanding standing, string method, string path = "/api/Unit", CoreOptions? core = null)
    {
        var tenant = new Mock<ITenant>();
        tenant.SetupGet(t => t.IsLoaded).Returns(true);
        tenant.Setup(t => t.GetLicenseStanding(It.IsAny<int>())).Returns(standing);

        return BuildContext(tenant, method, path, core);
    }

    private static DefaultHttpContext BuildContext(Mock<ITenant> tenant, string method, string path = "/api/Unit", CoreOptions? core = null)
    {
        var userContext = new Mock<IUserContext>();
        userContext.SetupGet(u => u.Tenant).Returns(TenantId);

        var services = new ServiceCollection();
        services.AddSingleton(tenant.Object);
        services.AddSingleton(userContext.Object);

        if (core is not null)
            services.AddSingleton(Microsoft.Extensions.Options.Options.Create(core));

        var context = new DefaultHttpContext { RequestServices = services.BuildServiceProvider() };
        context.Request.Method = method;
        context.Request.Path = path;
        context.Response.Body = new MemoryStream();

        return context;
    }

    private static JObject ReadBody(DefaultHttpContext context)
    {
        context.Response.Body.Position = 0;

        using var reader = new StreamReader(context.Response.Body);

        return JObject.Parse(reader.ReadToEnd());
    }
}
