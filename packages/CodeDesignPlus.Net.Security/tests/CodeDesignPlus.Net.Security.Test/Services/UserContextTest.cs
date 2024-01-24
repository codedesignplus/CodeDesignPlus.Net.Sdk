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
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options));

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
    public void GetHeader_NotExistHeader_ReturnNull()
    {
        // Arrange
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options));

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
        var userContext = new UserContext(httpContextAccessor, O.Options.Create(options));

        // Act
        var idUser = userContext.GetClaim<string>(System.Security.Claims.ClaimTypes.NameIdentifier);

        // Assert
        Assert.Null(idUser);
    }
}
