using Moq;
using System.Net;
using Microsoft.AspNetCore.Http;
using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Middlewares;
using FluentValidation;
using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.Rest;

public class ExceptionMiddlwareTest
{

    [Fact]
    public async Task InvokeAsync_NoException_ReturnsOk()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var mockRequestDelegate = new Mock<RequestDelegate>();
        var middleware = new ExceptionMiddleware(mockRequestDelegate.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ValidationException_ReturnsBadRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var mockRequestDelegate = new Mock<RequestDelegate>();
        mockRequestDelegate.Setup(rd => rd(It.IsAny<HttpContext>())).ThrowsAsync(new ValidationException("Validation error", []));
        var middleware = new ExceptionMiddleware(mockRequestDelegate.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);
    }

    [Fact]
    public async Task InvokeAsync_CodeDesignPlusException_ReturnsBadRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var mockRequestDelegate = new Mock<RequestDelegate>();
        mockRequestDelegate.Setup(rd => rd(It.IsAny<HttpContext>())).ThrowsAsync(new CodeDesignPlusException(Layer.Application, "CodeDesignPlus error", "1-001"));
        var middleware = new ExceptionMiddleware(mockRequestDelegate.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);
    }

    [Fact]
    public async Task InvokeAsync_Exception_ReturnsInternalServerError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var mockRequestDelegate = new Mock<RequestDelegate>();
        mockRequestDelegate.Setup(rd => rd(It.IsAny<HttpContext>())).ThrowsAsync(new Exception("General error"));
        var middleware = new ExceptionMiddleware(mockRequestDelegate.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);
    }
}

