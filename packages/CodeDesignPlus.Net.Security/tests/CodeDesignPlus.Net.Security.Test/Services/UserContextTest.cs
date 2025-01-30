using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Security.Test.Helpers.Server;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Security.Test.Services;

public class UserContextTest
{
    private readonly ServerAuth serverAuth;

    public UserContextTest()
    {
        this.serverAuth = new ServerAuth();
    }

    [Fact]
    public void State_Guids_Success()
    {
        // Arrange
        var idUserExpected = Guid.NewGuid();
        var nameExpected = "John Doe";
        var emailExpected = "john.doe@gmail.com";
        var tenantExpected = Guid.NewGuid().ToString();
        var applicationExpected = "CodeDesignPlus.Net.Security.Test";

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new(Abstractions.ClaimTypes.ObjectIdentifier, idUserExpected.ToString()),
                new(Abstractions.ClaimTypes.Name, nameExpected),
                new(Abstractions.ClaimTypes.Emails, emailExpected),
                new(Abstractions.ClaimTypes.Audience, applicationExpected),
            }))
        };

        httpContext.Request.Headers.Append("X-Tenant", tenantExpected);

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var idUser = userContext.GetClaim<Guid>(Abstractions.ClaimTypes.ObjectIdentifier);

        // Assert
        Assert.Equal(idUserExpected, idUser);
        Assert.Equal(idUserExpected, userContext.IdUser);
        Assert.Equal(nameExpected, userContext.Name);
        Assert.Contains(emailExpected, userContext.Emails);
        Assert.Equal(tenantExpected, userContext.Tenant.ToString());
        Assert.True(userContext.IsApplication);
        Assert.False(userContext.IsAuthenticated);
    }

    [Fact]
    public void GetHeader_Guid_Success()
    {
        // Arrange
        var tenantExpected = Guid.NewGuid();

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Append("X-Tenant", tenantExpected.ToString());

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var tenant = userContext.GetHeader<Guid>("X-Tenant");

        // Assert
        Assert.Equal(tenantExpected, tenant);
    }

    [Fact]
    public void GetHeader_ChangeTypeWithInteger_Success()
    {
        // Arrange
        var planExpected = 1;

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Append("X-Plan", planExpected.ToString());

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var plan = userContext.GetHeader<int>("X-Plan");

        // Assert
        Assert.Equal(planExpected, plan);
    }

    [Fact]
    public void GetHeader_NotExistHeader_ReturnNull()
    {
        // Arrange
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var plan = userContext.GetHeader<string>("X-Plan");

        // Assert
        Assert.Null(plan);
    }

    [Fact]
    public void GetClaim_NotExistClaim_ReturnNull()
    {
        // Arrange
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var idUser = userContext.GetClaim<string>(System.Security.Claims.ClaimTypes.NameIdentifier);

        // Assert
        Assert.Null(idUser);
    }

    [Fact]
    public async Task GetPasswordToken_ShouldReturnAccessToken()
    {
        var accessToken = await serverAuth.GetAccessTokenAsync();
        Assert.NotNull(accessToken);

        var claims = ServerAuth.GetClaims(accessToken);

        Assert.Contains(claims, c => c.Type == "given_name" && c.Value == "Jaramillo Jaramillo");
    }

    [Fact]
    public void GetTenant_HeaderTenantExists_ReturnsHeaderTenant()
    {
        // Arrange
        var tenantExpected = Guid.NewGuid();

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Append("X-Tenant", tenantExpected.ToString());

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var options = OptionsUtil.SecurityOptions;
        var eventContextMock = new Mock<IEventContext>();
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), eventContextMock.Object);

        // Act
        var tenant = userContext.Tenant;

        // Assert
        Assert.Equal(tenantExpected, tenant);
    }

    [Fact]
    public void GetTenant_HeaderTenantNotExists_ReturnsEventContextTenant()
    {
        // Arrange
        var tenantExpected = Guid.NewGuid();

        var httpContext = new DefaultHttpContext();

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var options = OptionsUtil.SecurityOptions;
        var eventContextMock = new Mock<IEventContext>();
        eventContextMock.Setup(e => e.Tenant).Returns(tenantExpected);
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), eventContextMock.Object);

        // Act
        var tenant = userContext.Tenant;

        // Assert
        Assert.Equal(tenantExpected, tenant);
    }

}
