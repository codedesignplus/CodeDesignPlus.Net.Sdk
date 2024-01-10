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
        var userContext = new UserContext<Guid, Guid>(httpContextAccessor, O.Options.Create(options));

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
    public void State_Strings_Success()
    {
        // Arrange
        var idUserExpected = new Random().Next(1, 100).ToString();
        var idLicenseExpected = new Random().Next(1, 100).ToString();
        var name = "John Doe";
        var firstName = "John";
        var lastName = "Doe";
        var city = "Bogota";
        var country = "Colombia";
        var postalCode = "110911";
        var streetAddress = "Calle 123";
        var jobTitle = "Developer";
        var state = "Bogota";
        var email = "john.doe@gmail.com";
        var tenant = Guid.NewGuid().ToString();
        var applicationExpected = "CodeDesignPlus.Net.Security.Test";

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new(Abstractions.ClaimTypes.ObjectIdentifier, idUserExpected.ToString()),
                new(Abstractions.ClaimTypes.Name, name),
                new(Abstractions.ClaimTypes.Emails, email),
                new(Abstractions.ClaimTypes.Audience, applicationExpected),
                new(Abstractions.ClaimTypes.FirstName, firstName),
                new(Abstractions.ClaimTypes.LastName, lastName),
                new(Abstractions.ClaimTypes.City, city),
                new(Abstractions.ClaimTypes.Country, country),
                new(Abstractions.ClaimTypes.PostalCode, postalCode),
                new(Abstractions.ClaimTypes.StreetAddress, streetAddress),
                new(Abstractions.ClaimTypes.JobTitle, jobTitle),
                new(Abstractions.ClaimTypes.State, state),

            }))
        };

        httpContext.Request.Headers.Append("X-Tenant", tenant);
        httpContext.Request.Headers.Append("X-License", idLicenseExpected);

        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };
        var options = OptionsUtil.SecurityOptions;
        var userContext = new UserContext<string, Guid>(httpContextAccessor, O.Options.Create(options));

        // Act
        var idUser = userContext.GetClaim<string>(Abstractions.ClaimTypes.ObjectIdentifier);

        // Assert
        Assert.Equal(idUserExpected, idUser);
        Assert.Equal(idUserExpected, userContext.IdUser);
        Assert.Equal(name, userContext.Name);
        Assert.Contains(email, userContext.Emails);
        Assert.Equal(firstName, userContext.FirstName);
        Assert.Equal(lastName, userContext.LastName);
        Assert.Equal(city, userContext.City);
        Assert.Equal(country, userContext.Country);
        Assert.Equal(postalCode, userContext.PostalCode);
        Assert.Equal(streetAddress, userContext.StreetAddress);
        Assert.Equal(jobTitle, userContext.JobTitle);
        Assert.Equal(state, userContext.State);
        Assert.Equal(tenant, userContext.Tenant.ToString());
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
