using CodeDesignPlus.Net.Security.Extensions;
using CodeDesignPlus.Net.Security.Test.Helpers.Server;
using CodeDesignPlus.Net.xUnit.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Http.Headers;

namespace CodeDesignPlus.Net.Security.Test.Extensions;

public class ServiceCollectionExtensionsTest
{

    [Fact]
    public void AddSecurity_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddSecurity(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddSecurity_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddSecurity(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddSecurity_SectionNotExist_SecurityException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<SecurityException>(() => serviceCollection.AddSecurity(configuration));

        // Assert
        Assert.Equal($"The section {SecurityOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddSecurity_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { Security = OptionsUtil.SecurityOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddSecurity(configuration);

        // Assert
        var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IUserContext));

        Assert.NotNull(libraryService);
        Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
        Assert.Equal(typeof(UserContext), libraryService.ImplementationType);
    }

    [Fact]
    public void AddSecurity_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { Security = OptionsUtil.SecurityOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddSecurity(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<SecurityOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);
    }

    [Fact]
    public async Task Middleware_Authentication_JwtBearer()
    {
        // Arrange
        var serverAuth = new ServerAuth();
        var serverApi = new ServerApi();

        var accessToken = await serverAuth.GetAccessTokenAsync();

        // Act
        var client = serverApi.Server.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.SendAsync(request);

        var json = await response.Content.ReadAsStringAsync();

        var info = JsonSerializer.Deserialize<UserInfo>(json);

        // Assert
        Assert.NotNull(client);
        Assert.NotNull(response);
        Assert.NotNull(info);
        Assert.NotNull(info.User);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Jaramillo Jaramillo Lina Marcela", info.User.Name);
        Assert.Equal("Jaramillo Jaramillo", info!.User.FirstName);
        Assert.Equal("Lina Marcela", info!.User.LastName);
        Assert.Equal("Bogotá", info!.User.City);
        Assert.Equal("Colombia", info!.User.Country);
        Assert.Equal("111611", info!.User.PostalCode);
        Assert.Equal("Calle Siempre Viva", info!.User.StreetAddress);
        Assert.Equal("Bogotá D.C", info!.User.State);
        Assert.Equal("Arquitecto", info!.User.JobTitle);
        Assert.Equal("802b1e5c-6e40-4e01-8095-c735b4c9959e", info.User.IdUser);
        Assert.Contains("codedesignplus@outlook.com", info!.User.Emails);
    }

    [Fact]
    public async Task Middleware_CheckOptions_JwtBearer()
    {
        // Arrange
        var optionsIsInvoke = false;
        var serverAuth = new ServerAuth();
        var serverApi = new ServerApi(x =>
        {
            optionsIsInvoke = true;
        });

        var accessToken = await serverAuth.GetAccessTokenAsync();

        // Act
        var client = serverApi.Server.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.SendAsync(request);

        var json = await response.Content.ReadAsStringAsync();

        var info = JsonSerializer.Deserialize<UserInfo>(json);

        // Assert
        Assert.True(optionsIsInvoke);
        Assert.NotNull(client);
        Assert.NotNull(response);
        Assert.NotNull(info);
        Assert.NotNull(info.User);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Jaramillo Jaramillo Lina Marcela", info.User.Name);
        Assert.Equal("Jaramillo Jaramillo", info!.User.FirstName);
        Assert.Equal("Lina Marcela", info!.User.LastName);
        Assert.Equal("Bogotá", info!.User.City);
        Assert.Equal("Colombia", info!.User.Country);
        Assert.Equal("111611", info!.User.PostalCode);
        Assert.Equal("Calle Siempre Viva", info!.User.StreetAddress);
        Assert.Equal("Bogotá D.C", info!.User.State);
        Assert.Equal("Arquitecto", info!.User.JobTitle);
        Assert.Equal("802b1e5c-6e40-4e01-8095-c735b4c9959e", info.User.IdUser);
        Assert.Contains("codedesignplus@outlook.com", info!.User.Emails);
    }

    [Theory]
    [InlineData(typeof(SecurityTokenExpiredException), "Token-Expired", "Token has expired")]
    [InlineData(typeof(SecurityTokenInvalidAudienceException), "Token-InvalidAudience", "Invalid audience")]
    [InlineData(typeof(SecurityTokenInvalidIssuerException), "Token-InvalidIssuer", "Invalid issuer")]
    [InlineData(typeof(SecurityTokenValidationException), "Token-Validation", "Token validation failed")]
    [InlineData(typeof(SecurityTokenException), "Token-Exception", "General token exception")]
    [InlineData(typeof(Exception), "Internal Error", "An internal error occurred.")]
    public async Task AuthenticationFailed_ShouldHandleExceptions(Type exceptionType, string expectedHeader, string expectedMessage)
    {
        // Arrange
        var exception = (Exception)Activator.CreateInstance(exceptionType, expectedMessage)!;

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var context = CreateAuthenticationFailedContext(httpContext, exception);

        // Act
        await ServiceCollectionExtensions.AuthenticationFailed(context);

        // Assert
        Assert.Contains(httpContext.Response.Headers, x => x.Key == expectedHeader && x.Value == "true");
        Assert.Equal(StatusCodes.Status401Unauthorized, httpContext.Response.StatusCode);


        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await GetResponseBodyAsync(httpContext.Response.Body);

        var responseJson = JsonSerializer.Deserialize<Response>(responseBody)!;

        Assert.NotNull(responseJson);
        Assert.Equal(expectedMessage, responseJson.Message);
    }

    private static AuthenticationFailedContext CreateAuthenticationFailedContext(HttpContext httpContext, Exception exception)
    {
        var scheme = new AuthenticationScheme(JwtBearerDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme, typeof(JwtBearerHandler));

        var authenticationFailedContext = new AuthenticationFailedContext(httpContext, scheme, new JwtBearerOptions())
        {
            Exception = exception
        };

        return authenticationFailedContext;
    }

    private static async Task<string> GetResponseBodyAsync(Stream body)
    {
        using var reader = new StreamReader(body, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }
}
