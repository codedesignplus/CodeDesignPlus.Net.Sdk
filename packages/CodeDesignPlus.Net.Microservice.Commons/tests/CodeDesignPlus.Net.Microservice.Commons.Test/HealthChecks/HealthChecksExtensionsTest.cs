using CodeDesignPlus.Net.Microservice.Commons.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.HealthChecks;

public class HealthChecksExtensionsTest
{
    [Fact]
    public void AddHealthChecksServices_ThrowsArgumentNullException_WhenServicesIsNull()
    {
        IServiceCollection services = null!;
        Assert.Throws<ArgumentNullException>(() => HealthChecksExtensions.AddHealthChecksServices(services));
    }

    [Fact]
    public async Task AddHealthChecksServices_AddsHealthCheckLive()
    {
        var server = CreateTestServer();
        var client = server.CreateClient();

        var response = await client.GetAsync("/health/live");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AddHealthChecksServices_AddsHealthCheckReady()
    {
        var server = CreateTestServer();
        var client = server.CreateClient();

        var response = await client.GetAsync("/health/ready");

        response.EnsureSuccessStatusCode();

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
    
    private static TestServer CreateTestServer()
    {
        return new TestServer(new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.UseHealthChecks();
                    endpoints.MapGet("/", context => context.Response.WriteAsync("Hello World!"));
                });

            })
            .ConfigureServices(services =>
            {
                services.AddRouting();
                services.AddHealthChecksServices();
            }));
    }
}
