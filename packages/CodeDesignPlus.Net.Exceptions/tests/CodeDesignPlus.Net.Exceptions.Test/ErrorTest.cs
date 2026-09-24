using System.Globalization;
using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Exceptions.Test;

/// <summary>
/// El catalogo de errores con idiomas: el ingles vive en el codigo y es obligatorio, las traducciones
/// viven en los <c>errors.&lt;idioma&gt;.json</c> embebidos y lo que falte cae al ingles.
/// </summary>
/// <remarks>
/// Este proyecto de pruebas lleva embebidos un <c>errors.es.json</c> con tres codigos y un
/// <c>errors.fr-CA.json</c> con uno solo, a proposito: el frances incompleto es lo que demuestra que una
/// traduccion a medias no rompe nada.
/// </remarks>
public class ErrorTest
{
    private static readonly CultureInfo Spanish = CultureInfo.GetCultureInfo("es");
    private static readonly CultureInfo SpanishColombia = CultureInfo.GetCultureInfo("es-CO");
    private static readonly CultureInfo FrenchCanada = CultureInfo.GetCultureInfo("fr-CA");
    private static readonly CultureInfo French = CultureInfo.GetCultureInfo("fr");

    [Fact]
    public void Constructor_SplitsCodeAndMessage()
    {
        var error = new Error("201", "The user was not found.");

        Assert.Equal("201", error.Code);
        Assert.Equal("The user was not found.", error.Fallback);
        Assert.Empty(error.Arguments);
    }

    [Fact]
    public void GetMessage_WithoutTranslation_ReturnsEnglish()
    {
        var error = new Error("999", "There is no translation for this one.");

        Assert.Equal("There is no translation for this one.", error.GetMessage(Spanish));
    }

    [Fact]
    public void GetMessage_WithTranslation_ReturnsIt()
    {
        var error = new Error("201", "The user was not found.");

        Assert.Equal("No encontramos ese usuario.", error.GetMessage(Spanish));
    }

    [Fact]
    public void GetMessage_WithRegionalCulture_FallsBackToTheNeutralOne()
    {
        var error = new Error("201", "The user was not found.");

        // No hay `errors.es-CO.json`, pero si `errors.es.json`.
        Assert.Equal("No encontramos ese usuario.", error.GetMessage(SpanishColombia));
    }

    [Fact]
    public void GetMessage_WithRegionalCatalog_PrefersItOverTheNeutralOne()
    {
        var error = new Error("201", "The user was not found.");

        Assert.Equal("Cet utilisateur est introuvable.", error.GetMessage(FrenchCanada));
    }

    /// <summary>
    /// La mitad de la ficha 114: un idioma incompleto no deja al usuario sin mensaje, le da el ingles.
    /// </summary>
    [Fact]
    public void GetMessage_WithPartialCatalog_FallsBackToEnglishForTheMissingOnes()
    {
        var translated = new Error("201", "The user was not found.");
        var untranslated = new Error("250", "Cannot decide eligibility: nothing was replicated.");

        Assert.Equal("Cet utilisateur est introuvable.", translated.GetMessage(FrenchCanada));
        Assert.Equal("Cannot decide eligibility: nothing was replicated.", untranslated.GetMessage(FrenchCanada));
    }

    [Fact]
    public void GetMessage_WithUnknownLanguage_ReturnsEnglish()
    {
        var error = new Error("201", "The user was not found.");

        Assert.Equal("The user was not found.", error.GetMessage(CultureInfo.GetCultureInfo("de")));
    }

    [Fact]
    public void GetMessage_WithoutCulture_ReturnsEnglish()
    {
        var error = new Error("201", "The user was not found.");

        Assert.Equal("The user was not found.", error.GetMessage(null));
    }

    [Fact]
    public void With_FormatsAgainstTheTranslatedTemplate()
    {
        var error = new Error("301", "The product {0} is not active.").With("A-102");

        Assert.Equal("El producto A-102 no esta activo.", error.GetMessage(Spanish));
        Assert.Equal("The product A-102 is not active.", error.GetMessage(French));
    }

    [Fact]
    public void With_DoesNotTouchTheDeclaredError()
    {
        var declared = new Error("301", "The product {0} is not active.");

        declared.With("A-102");

        Assert.Empty(declared.Arguments);
    }

    /// <summary>
    /// El <c>GetMessage()</c> de la extension partia por el <b>ultimo</b> <c>:</c> y truncaba el mensaje.
    /// Hay uno asi en produccion —el de elegibilidad de parqueaderos—, que llegaba al usuario empezando por
    /// la mitad.
    /// </summary>
    [Fact]
    public void FromString_SplitsOnTheFirstColonSoAMessageWithColonsSurvives()
    {
        var error = Error.FromString("250 : Cannot decide eligibility: no financial document has been replicated.");

        Assert.Equal("250", error.Code);
        Assert.Equal("Cannot decide eligibility: no financial document has been replicated.", error.Fallback);
    }

    [Fact]
    public void ImplicitOperator_KeepsAnUnmigratedMicroserviceWorking()
    {
        Error error = "201 : The user was not found.";

        Assert.Equal("201", error.GetCode());
        Assert.Equal("No encontramos ese usuario.", error.GetMessage(Spanish));
    }

    [Fact]
    public void ToString_KeepsTheOldShapeSoFormatTestsStillPass()
    {
        var error = new Error("201", "The user was not found.");

        Assert.Matches(@"^\d{3} : .+$", error.ToString());
    }

    [Fact]
    public void Catalog_ReportsTheLanguagesItHas()
    {
        Assert.Contains("es", ErrorCatalog.Languages);
        Assert.Contains("fr-CA", ErrorCatalog.Languages);
        Assert.Equal(3, ErrorCatalog.Count("es"));
        Assert.Equal(1, ErrorCatalog.Count("fr-CA"));
        Assert.Equal(0, ErrorCatalog.Count("de"));
    }
}
