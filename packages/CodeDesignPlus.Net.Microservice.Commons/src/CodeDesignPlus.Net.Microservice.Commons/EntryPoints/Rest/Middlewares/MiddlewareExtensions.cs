using System.Net;
using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Exceptions.Extensions;
using CodeDesignPlus.Net.Exceptions.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Middlewares;

/// <summary>
/// Class extension for the middleware.
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Add the exception middleware to the application.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        return app;
    }

    /// <summary>
    /// Add the code errors middleware to the application.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <returns>The endpoint route builder.</returns>
    public static IEndpointRouteBuilder UseCodeErrors(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/errors", x =>
        {
            x.Response.ContentType = "application/json";
            x.Response.StatusCode = (int)HttpStatusCode.OK;

            var items = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IErrorCodes).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            var codes = items.SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                .Where(f => f.FieldType == typeof(string))
                .Select(f => f.GetValue(null).ToString());

            var errors = codes.Select(x => new ErrorDetail(x.GetCode(), null!, x.GetMessage()));

            return x.Response.WriteAsJsonAsync(errors);

        });

        return builder;
    }
}
