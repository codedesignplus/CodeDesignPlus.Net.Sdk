using System;
using CodeDesignPlus.Net.Exceptions.Models;
using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Moq;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.Rest;

public class MiddlewaresExtensionTest
{
    [Fact]
    public void UseExceptionMiddleware_Should_Add_ExceptionMiddleware()
    {
        // Arrange
        var appMock = new Mock<IApplicationBuilder>();

        // Act
        var result = MiddlewareExtensions.UseExceptionMiddleware(appMock.Object);

        // Assert
        appMock.Verify(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once);
    }

    [Fact]
    public async Task UseCodeErrors_ReturnErrors()
    {
        var server = CreateTestServer();
        var client = server.CreateClient();

        var response = await client.GetAsync("/errors");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var errors = CodeDesignPlus.Net.Serializers.JsonSerializer.Deserialize<List<ErrorDetail>>(content);

        Assert.NotNull(errors);
        Assert.NotEmpty(errors);
        Assert.Contains(errors, e => e.Code == "101" && e.Message == "Custom error message");
    }


    private static TestServer CreateTestServer()
    {
        return new TestServer(new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.UseCodeErrors();
                    endpoints.MapGet("/", context => context.Response.WriteAsync("Hello World!"));
                });

            })
            .ConfigureServices(services =>
            {
                services.AddRouting();
            }));
    }
}
