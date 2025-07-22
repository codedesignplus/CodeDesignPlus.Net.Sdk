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
            User = new ClaimsPrincipal(new ClaimsIdentity([
                new("userId", idUserExpected.ToString()),
                new(Abstractions.ClaimTypes.Name, nameExpected),
                new(Abstractions.ClaimTypes.Emails, emailExpected),
                new(Abstractions.ClaimTypes.Audience, applicationExpected),
            ]))
        };

        httpContext.Request.Headers.Append("X-Tenant", tenantExpected);

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var idUser = userContext.GetClaim<Guid>("userId");

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
    public void GetHeader_HttpContextIsNull_ReturnNull()
    {
        // Arrange
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = null
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


    [Fact]
    public void AccessToken_HeaderWithBearer_ReturnsTokenWithoutBearerPrefix()
    {
        // Arrange
        var token = Guid.NewGuid().ToString();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Append("Authorization", $"Bearer {token}");

        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var accessToken = userContext.AccessToken;

        // Assert
        Assert.Equal(token, accessToken);
    }

    [Fact]
    public void AccessToken_HeaderWithoutBearer_ReturnsRawHeader()
    {
        // Arrange
        var token = Guid.NewGuid().ToString();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Append("Authorization", token);

        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var accessToken = userContext.AccessToken;

        // Assert
        Assert.Equal(token, accessToken);
    }

    [Fact]
    public void AccessToken_HeaderNotPresent_ReturnsNull()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var accessToken = userContext.AccessToken;

        // Assert
        Assert.Null(accessToken);
    }

    [Fact]
    public void UserAgent_HeaderPresent_ReturnsHeaderValue()
    {
        // Arrange
        var userAgent = "CustomUserAgent/1.0";
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Append("User-Agent", userAgent);

        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var result = userContext.UserAgent;

        // Assert
        Assert.Equal(userAgent, result);
    }

    [Fact]
    public void UserAgent_HeaderNotPresent_ReturnsDefault()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var result = userContext.UserAgent;

        // Assert
        Assert.Equal("CodeDesignPlus/Client", result);
    }

    [Fact]
    public void IpAddress_XForwardedForHeaderPresent_ReturnsHeaderValue()
    {
        // Arrange
        var ip = "203.0.113.42";
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Append("X-Forwarded-For", ip);

        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var result = userContext.IpAddress;

        // Assert
        Assert.Equal(ip, result);
    }

    [Fact]
    public void IpAddress_XForwardedForNotPresent_UsesRemoteIpAddress()
    {
        // Arrange
        var ip = "192.168.1.100";
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ip);

        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var result = userContext.IpAddress;

        // Assert
        Assert.Equal(ip, result);
    }

    [Fact]
    public void IpAddress_NoHeaderOrRemoteIp_ReturnsEmptyString()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var result = userContext.IpAddress;

        // Assert
        Assert.Equal(string.Empty, result);
    }


    [Fact]
    public void IpAddress_HttpContextIsNull_ReturnsEmptyString()
    {
        // Arrange
        var httpContextAccessor = new HttpContextAccessor { HttpContext = null };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var result = userContext.IpAddress;

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void IpAddress_RemoteIpAddressIsNull_ReturnsEmptyString()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = null;

        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var result = userContext.IpAddress;

        // Assert
        Assert.Equal(string.Empty, result);
    }


    [Fact]
    public void Oid_WithObjectIdentifierClaim_ReturnsGuid()
    {
        // Arrange
        var oidExpected = Guid.NewGuid();
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
            new(Abstractions.ClaimTypes.ObjectIdentifier, oidExpected.ToString())
            ]))
        };
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var oid = userContext.Oid;

        // Assert
        Assert.Equal(oidExpected.ToString(), oid);
    }

    [Fact]
    public void Oid_NotExist_ReturnsNull()
    {
        // Arrange
        var oidExpected = Guid.NewGuid();
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
            new(Abstractions.ClaimTypes.Subject, oidExpected.ToString())
            ]))
        };
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var oid = userContext.Oid;

        // Assert
        Assert.Null(oid);
    }


    [Fact]
    public void IdUser_WithValue_ReturnsGuid()
    {
        // Arrange
        var idUserExpected = Guid.NewGuid();
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
            new("userId", idUserExpected.ToString())
            ]))
        };
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var idUser = userContext.IdUser;

        // Assert
        Assert.Equal(idUserExpected, idUser);
    }

    [Fact]
    public void Roles_WithGroupsClaim_ReturnsRolesArray()
    {
        // Arrange
        var roles = new[] { "Admin", "User" };
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new(Abstractions.ClaimTypes.Groups, roles[0]),
                new(Abstractions.ClaimTypes.Groups, roles[1])
            ]))
        };
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var result = userContext.Roles;

        // Assert
        Assert.NotNull(result);
        Assert.Contains(roles[0], result);
        Assert.Contains(roles[1], result);
    }

    [Fact]
    public void Roles_WithoutGroupsClaim_ReturnsNull()
    {
        // Arrange
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity())
        };
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options), Mock.Of<IEventContext>());

        // Act
        var result = userContext.Roles;

        // Assert
        Assert.Empty(result);
    }

}