using Microsoft.Extensions.Hosting;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Middlewares;
using Microsoft.AspNetCore.Http;
using Moq;
using CodeDesignPlus.Net.Exceptions;
using FluentValidation;
using FluentValidation.Results;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.Rest;

public class ExceptionMiddlewareTests
{
    private static ExceptionMiddleware CreateMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware>? logger = null, IHostEnvironment? env = null, CoreOptions? coreOptions = null)
    {
        var hostEnvironment = new Mock<IHostEnvironment>();
        hostEnvironment.Setup(e => e.EnvironmentName).Returns("Development");

        logger ??= Mock.Of<ILogger<ExceptionMiddleware>>();
        env ??= hostEnvironment.Object;
        coreOptions ??= new CoreOptions
        {
            Business = "CodeDesignPlus",
            Contact = new Contact
            {
                Name = "CodeDesignPlus Team",
                Email = "supoort@codedesignplus.com",
            },
            Description = "CodeDesignPlus.Net.Microservice.Commons",
            Version = "1.0.0",
            Id = Guid.NewGuid(),
            ApiDocumentationBaseUrl = "https://docs/",
            AppName = "TestApp"
        };
        var options = Options.Create(coreOptions);

        return new ExceptionMiddleware(next, logger, env, options);
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_CallsNextAndDoesNotChangeStatusCode()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var called = false;
        Task next(HttpContext ctx)
        {
            called = true;
            return Task.CompletedTask;
        }
        var middleware = CreateMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(called);
        // Default status code is 200
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WhenValidationException_ReturnsProblemJson()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        static Task next(HttpContext ctx) => throw new ValidationException("Validation failed",
        [
            new ValidationFailure("name", "Name is required"),
            new ValidationFailure("age", "Age must be a positive number")
        ]);
        var middleware = CreateMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(400, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Contains("validation-error", body);
        Assert.Contains("invalid_params", body);
        Assert.Contains("name", body);
        Assert.Contains("age", body);
    }

    [Fact]
    public async Task InvokeAsync_WhenCodeDesignPlusException_ReturnsProblemJson()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var ex = new CodeDesignPlusException(Layer.Application, "001", "App error");
        Task next(HttpContext ctx) => throw ex;
        var middleware = CreateMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(400, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Contains("001", body);
        Assert.Contains("App error", body);
        Assert.Contains("TestApp", body);
    }

    [Fact]
    public async Task InvokeAsync_WhenException_ReturnsInternalServerErrorProblemJson()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        static Task next(HttpContext ctx) => throw new Exception("Something went wrong");
        var middleware = CreateMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Contains("internal-error", body);
        Assert.Contains("Internal Server Error", body);
        Assert.Contains("Something went wrong", body);
    }

    [Fact]
    public async Task InvokeAsync_WhenProduction_DoesNotIncludeExceptionMessage()
    {
        // Arrange
        var context = new DefaultHttpContext();
        //var env = Mock.Of<IHostEnvironment>(e => e.IsProduction() == true);
        var env = new Mock<IHostEnvironment>();
        env.Setup(e => e.EnvironmentName).Returns("Production");
        static Task next(HttpContext ctx) => throw new Exception("Sensitive error");
        var middleware = CreateMiddleware(next, env: env.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.DoesNotContain("Sensitive error", body);
        Assert.DoesNotContain("exception_message", body);
    }
}
