using System.Security.Claims;
using IdentityModel.Client;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace CodeDesignPlus.Net.Security.Test.Helpers.Server;

public class ServerAuth
{
    public readonly TestServer Server;
    public readonly HttpClient ClientAuth;

    public ServerAuth()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddControllers();
                services.AddIdentityServer()
                    .AddInMemoryClients(
                    [
                        new Client
                        {
                            ClientId = "password-client",
                            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                            ClientSecrets = { new Secret("secret".Sha256()) },
                            AllowedScopes = { "api1", "profile" },
                            AlwaysIncludeUserClaimsInIdToken = true
                        }
                    ])
                    .AddInMemoryApiScopes(
                    [
                        new ApiScope("api1", "Api-Demo")
                    ])
                    .AddInMemoryApiResources(
                    [
                        new ApiResource("api1", "Api-Demo") { Scopes = { "api1" } }
                    ])
                    .AddTestUsers(
                    [
                        new TestUser
                        {
                            SubjectId = "1",
                            Username = "test",
                            Password = "password"
                        }
                    ])
                    .AddProfileService<CustomProfileService>()
                    .AddDeveloperSigningCredential();

                services.AddLogging(config =>
                {
                    config.AddConsole();
                    config.AddDebug();
                });
            })
            .Configure(app =>
            {
                var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

                app.UseRouting();
                app.UseIdentityServer();
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            });

        Server = new TestServer(builder);
        ClientAuth = Server.CreateClient();
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var disco = await ClientAuth.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = "http://localhost",
            Policy = { RequireHttps = false }
        });

        if (disco.IsError)
            throw new Exception(disco.Error);

        var tokenResponse = await ClientAuth.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = "password-client",
            ClientSecret = "secret",
            UserName = "test",
            Password = "password",
            Scope = "api1"
        });

        if (tokenResponse.IsError)
            throw new Exception(tokenResponse.Error);

        if(string.IsNullOrEmpty(tokenResponse.AccessToken))
            throw new Exception("The access token is empty.");

        return tokenResponse.AccessToken;
    }

    public static IEnumerable<Claim> GetClaims(string accessToken)
    {
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(accessToken);

        return token.Claims;
    }
}
