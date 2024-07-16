using CodeDesignPlus.Net.xUnit.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using CodeDesignPlus.Net.Security.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;

namespace CodeDesignPlus.Net.Security.Test.Helpers.Server;

public class ServerApi
{
    public readonly TestServer Server;

    public ServerApi(Action<JwtBearerOptions>? options = null)
    {
        var configuration = ConfigurationUtil.GetConfiguration(new { Security = OptionsUtil.SercurityOptionsLocalhost });
       
        this.Server = new TestServer(new WebHostBuilder()
            .UseConfiguration(configuration)
            .ConfigureServices(x =>
            {
                x.AddSecurity(configuration, options);
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
    }
}
