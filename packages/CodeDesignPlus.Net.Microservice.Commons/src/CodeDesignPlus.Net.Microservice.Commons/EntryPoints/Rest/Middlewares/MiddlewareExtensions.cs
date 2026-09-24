using System.Globalization;
using System.Net;
using System.Reflection;
using CodeDesignPlus.Net.Exceptions;
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
    /// Add the language middleware to the application, so errors answer in the language the client asks for.
    /// </summary>
    /// <remarks>
    /// Va <b>antes</b> que <c>UseExceptionMiddleware</c>: el idioma tiene que estar fijado cuando la
    /// excepcion llega al borde y se traduce.
    /// </remarks>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseLanguageMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<LanguageMiddleware>();

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

            // El catalogo se publica en el idioma que pida el cliente, para que diga lo mismo que dicen
            // los errores del API. Sin `Accept-Language` sale el ingles, que es el obligatorio.
            var culture = CultureInfo.CurrentUICulture;

            var errors = GetCatalog()
                .Select(error => new ErrorDetail(error.Code, null!, error.GetMessage(culture)));

            return x.Response.WriteAsJsonAsync(errors);

        });

        return builder;
    }

    /// <summary>
    /// Comprueba al arrancar que ningun catalogo repite un codigo.
    /// </summary>
    /// <remarks>
    /// Un codigo repetido no identifica nada: ni la pantalla puede traducirlo, ni quien lee un log sabe cual
    /// de los dos errores ocurrio. Cuando se escribio esto habia <b>seis</b> catalogos asi en el monorepo, y
    /// uno repetia veinte codigos. Se falla al arrancar por la misma razon por la que falla la validacion de
    /// opciones: un microservicio que levanta mintiendo es peor que uno que no levanta.
    /// </remarks>
    /// <param name="app">The application builder.</param>
    /// <exception cref="InvalidOperationException">Si hay codigos repetidos.</exception>
    public static IApplicationBuilder UseCodeErrorsValidation(this IApplicationBuilder app)
    {
        var duplicated = GetCatalog()
            .GroupBy(error => error.Code, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => $"{group.Key} ({group.Count()} veces)")
            .ToList();

        if (duplicated.Count > 0)
            throw new InvalidOperationException(
                $"The error catalog declares duplicated codes and they no longer identify a single error: {string.Join(", ", duplicated)}.");

        return app;
    }

    /// <summary>
    /// Todos los errores declarados por los catalogos cargados en el proceso.
    /// </summary>
    /// <remarks>
    /// Acepta las dos formas a proposito: el <see cref="Error"/> nuevo y la constante <c>"201 : mensaje"</c>
    /// de los microservicios que todavia no se han migrado.
    /// </remarks>
    private static IEnumerable<Error> GetCatalog()
    {
        var catalogs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .SelectMany(GetTypes)
            .Where(type => typeof(IErrorCodes).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract);

        return catalogs
            .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            .Select(field => field.GetValue(null))
            .Select(value => value switch
            {
                Error error => error,
                string legacy when !string.IsNullOrWhiteSpace(legacy) => Error.FromString(legacy),
                _ => null,
            })
            .Where(error => error is not null)
            .Select(error => error!);
    }

    private static IEnumerable<Type> GetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            // Un ensamblado con tipos que no cargan no debe impedir publicar el resto del catalogo.
            return ex.Types.Where(type => type is not null).Select(type => type!);
        }
    }
}
