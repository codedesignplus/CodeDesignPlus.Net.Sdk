#nullable enable

using System.Globalization;
using CodeDesignPlus.Net.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Middlewares;

/// <summary>
/// Fija el idioma de la peticion a partir de la cabecera <c>Accept-Language</c>.
/// </summary>
/// <remarks>
/// No se usa <c>UseRequestLocalization</c> del framework a proposito: obliga a declarar por adelantado la
/// lista de idiomas soportados, y entonces anadir un idioma dejaria de ser anadir un fichero
/// <c>errors.&lt;idioma&gt;.json</c>, que es justamente lo que se buscaba. Aqui se admite cualquier idioma
/// del que exista traduccion, y el resto cae al ingles.
/// <para>
/// Solo toca <see cref="CultureInfo.CurrentUICulture"/> —el idioma de los textos— y <b>no</b>
/// <see cref="CultureInfo.CurrentCulture"/>, que es el que decide como se formatean numeros y fechas.
/// Cambiar ese seria un desastre silencioso: un importe se serializaria con coma decimal segun el navegador
/// de quien llame.
/// </para>
/// </remarks>
/// <param name="next">The next middleware in the pipeline.</param>
public class LanguageMiddleware(RequestDelegate next)
{
    private const string Header = "Accept-Language";

    /// <summary>
    /// Invokes the middleware to handle the HTTP context.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task InvokeAsync(HttpContext context)
    {
        var language = Resolve(context.Request.Headers[Header]);

        if (language is not null)
            CultureInfo.CurrentUICulture = language;

        return next(context);
    }

    /// <summary>
    /// Elige el primer idioma pedido del que haya traduccion, respetando las preferencias del cliente.
    /// </summary>
    /// <param name="header">El valor crudo de <c>Accept-Language</c>, por ejemplo <c>fr-CA, es;q=0.8</c>.</param>
    /// <returns>El idioma a usar, o <c>null</c> para dejar el ingles.</returns>
    internal static CultureInfo? Resolve(string? header)
    {
        if (string.IsNullOrWhiteSpace(header))
            return null;

        var available = ErrorCatalog.Languages;

        if (available.Count == 0)
            return null;

        var preferences = header
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(Parse)
            .Where(preference => preference.Language is not null)
            .OrderByDescending(preference => preference.Quality)
            .ToList();

        foreach (var (candidate, _) in preferences)
        {
            var match = available.FirstOrDefault(language => language.Equals(candidate, StringComparison.OrdinalIgnoreCase))
                ?? available.FirstOrDefault(language => language.Equals(Neutral(candidate!), StringComparison.OrdinalIgnoreCase));

            if (match is not null)
                return Build(candidate!);
        }

        return null;
    }

    private static (string? Language, double Quality) Parse(string entry)
    {
        var parts = entry.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var language = parts.Length > 0 ? parts[0] : null;

        if (string.IsNullOrWhiteSpace(language) || language == "*")
            return (null, 0);

        var quality = 1d;

        foreach (var part in parts.Skip(1))
        {
            if (part.StartsWith("q=", StringComparison.OrdinalIgnoreCase)
                && double.TryParse(part[2..], NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
                quality = parsed;
        }

        return (language, quality);
    }

    private static string Neutral(string language)
    {
        var dash = language.IndexOf('-');

        return dash < 0 ? language : language[..dash];
    }

    private static CultureInfo? Build(string language)
    {
        try
        {
            return CultureInfo.GetCultureInfo(language);
        }
        catch (CultureNotFoundException)
        {
            // Un cliente puede mandar cualquier cosa en la cabecera; que no sea una cultura valida no es
            // motivo para fallar la peticion, se sigue en ingles.
            return null;
        }
    }
}
