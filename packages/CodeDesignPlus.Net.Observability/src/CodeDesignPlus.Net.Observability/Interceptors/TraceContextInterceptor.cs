using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace CodeDesignPlus.Net.Observability.Interceptors;

/// <summary>
/// gRPC interceptor that adds the current trace ID to response trailers (metadata).
/// </summary>
public class TraceContextInterceptor : Interceptor
{
    /// <summary>
    /// Intercepts a unary (single request/response) gRPC call.
    /// </summary>
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var response = await base.UnaryServerHandler(request, context, continuation);

        AddTraceContextToMetadata(context);

        return response;
    }

    /// <summary>
    /// Intercepts a server streaming gRPC call.
    /// </summary>
    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        await base.ServerStreamingServerHandler(request, responseStream, context, continuation);

        AddTraceContextToMetadata(context);
    }

    /// <summary>
    /// Intercepts a client streaming gRPC call.
    /// </summary>
    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        var response = await base.ClientStreamingServerHandler(requestStream, context, continuation);

        AddTraceContextToMetadata(context);

        return response;
    }

    /// <summary>
    /// Intercepts a bidirectional streaming gRPC call.
    /// </summary>
    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        await base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);

        AddTraceContextToMetadata(context);
    }

    /// <summary>
    /// Adds trace context metadata (X-Trace-Id and X-Span-Id) to the response trailers.
    /// </summary>
    /// <param name="context">The server call context.</param>
    private static void AddTraceContextToMetadata(ServerCallContext context)
    {
        var activity = Activity.Current;
        if (activity != null)
        {
            context.ResponseTrailers.Add("x-trace-id", activity.TraceId.ToString());
            context.ResponseTrailers.Add("x-span-id", activity.SpanId.ToString());
        }
    }
}
