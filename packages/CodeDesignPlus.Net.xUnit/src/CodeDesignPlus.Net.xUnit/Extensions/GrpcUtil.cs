
using Grpc.Core;

namespace CodeDesignPlus.Net.xUnit.Extensions;

/// <summary>
/// Utility class to create gRpc responses. 
/// </summary>
public static class GrpcUtil
{
    /// <summary>
    /// Create a <see cref="AsyncUnaryCall{TResponse}"/> with the specified response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="response">The response to return.</param>
    /// <returns>Returns a <see cref="AsyncUnaryCall{TResponse}"/> with the specified response.</returns>
    public static AsyncUnaryCall<TResponse> CreateAsyncUnaryCall<TResponse>(TResponse response)
    {
        return new AsyncUnaryCall<TResponse>(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => [],
            () => { });
    }

    /// <summary>
    /// Create a <see cref="AsyncUnaryCall{TResponse}"/> with the specified <see cref="StatusCode"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="statusCode">The status code to return.</param>
    /// <returns>Returns a <see cref="AsyncUnaryCall{TResponse}"/> with the specified <see cref="StatusCode"/>.</returns>
    public static AsyncUnaryCall<TResponse> CreateAsyncUnaryCall<TResponse>(StatusCode statusCode)
    {
        var status = new Status(statusCode, string.Empty);
        return new AsyncUnaryCall<TResponse>(
            Task.FromException<TResponse>(new RpcException(status)),
            Task.FromResult(new Metadata()),
            () => status,
            () => [],
            () => { });
    }
}