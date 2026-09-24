using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Exceptions.Test;

/// <summary>
/// El catalogo de errores con idiomas: el C# solo declara el codigo y todos los mensajes —el ingles
/// tambien— viven en los <c>errors.&lt;idioma&gt;.json</c> embebidos.
/// </summary>
/// <remarks>
/// Este proyecto de pruebas lleva embebidos un <c>errors.en.json</c> con cuatro codigos, un
/// <c>errors.es.json</c> con tres y un <c>errors.fr-CA.json</c> con uno solo, a proposito: los idiomas
/// incompletos son los que demuestran que una traduccion a medias no deja a nadie sin mensaje.
/// </remarks>
public class ErrorTest
{
    private const string English = "en";
    private const string EnglishUnitedStates = "en-US";
    private const string Spanish = "es";
    private const string SpanishColombia = "es-CO";
    private const string FrenchCanada = "fr-CA";
    private const string French = "fr";

    [Fact]
    public void Constructor_KeepsOnlyTheCode()
    {
        var error = new Error("201");

        Assert.Equal("201", error.Code);
        Assert.Empty(error.Arguments);
    }

    [Fact]
    public void Constructor_WithoutCode_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Error(" "));
    }

    /// <summary>
    /// Un <c>default(Error)</c> no tiene codigo: no debe reventar por dentro, porque un struct se puede
    /// construir sin pasar por el constructor.
    /// </summary>
    [Fact]
    public void Default_DoesNotThrow()
    {
        var error = default(Error);

        Assert.Equal(string.Empty, error.Code);
        Assert.Empty(error.Arguments);
        Assert.Equal(string.Empty, error.GetMessage(Spanish));
    }

    [Fact]
    public void GetMessage_WithoutTranslation_ReturnsEnglish()
    {
        var error = new Error("999");

        Assert.Equal("There is no translation for this one.", error.GetMessage(Spanish));
    }

    [Fact]
    public void GetMessage_WithTranslation_ReturnsIt()
    {
        var error = new Error("201");

        Assert.Equal("No encontramos ese usuario.", error.GetMessage(Spanish));
    }

    [Fact]
    public void GetMessage_WithRegionalCulture_FallsBackToTheNeutralOne()
    {
        var error = new Error("201");

        // No hay `errors.es-CO.json`, pero si `errors.es.json`.
        Assert.Equal("No encontramos ese usuario.", error.GetMessage(SpanishColombia));
    }

    [Fact]
    public void GetMessage_WithRegionalCatalog_PrefersItOverTheNeutralOne()
    {
        var error = new Error("201");

        Assert.Equal("Cet utilisateur est introuvable.", error.GetMessage(FrenchCanada));
    }

    /// <summary>
    /// La mitad de la ficha 114: un idioma incompleto no deja al usuario sin mensaje, le da el ingles.
    /// </summary>
    [Fact]
    public void GetMessage_WithPartialCatalog_FallsBackToEnglishForTheMissingOnes()
    {
        var translated = new Error("201");
        var untranslated = new Error("250");

        Assert.Equal("Cet utilisateur est introuvable.", translated.GetMessage(FrenchCanada));
        Assert.Equal(
            "Cannot decide eligibility: no financial document has been replicated.",
            untranslated.GetMessage(FrenchCanada));
    }

    [Fact]
    public void GetMessage_WithUnknownLanguage_ReturnsEnglish()
    {
        var error = new Error("201");

        Assert.Equal("The user was not found.", error.GetMessage("de"));
    }

    [Fact]
    public void GetMessage_WithoutCulture_ReturnsEnglish()
    {
        var error = new Error("201");

        Assert.Equal("The user was not found.", error.GetMessage(null));
    }

    /// <summary>
    /// El ingles dejo de ser «lo que queda al no encontrar nada» y es un idioma que se puede pedir, con
    /// su fichero. Pedirlo tiene que devolverlo, y su variante regional tambien.
    /// </summary>
    [Fact]
    public void GetMessage_WhenEnglishIsAsked_ReturnsEnglish()
    {
        var error = new Error("201");

        Assert.Equal("The user was not found.", error.GetMessage(English));
        Assert.Equal("The user was not found.", error.GetMessage(EnglishUnitedStates));
    }

    /// <summary>
    /// Si el codigo no esta ni en ingles, el mensaje es el codigo pelado y no una cadena vacia: al menos
    /// dice cual es el error. Que eso no pase lo comprueba el arranque.
    /// </summary>
    [Fact]
    public void GetMessage_WithoutEvenEnglish_ReturnsTheCode()
    {
        var error = new Error("404");

        Assert.Equal("404", error.GetMessage(Spanish));
    }

    [Fact]
    public void With_FormatsAgainstTheTranslatedTemplate()
    {
        var error = new Error("301").With("A-102");

        Assert.Equal("El producto A-102 no esta activo.", error.GetMessage(Spanish));
        Assert.Equal("The product A-102 is not active.", error.GetMessage(French));
    }

    [Fact]
    public void With_DoesNotTouchTheDeclaredError()
    {
        var declared = new Error("301");

        declared.With("A-102");

        Assert.Empty(declared.Arguments);
    }

    /// <summary>
    /// Lo que va al log: ingles y con los argumentos puestos. Sin esto el log guardaria la plantilla cruda.
    /// </summary>
    [Fact]
    public void ToEnglish_AppliesTheArguments()
    {
        var error = new Error("301").With("A-102");

        Assert.Equal("The product A-102 is not active.", error.ToEnglish());
    }

    /// <summary>
    /// El <c>GetMessage()</c> de la extension partia por el <b>ultimo</b> <c>:</c> y truncaba el mensaje.
    /// Hay uno asi en produccion —el de elegibilidad de parqueaderos—, que llegaba al usuario empezando por
    /// la mitad.
    /// </summary>
    [Fact]
    public void FromString_SplitsOnTheFirstColonSoTheCodeIsRight()
    {
        var error = Error.FromString("250 : Cannot decide eligibility: no financial document has been replicated.");

        Assert.Equal("250", error.Code);
    }

    /// <summary>
    /// <c>FromString</c> sigue existiendo para leer catalogos ajenos en el formato viejo, pero <b>no hay
    /// conversion implicita</b>: una cadena suelta en un guard tiene que dejar de compilar, que es lo que
    /// obliga a que todo error salga del catalogo.
    /// </summary>
    [Fact]
    public void FromString_ReadsTheOldShape()
    {
        var error = Error.FromString("201 : The user was not found.");

        Assert.Equal("201", error.GetCode());
        Assert.Equal("No encontramos ese usuario.", error.GetMessage(Spanish));
    }

    [Fact]
    public void ToString_KeepsTheOldShapeSoFormatTestsStillPass()
    {
        var error = new Error("201");

        Assert.Matches(@"^\d{3} : .+$", error.ToString());
        Assert.Equal("201 : The user was not found.", error.ToString());
    }

    /// <summary>
    /// Es un tipo por valor: dos errores con el mismo codigo son el mismo error.
    /// </summary>
    [Fact]
    public void Equality_ComparesByValue()
    {
        Assert.Equal(new Error("201"), new Error("201"));
        Assert.NotEqual(new Error("201"), new Error("202"));
    }

    [Fact]
    public void Catalog_ReportsTheLanguagesItHas()
    {
        Assert.Contains("en", ErrorCatalog.Languages);
        Assert.Contains("es", ErrorCatalog.Languages);
        Assert.Contains("fr-CA", ErrorCatalog.Languages);
        // Los cuatro de estas pruebas mas los 20 de las pruebas de guardas, que tambien salen del catalogo.
        Assert.Equal(24, ErrorCatalog.Count("en"));
        Assert.Equal(3, ErrorCatalog.Count("es"));
        Assert.Equal(1, ErrorCatalog.Count("fr-CA"));
        Assert.Equal(0, ErrorCatalog.Count("de"));
    }
}
