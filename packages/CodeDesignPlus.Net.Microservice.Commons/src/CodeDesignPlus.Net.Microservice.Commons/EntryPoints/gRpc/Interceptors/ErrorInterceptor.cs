using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Exceptions.Models;
using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.gRpc.Interceptors;

/// <summary>
/// Interceptor for handling errors in gRPC calls.
/// </summary>
/// <param name="logger">Logger for logging errors.</param>
public class ErrorInterceptor(ILogger<ErrorInterceptor> logger) : Interceptor
{
    /// <summary>
    /// Handles unary server calls.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request.</typeparam>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    /// <param name="request">Client request.</param>
    /// <param name="context">Server call context.</param>
    /// <param name="continuation">Continuation method for the call.</param>
    /// <returns>Server response.</returns>
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during unary server call");
            throw HandleException(ex);
        }
    }

    /// <summary>
    /// Handles client streaming server calls.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request.</typeparam>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    /// <param name="requestStream">Client request stream.</param>
    /// <param name="context">Server call context.</param>
    /// <param name="continuation">Continuation method for the call.</param>
    /// <returns>Server response.</returns>
    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(requestStream, context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during client streaming server call");
            throw HandleException(ex);
        }
    }

    /// <summary>
    /// Handles server streaming server calls.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request.</typeparam>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    /// <param name="request">Client request.</param>
    /// <param name="responseStream">Server response stream.</param>
    /// <param name="context">Server call context.</param>
    /// <param name="continuation">Continuation method for the call.</param>
    /// <returns>Completed task.</returns>
    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(request, responseStream, context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during server streaming server call");
            throw HandleException(ex);
        }
    }

    /// <summary>
    /// Handles duplex streaming server calls.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request.</typeparam>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    /// <param name="requestStream">Client request stream.</param>
    /// <param name="responseStream">Server response stream.</param>
    /// <param name="context">Server call context.</param>
    /// <param name="continuation">Continuation method for the call.</param>
    /// <returns>Completed task.</returns>
    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(requestStream, responseStream, context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during duplex streaming call");
            throw HandleException(ex);
        }
    }

    /// <summary>
    /// Handles exceptions and converts them to RpcException.
    /// </summary>
    /// <param name="exception">Exception to handle.</param>
    /// <returns>Corresponding RpcException.</returns>
    private static RpcException HandleException(Exception exception)
    {
        return exception switch
        {
            ValidationException ex => HandleValidationException(ex),
            CodeDesignPlusException ex => HandleCodeDesignPlusException(ex),
            _ => HandleGeneralException(exception),
        };
    }

    /// <summary>
    /// Handles validation exceptions.
    /// </summary>
    /// <param name="exception">Validation exception.</param>
    /// <returns>Corresponding RpcException.</returns>
    private static RpcException HandleValidationException(ValidationException exception)
    {
        var errors = exception.Errors.Select(e => new ErrorDetail
        (
            e.ErrorCode,
            e.PropertyName,
            e.ErrorMessage
        )).ToList();

        var response = new ErrorResponse(null, Layer.Application);

        response.Errors.AddRange(errors);

        var metadata = new Metadata
        {
            { "Layer", Layer.Application.ToString() }
        };

        foreach (var error in errors)
        {
            metadata.Add("ValidationError", $"Code: {error.Code}, Property: {error.Field}, Message: {error.Message}");
        }

        var status = new Status(StatusCode.InvalidArgument, "Validation failed");

        return new RpcException(status, metadata, response.ToString());
    }

    /// <summary>
    /// Handles CodeDesignPlus exceptions.
    /// </summary>
    /// <param name="exception">CodeDesignPlus exception.</param>
    /// <returns>Corresponding RpcException.</returns>
    private static RpcException HandleCodeDesignPlusException(CodeDesignPlusException exception)
    {
        var response = new ErrorResponse(null, exception.Layer);

        response.Errors.Add(new ErrorDetail(exception.Code, null, exception.Message));

        var metadata = new Metadata
        {
            { "Layer", exception.Layer.ToString() },
            { "Code", exception.Code },
            { "Message", exception.Message }
        };

        var status = new Status(StatusCode.FailedPrecondition, exception.Message);

        return new RpcException(status, metadata, response.ToString());
    }

    /// <summary>
    /// Handles general exceptions.
    /// </summary>
    /// <param name="exception">General exception.</param>
    /// <returns>Corresponding RpcException.</returns>
    private static RpcException HandleGeneralException(Exception exception)
    {
        var response = new ErrorResponse(null, Layer.None);

        response.Errors.Add(new ErrorDetail("0-000", null, exception.Message));

        var metadata = new Metadata
        {
            { "Layer", Layer.None.ToString() },
            { "Code", "0-000" },
            { "Message", exception.Message }
        };

        var status = new Status(StatusCode.Internal, "Internal server error");

        return new RpcException(status, metadata, response.ToString());
    }
}