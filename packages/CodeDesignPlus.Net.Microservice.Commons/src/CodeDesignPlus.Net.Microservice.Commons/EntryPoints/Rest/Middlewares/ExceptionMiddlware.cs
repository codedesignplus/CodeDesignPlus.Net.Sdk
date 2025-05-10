using System.Diagnostics;
using System.Net;
using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Exceptions.Models;
using CodeDesignPlus.Net.Serializers;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Middlewares;

/// <summary>
/// Middleware for handling exceptions in HTTP requests.
/// </summary>
public class ExceptionMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Invokes the middleware to handle the HTTP context.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await HandleExceptionsAsync(context, ex);
        }
        catch (CodeDesignPlusException ex)
        {
            await HandleExceptionsAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionsAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles CodeDesignPlus exceptions.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The CodeDesignPlus exception.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static Task HandleExceptionsAsync(HttpContext context, CodeDesignPlusException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var logger = context.RequestServices.GetRequiredService<ILogger<ExceptionMiddleware>>();

        logger.LogError(exception, "TraceId: {TraceIdentifier} - {Message}", context.TraceIdentifier, exception.Message);
        
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

        var response = new ErrorResponse(traceId, exception.Layer);

        response.AddError(exception.Code, exception.Message, null);

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    /// <summary>
    /// Handles validation exceptions.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The validation exception.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static Task HandleExceptionsAsync(HttpContext context, ValidationException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var errors = exception.Errors.Select(e => new ErrorDetail(e.ErrorCode, e.PropertyName, e.ErrorMessage));

        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

        var response = new ErrorResponse(traceId, Layer.Application);

        response.Errors.AddRange(errors);

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    /// <summary>
    /// Handles general exceptions.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The general exception.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static Task HandleExceptionsAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var result = new ErrorResponse(context.TraceIdentifier, Layer.None);

        result.AddError("0-000", exception.Message, null);

        return context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }
}