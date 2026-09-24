using System.Globalization;
using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Middlewares;
using Microsoft.AspNetCore.Http;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.Rest;

/// <summary>
/// El idioma de la respuesta sale de la cabecera <c>Accept-Language</c> y solo se acepta si hay traduccion.
/// </summary>
/// <remarks>
/// Este proyecto de pruebas lleva embebidos un <c>errors.en.json</c> y un <c>errors.es.json</c>, asi que
/// aqui los idiomas disponibles son el ingles y el espanol. Todo lo demas tiene que quedarse en ingles.
/// </remarks>
public class LanguageMiddlewareTest
{
    [Theory]
    [InlineData("es", "es")]
    [InlineData("es-CO", "es-CO")]
    [InlineData("es-CO,en;q=0.5", "es-CO")]
    [InlineData("en;q=0.5,es;q=0.9", "es")]
    public async Task InvokeAsync_WithATranslatedLanguage_SetsIt(string header, string expected)
    {
        Assert.Equal(expected, await RunAsync(header));
    }

    /// <summary>
    /// El ingles es un idioma que se puede <b>pedir</b>, con su fichero, y no «lo que queda al agotar la
    /// lista».
    /// </summary>
    /// <remarks>
    /// Mientras vivio en el C# no figuraba entre los disponibles, asi que nunca casaba con la cabecera y el
    /// turno pasaba al siguiente idioma: el ultimo caso de abajo —un navegador configurado en ingles que
    /// tambien lleva espanol, que es lo mas comun en Colombia— acababa respondiendo <b>en espanol</b>, y no
    /// habia forma de pedir ingles. Esto es lo que arregla que tenga su <c>errors.en.json</c>.
    /// </remarks>
    [Theory]
    [InlineData("en", "en")]
    [InlineData("en-US", "en-US")]
    [InlineData("en-US,en;q=0.9,es;q=0.8", "en-US")]
    public async Task InvokeAsync_WhenEnglishIsAsked_HonoursTheWeights(string header, string expected)
    {
        Assert.Equal(expected, await RunAsync(header));
    }

    [Theory]
    [InlineData("")]
    [InlineData("de-DE, ja;q=0.8")]
    [InlineData("*")]
    [InlineData("esto-no-es-un-idioma")]
    public async Task InvokeAsync_WithoutATranslatedLanguage_AnswersInEnglish(string header)
    {
        Assert.Null(await RunAsync(header));
    }

    /// <summary>
    /// El idioma manda sobre los textos y <b>no</b> sobre los numeros: los entrypoints corren con
    /// globalizacion invariante, y ademas tocar la cultura de formato haria que un importe se serializara
    /// con coma decimal segun el navegador de quien llame.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_DoesNotTouchTheFormattingCulture()
    {
        var original = CultureInfo.CurrentCulture;

        CultureInfo? formatting = null;

        var middleware = new LanguageMiddleware(_ =>
        {
            formatting = CultureInfo.CurrentCulture;
            return Task.CompletedTask;
        });

        var context = new DefaultHttpContext();
        context.Request.Headers.AcceptLanguage = "es-CO";

        await middleware.InvokeAsync(context);

        Assert.Equal(original.Name, formatting!.Name);
    }

    /// <summary>
    /// El pendiente 114 de punta a punta: un error del catalogo sale traducido cuando el cliente lo pide.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_TranslatesTheCatalogError()
    {
        string? message = null;

        var middleware = new LanguageMiddleware(_ =>
        {
            message = new Error("239").GetMessage();
            return Task.CompletedTask;
        });

        var context = new DefaultHttpContext();
        context.Request.Headers.AcceptLanguage = "es";

        await middleware.InvokeAsync(context);

        Assert.Equal("En ese periodo no se retuvo nada.", message);
    }

    private static async Task<string?> RunAsync(string header)
    {
        string? seen = null;

        var middleware = new LanguageMiddleware(_ =>
        {
            seen = ErrorLanguage.Current;
            return Task.CompletedTask;
        });

        var context = new DefaultHttpContext();

        if (!string.IsNullOrEmpty(header))
            context.Request.Headers.AcceptLanguage = header;

        await middleware.InvokeAsync(context);

        return seen;
    }
}
