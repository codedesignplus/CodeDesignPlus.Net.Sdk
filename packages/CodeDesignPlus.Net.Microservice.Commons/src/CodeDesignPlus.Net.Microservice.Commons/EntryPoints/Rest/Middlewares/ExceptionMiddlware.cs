using System.Diagnostics;
using System.Net;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Exceptions.Models;
using CodeDesignPlus.Net.Serializers;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Middlewares;

/// <summary>
/// Middleware for handling exceptions in HTTP requests.
/// </summary>
/// <remarks>
/// This middleware catches exceptions thrown during the processing of HTTP requests and formats them into a standardized problem details response based on RFC 7807 and RFC 9457
/// </remarks>
/// <param name="next">The next middleware in the pipeline.</param>
/// <param name="logger">Logger for logging exceptions.</param>
/// <param name="env">The hosting environment to determine if the application is in production.</param>
/// <param name="options">Options for configuring the middleware.</param>
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env, IOptions<CoreOptions> options)
{
    private readonly Newtonsoft.Json.JsonSerializerSettings serializerSettings = new()
    {
        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
        Formatting = Newtonsoft.Json.Formatting.None
    };

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
            logger.LogWarning(ex, "Validation error occurred: {ValidationErrors}", string.Join(", ", ex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")));

            await HandleExceptionsAsync(context, ex);
        }
        catch (CodeDesignPlusException ex)
        {
            logger.LogWarning(ex, "CodeDesignPlusException occurred. Layer: {Layer}, Code: {Code}, Message: {CustomMessage}", ex.Layer, ex.Code, ex.Message);

            await HandleExceptionsAsync(context, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception has occurred.");

            await HandleExceptionsAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles validation exceptions.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The validation exception.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task HandleExceptionsAsync(HttpContext context, ValidationException exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var problemDetails = new ProblemDetails
        {
            Type = $"{options.Value.ApiDocumentationBaseUrl}validation-error",
            Title = "Error de validación.",
            Status = context.Response.StatusCode,
            Detail = "Uno o más campos no pasaron la validación.",
            Instance = traceId
        };

        var invalidParams = exception.Errors
            .Select(e => new InvalidParamDetail(
                ToCamelCase(e.PropertyName),
                e.ErrorMessage,
                e.ErrorCode
            )).ToList();

        problemDetails.Extensions["invalid_params"] = invalidParams;

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, serializerSettings));
    }


    /// <summary>
    /// Handles CodeDesignPlus exceptions.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The CodeDesignPlus exception.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task HandleExceptionsAsync(HttpContext context, CodeDesignPlusException exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var messageTemplate = exception.Layer switch
        {
            Layer.Domain => "Domain Error",
            Layer.Infrastructure => "Infrastructure Error",
            Layer.Application => "Application Error",
            _ => "Internal Error Sdk CodeDesignPlus"
        };

        var detailMessage = exception.Layer switch
        {
            Layer.Domain => $"An error occurred in the domain layer - {exception.Code} ({exception.Message})",
            Layer.Infrastructure => $"An error occurred in the infrastructure layer - {exception.Code} ({exception.Message})",
            Layer.Application => $"An error occurred in the application layer - {exception.Code} ({exception.Message})",
            _ => $"An internal error occurred in the SDK CodeDesignPlus - {exception.Code} ({exception.Message})"
        };

        var layerName = exception.Layer.ToString().ToLowerInvariant();

        var problemDetails = new ProblemDetails
        {
            Type = $"{options.Value.ApiDocumentationBaseUrl}{options.Value.AppName}/{layerName}-{exception.Code}",
            Title = messageTemplate,
            Status = context.Response.StatusCode,
            Detail = detailMessage,
            Instance = traceId
        };

        problemDetails.Extensions["layer"] = exception.Layer.ToString();
        problemDetails.Extensions["error_code"] = exception.Code;

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, serializerSettings));
    }

    /// <summary>
    /// Handles general exceptions.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The general exception.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task HandleExceptionsAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var problemDetails = new ProblemDetails
        {
            Type = $"{options.Value.ApiDocumentationBaseUrl}internal-error",
            Title = "Internal Server Error",
            Status = context.Response.StatusCode,
            Detail = "An error occurred while processing your request, please try again later or contact support.",
            Instance = traceId
        };

        if (!env.IsProduction())
            problemDetails.Extensions["exception_message"] = exception.Message;

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, serializerSettings));
    }

    /// <summary>
    /// Converts a string to camel case.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <returns>A camel-cased string.</returns>
    private static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str) || !char.IsUpper(str[0]))
            return str;

        var chars = str.ToCharArray();
        chars[0] = char.ToLowerInvariant(chars[0]);
        return new string(chars);
    }
}