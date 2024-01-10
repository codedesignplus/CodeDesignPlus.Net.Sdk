using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Security.Test.Services;

public class UserContextTest
{
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
                new(System.Security.Claims.ClaimTypes.NameIdentifier, idUserExpected.ToString()) ,
                new(System.Security.Claims.ClaimTypes.Name, nameExpected) ,
                new(System.Security.Claims.ClaimTypes.Email, emailExpected) ,
                new(System.Security.Claims.ClaimTypes.Sid, applicationExpected) ,

            }))
        };

        httpContext.Request.Headers.Append("X-Tenant", tenantExpected);

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext<Guid, Guid>(httpContextAccessor, O.Options.Create(options));

        // Act
        var idUser = userContext.GetClaim<Guid>(System.Security.Claims.ClaimTypes.NameIdentifier);

        // Assert
        Assert.Equal(idUserExpected, idUser);
        Assert.Equal(idUserExpected, userContext.IdUser);
        Assert.Equal(nameExpected, userContext.Name);
        Assert.Contains(emailExpected, userContext.Emails);
        Assert.Equal(tenantExpected, userContext.Tenant.ToString());
        Assert.True(userContext.IsApplication);
        Assert.True(userContext.IsAuthenticated);
    }

    [Fact]
    public void State_Strings_Success()
    {
        // Arrange
        var idUserExpected = new Random().Next(1, 100).ToString();
        var idLicenseExpected = new Random().Next(1, 100).ToString();
        var nameExpected = "John Doe";
        var emailExpected = "john.doe@gmail.com";
        var tenantExpected = Guid.NewGuid().ToString();
        var applicationExpected = "CodeDesignPlus.Net.Security.Test";

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new(System.Security.Claims.ClaimTypes.NameIdentifier, idUserExpected.ToString()) ,
                new(System.Security.Claims.ClaimTypes.Name, nameExpected) ,
                new(System.Security.Claims.ClaimTypes.Email, emailExpected) ,
                new(System.Security.Claims.ClaimTypes.Sid, applicationExpected) ,

            }))
        };

        httpContext.Request.Headers.Append("X-Tenant", tenantExpected);
        httpContext.Request.Headers.Append("X-License", idLicenseExpected);

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext<string, Guid>(httpContextAccessor, O.Options.Create(options));

        // Act
        var idUser = userContext.GetClaim<string>(System.Security.Claims.ClaimTypes.NameIdentifier);

        // Assert
        Assert.Equal(idUserExpected, idUser);
        Assert.Equal(idUserExpected, userContext.IdUser);
        Assert.Equal(nameExpected, userContext.Name);
        Assert.Contains(emailExpected, userContext.Emails);
        Assert.Equal(tenantExpected, userContext.Tenant.ToString());
        Assert.Equal(idLicenseExpected, userContext.GetHeader<string>("X-License"));
        Assert.Null(userContext.GetHeader<string>("X-Plan"));
        Assert.True(userContext.IsApplication);
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
        var userContext = new UserContext<string, Guid>(httpContextAccessor, O.Options.Create(options));

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
        var userContext = new UserContext<string, Guid>(httpContextAccessor, O.Options.Create(options));

        // Act
        var idUser = userContext.GetClaim<string>(System.Security.Claims.ClaimTypes.NameIdentifier);

        // Assert
        Assert.Null(idUser);
    }
}
