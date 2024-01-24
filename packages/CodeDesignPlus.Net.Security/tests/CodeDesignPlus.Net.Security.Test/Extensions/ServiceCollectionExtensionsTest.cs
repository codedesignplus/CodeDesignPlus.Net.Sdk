using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using CodeDesignPlus.Net.xUnit.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Security.Extensions;

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
        var configuration = ConfigurationUtil.GetConfiguration(new { Security = OptionsUtil.SecurityOptionsAzure });

        var server = new TestServer(new WebHostBuilder()
            .UseConfiguration(configuration)
            .ConfigureServices(x =>
            {
                x.AddSecurity(configuration, x =>
                {
                    x.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // Log the error
                            Console.WriteLine(context.Exception);
                            return Task.CompletedTask;
                        }
                    };
                });
                x.AddRouting();
            })
            .Configure(x =>
            {
                x.UseRouting();
                x.UseAuth();

                x.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/", async context =>
                    {
                        var userContext = context.RequestServices.GetRequiredService<IUserContext>();

                        var claims = context.User.Claims.ToDictionary(x => x.Type, x => x.Value);

                        var info = new
                        {
                            User = new
                            {
                                userContext.IdUser,
                                userContext.Name,
                                userContext.FirstName,
                                userContext.LastName,
                                userContext.City,
                                userContext.Country,
                                userContext.PostalCode,
                                userContext.StreetAddress,
                                userContext.State,
                                userContext.JobTitle,
                                userContext.Emails,
                                userContext.Tenant,
                                userContext.IsApplication,
                                userContext.IsAuthenticated
                            },
                            Claims = claims
                        };

                        var json = JsonSerializer.Serialize(info);

                        var data = Encoding.UTF8.GetBytes(json);

                        await context.Response.Body.WriteAsync(data);
                    }).RequireAuthorization();

                    endpoints.Map("/insecure", async context =>
                    {
                        var data = Encoding.UTF8.GetBytes("Insecure!");

                        await context.Response.Body.WriteAsync(data);
                    }).AllowAnonymous();
                });
            })
        );

        // Act
        var client = server.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsiLCJ0eXAiOiJKV1QifQ.eyJ2ZXIiOiIxLjAiLCJpc3MiOiJodHRwczovL2NvZGVkZXNpZ25wbHVzLmIyY2xvZ2luLmNvbS8zNDYxZTMxMS1hNjZlLTQ2YWItYWZkZi0yYmJmYjcyYTVjYjAvdjIuMC8iLCJzdWIiOiI4MDJiMWU1Yy02ZTQwLTRlMDEtODA5NS1jNzM1YjRjOTk1OWUiLCJhdWQiOiI3ZjJhZWExMC1iNjNjLTQ0Y2UtODU4MC03OGFiM2U1MTg5YWEiLCJleHAiOjE3MDQ4NTU3NTcsIm5vbmNlIjoiZGVmYXVsdE5vbmNlIiwiaWF0IjoxNzA0ODUyMTU3LCJhdXRoX3RpbWUiOjE3MDQ4NTIxNTcsIm9pZCI6IjgwMmIxZTVjLTZlNDAtNGUwMS04MDk1LWM3MzViNGM5OTU5ZSIsImNpdHkiOiJCb2dvdMOhIiwiY291bnRyeSI6IkNvbG9tYmlhIiwiZ2l2ZW5fbmFtZSI6IldpbHpvbiBDYW1pbG8iLCJmYW1pbHlfbmFtZSI6Ikxpc2Nhbm8gR2FsaW5kbyIsIm5hbWUiOiJXaWx6b24gQ2FtaWxvIExpc2Nhbm8gR2FsaW5kbyIsInBvc3RhbENvZGUiOiIxMTE2MTEiLCJzdHJlZXRBZGRyZXNzIjoiQ2FsbGUgM2EgIyA1M2MtMTMiLCJzdGF0ZSI6IkJvZ290w6EgRC5DIiwiam9iVGl0bGUiOiJBcnF1aXRlY3RvIiwiZW1haWxzIjpbImNvZGVkZXNpZ25wbHVzQG91dGxvb2suY29tIl0sInRmcCI6IkIyQ18xX3NpZ25pbiIsIm5iZiI6MTcwNDg1MjE1N30.JOktTfq1VIqxzgAKyAd8mI_46GLhnm20XFyDWOU1fIJGSlHJSKPcg1vCsircLBwfgrtU4vXmUfqz2tg5UTGxNc6LTwuKTkFWbjwN-qq5TQnkjpPudW6EoYkOb7ilij-y9qPMA_9fa7mWdn3vv_LqDyJAvqXwrFUXh-uSHtU4kl1U7IcWzPQHTqE6klGmFYvcbc0Dh_p-I1B714styX8a8DgjPe_HGEwNVR9kEf_CDc4eUk0O7P3ayvUUNw99SmyAQuwOo7fsHLAED8D6Sqg5Wc3rfSPKhsJWHUpMFKUl-B40HQtYSz9pRd6KFoOzGgpJqkUnywyPaB5gxINrtiob2Q");

        var response = await client.SendAsync(request);

        var json = await response.Content.ReadAsStringAsync();

        var info = JsonSerializer.Deserialize<UserInfo>(json);

        // Assert
        Assert.NotNull(client);
        Assert.NotNull(response);
        Assert.NotNull(info);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Wilzon Camilo Liscano Galindo", info!.User.Name);
        Assert.Equal("Wilzon Camilo", info!.User.FirstName);
        Assert.Equal("Liscano Galindo", info!.User.LastName);
        Assert.Equal("Bogotá", info!.User.City);
        Assert.Equal("Colombia", info!.User.Country);
        Assert.Equal("111611", info!.User.PostalCode);
        Assert.Equal("Calle 3a # 53c-13", info!.User.StreetAddress);
        Assert.Equal("Bogotá D.C", info!.User.State);
        Assert.Equal("Arquitecto", info!.User.JobTitle);
        Assert.Contains("codedesignplus@outlook.com", info!.User.Emails);
    }
}
