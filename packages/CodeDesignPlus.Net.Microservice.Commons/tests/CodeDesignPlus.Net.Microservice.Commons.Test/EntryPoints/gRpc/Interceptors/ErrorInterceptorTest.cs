using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.gRpc.Interceptors;
using FluentValidation;
using FluentValidation.Results;
using Grpc.Core;
using Moq;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.gRpc.Interceptors;

public class ErrorInterceptorTest
{
    private readonly ErrorInterceptor _interceptor;

    public ErrorInterceptorTest()
    {
        _interceptor = new ErrorInterceptor(Mock.Of<ILogger<ErrorInterceptor>>());
    }

    [Fact]
    public async Task UnaryServerHandler_ShouldHandleValidationException()
    {
        // Arrange
        var request = new Mock<object>().Object;
        var context = new Mock<ServerCallContext>().Object;
        var continuation = new Mock<UnaryServerMethod<object, object>>();
        continuation.Setup(c => c(request, context)).ThrowsAsync(new ValidationException("Validation failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, continuation.Object));
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
    }

    [Fact]
    public async Task UnaryServerHandler_ShouldHandleCodeDesignPlusException()
    {
        // Arrange
        var request = new Mock<object>().Object;
        var context = new Mock<ServerCallContext>().Object;
        var continuation = new Mock<UnaryServerMethod<object, object>>();
        continuation.Setup(c => c(request, context)).ThrowsAsync(new CodeDesignPlusException(Layer.Application, "Error code", "Error message"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, continuation.Object));
        Assert.Equal(StatusCode.FailedPrecondition, exception.StatusCode);
    }

    [Fact]
    public async Task UnaryServerHandler_ShouldHandleGeneralException()
    {
        // Arrange
        var request = new Mock<object>().Object;
        var context = new Mock<ServerCallContext>().Object;
        var continuation = new Mock<UnaryServerMethod<object, object>>();
        continuation.Setup(c => c(request, context)).ThrowsAsync(new Exception("General error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, continuation.Object));
        Assert.Equal(StatusCode.Internal, exception.StatusCode);
    }

    [Fact]
    public async Task ClientStreamingServerHandler_ShouldHandleValidationException()
    {
        // Arrange
        var requestStream = new Mock<IAsyncStreamReader<object>>().Object;
        var context = new Mock<ServerCallContext>().Object;
        var continuation = new Mock<ClientStreamingServerMethod<object, object>>();
        continuation.Setup(c => c(requestStream, context)).ThrowsAsync(new ValidationException("Validation failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() => _interceptor.ClientStreamingServerHandler(requestStream, context, continuation.Object));
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
    }

    [Fact]
    public async Task ServerStreamingServerHandler_ShouldHandleValidationException()
    {
        // Arrange
        var request = new Mock<object>().Object;
        var responseStream = new Mock<IServerStreamWriter<object>>().Object;
        var context = new Mock<ServerCallContext>().Object;
        var continuation = new Mock<ServerStreamingServerMethod<object, object>>();
        continuation.Setup(c => c(request, responseStream, context)).ThrowsAsync(new ValidationException("Validation failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() => _interceptor.ServerStreamingServerHandler(request, responseStream, context, continuation.Object));
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
    }

    [Fact]
    public async Task DuplexStreamingServerHandler_ShouldHandleValidationException()
    {
        // Arrange
        var requestStream = new Mock<IAsyncStreamReader<object>>().Object;
        var responseStream = new Mock<IServerStreamWriter<object>>().Object;
        var context = new Mock<ServerCallContext>().Object;
        var continuation = new Mock<DuplexStreamingServerMethod<object, object>>();
        continuation.Setup(c => c(requestStream, responseStream, context)).ThrowsAsync(new ValidationException("Validation failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() => _interceptor.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation.Object));
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
    }

    [Fact]
    public async Task HandleValidationException_ShouldReturnMetadataErrors()
    {
        // Arrange
        var errors = new[]
        {
            new ValidationFailure("Property1", "Error 1"),
            new ValidationFailure("Property2", "Error 2")
        };
        var validationException = new ValidationException("General error", errors);
        var request = new Mock<object>().Object;
        var context = new Mock<ServerCallContext>().Object;
        var continuation = new Mock<UnaryServerMethod<object, object>>();
        continuation.Setup(c => c(request, context)).ThrowsAsync(validationException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() => _interceptor.UnaryServerHandler(request, context, continuation.Object));
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Contains(exception.Trailers, t => t.Key == "layer" && t.Value == Layer.Application.ToString());
        Assert.Contains(exception.Trailers, t => t.Key == "validationerror" && t.Value == "Code: , Property: Property1, Message: Error 1");
        Assert.Contains(exception.Trailers, t => t.Key == "validationerror" && t.Value == "Code: , Property: Property2, Message: Error 2");
    }
}

