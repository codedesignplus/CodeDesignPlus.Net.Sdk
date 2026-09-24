#nullable enable

using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
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
            var language = ErrorLanguage.Current;

            var errors = GetCatalog()
                .Select(entry => new ErrorDetail(entry.Error.Code, null!, entry.Error.GetMessage(language))
                {
                    Layer = entry.Layer,
                });

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
    /// <exception cref="InvalidOperationException">Si el catalogo o sus traducciones no son coherentes.</exception>
    public static IApplicationBuilder UseCodeErrorsValidation(this IApplicationBuilder app)
    {
        var catalog = GetCatalog().ToList();

        var problems = FindDuplicatedCodes(catalog)
            .Concat(FindLegacyConstants())
            .Concat(FindDeadTranslations(catalog))
            .Concat(FindPlaceholderMismatches(catalog))
            .ToList();

        if (problems.Count > 0)
            throw new InvalidOperationException($"The error catalog is not consistent: {string.Join(" | ", problems)}.");

        return app;
    }

    /// <summary>
    /// Un codigo repetido no identifica nada: ni la pantalla puede traducirlo ni quien lee un log sabe cual
    /// de los dos errores ocurrio.
    /// </summary>
    private static IEnumerable<string> FindDuplicatedCodes(IEnumerable<CatalogEntry> catalog)
    {
        return catalog
            .GroupBy(entry => entry.Error.Code, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => $"the code {group.Key} is declared {group.Count()} times");
    }

    /// <summary>
    /// Un catalogo que todavia declara <c>const string</c> es un microservicio a medio migrar: sus errores
    /// se quedarian en ingles para siempre y nadie se enteraria.
    /// </summary>
    private static IEnumerable<string> FindLegacyConstants()
    {
        return GetCatalogTypes()
            .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(field => field.FieldType == typeof(string))
                .Select(field => $"{type.Name}.{field.Name} is still a string instead of an Error"));
    }

    /// <summary>
    /// Una traduccion cuyo codigo ya no existe: alguien borro o renumero el error y dejo el texto detras.
    /// </summary>
    private static IEnumerable<string> FindDeadTranslations(IEnumerable<CatalogEntry> catalog)
    {
        var declared = catalog.Select(entry => entry.Error.Code).ToHashSet(StringComparer.Ordinal);

        foreach (var language in ErrorCatalog.Languages)
        {
            foreach (var code in ErrorCatalog.Codes(language).Where(code => !declared.Contains(code)))
                yield return $"the translation {language} has the code {code}, which no error declares";
        }
    }

    /// <summary>
    /// Si el ingles tiene <c>{0}</c> y la traduccion no, ese dato desaparece en ese idioma.
    /// </summary>
    private static IEnumerable<string> FindPlaceholderMismatches(IEnumerable<CatalogEntry> catalog)
    {
        foreach (var entry in catalog)
        {
            var expected = Placeholders(entry.Error.Fallback);

            foreach (var language in ErrorCatalog.Languages)
            {
                var translation = ErrorCatalog.Find(entry.Error.Code, language);

                if (translation is null)
                    continue;

                if (!expected.SetEquals(Placeholders(translation)))
                    yield return $"the code {entry.Error.Code} does not use the same placeholders in {language} as in English";
            }
        }
    }

    private static HashSet<string> Placeholders(string template)
    {
        return [.. PlaceholderPattern.Matches(template).Select(match => match.Groups[1].Value)];
    }

    private static readonly Regex PlaceholderPattern = new(@"\{(\d+)\}", RegexOptions.Compiled);

    /// <summary>
    /// Un error del catalogo junto a la capa que lo declara.
    /// </summary>
    private sealed record CatalogEntry(Error Error, string? Layer);

    /// <summary>
    /// Todos los errores declarados por los catalogos cargados en el proceso.
    /// </summary>
    /// <remarks>
    /// Solo cuentan los campos de tipo <see cref="Error"/>. Una constante que sobreviva no se ignora en
    /// silencio: <see cref="UseCodeErrorsValidation"/> la denuncia al arrancar.
    /// </remarks>
    private static IEnumerable<CatalogEntry> GetCatalog()
    {
        return GetCatalogTypes()
            .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(field => field.FieldType == typeof(Error))
                .Select(field => new CatalogEntry((Error)field.GetValue(null)!, GetLayer(type))));
    }

    private static IEnumerable<Type> GetCatalogTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .SelectMany(GetTypes)
            .Where(type => typeof(IErrorCodes).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract);
    }

    /// <summary>
    /// La capa sale del espacio de nombres del catalogo, que es donde vive el dato. Hasta ahora `/errors` no
    /// la publicaba y habia que deducirla por centenas, que es fragil.
    /// </summary>
    private static string? GetLayer(Type type)
    {
        var name = type.Namespace ?? string.Empty;

        if (name.EndsWith(".Domain", StringComparison.Ordinal) || name.Contains(".Domain.", StringComparison.Ordinal))
            return "Domain";

        if (name.EndsWith(".Application", StringComparison.Ordinal) || name.Contains(".Application.", StringComparison.Ordinal))
            return "Application";

        if (name.EndsWith(".Infrastructure", StringComparison.Ordinal) || name.Contains(".Infrastructure.", StringComparison.Ordinal))
            return "Infrastructure";

        return null;
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
