using CodeDesignPlus.Net.Exceptions;
using FluentValidation;
using FluentValidation.Results;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.Rest;

/// <summary>
/// Los errores de campo salen en el idioma de la peticion y sin perder el dato que traian.
/// </summary>
/// <remarks>
/// FluentValidation trae sus mensajes traducidos y los elige mirando <c>CurrentUICulture</c>, pero los
/// entrypoints se compilan con globalizacion invariante y ahi esa cultura no se puede cambiar: se quedaba
/// en ingles pidiera lo que pidiera el cliente. De ahi la tabla de la centena 4xx.
/// </remarks>
public class ValidationTranslationTest
{
    private class Muestra
    {
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    private class MuestraValidator : AbstractValidator<Muestra>
    {
        public MuestraValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty();
            RuleFor(x => x.Cantidad).GreaterThan(10);
        }
    }

    private class LargoValidator : AbstractValidator<Muestra>
    {
        public LargoValidator() => RuleFor(x => x.Nombre).MaximumLength(3);
    }

    [Theory]
    [InlineData("es", "Nombre es obligatorio.")]
    [InlineData("pt", "Nombre é obrigatório.")]
    [InlineData("fr", "Nombre est obligatoire.")]
    [InlineData("en", "Nombre is required.")]
    public void Required_SaleEnElIdiomaPedido(string idioma, string esperado)
    {
        var fallo = Validar(new Muestra { Cantidad = 99 }, new MuestraValidator(), "Nombre");

        var error = ValidationErrors.FromFluentValidation(fallo.ErrorCode);

        Assert.NotNull(error);
        Assert.Equal("9300", error!.Value.Code);
        Assert.Equal(esperado, Rellenar(error.Value.GetMessage(idioma), fallo));
    }

    /// <summary>
    /// Lo que se perderia con una traduccion generica: cuantos caracteres caben y cuantos se escribieron.
    /// </summary>
    [Fact]
    public void TooLong_ConservaElMaximoYLoEscrito()
    {
        var fallo = Validar(new Muestra { Nombre = new string('a', 50) }, new LargoValidator(), "Nombre");

        var error = ValidationErrors.FromFluentValidation(fallo.ErrorCode);

        Assert.Equal("9301", error!.Value.Code);
        Assert.Equal(
            "Nombre no puede superar los 3 caracteres; escribiste 50.",
            Rellenar(error.Value.GetMessage("es"), fallo));
    }

    [Fact]
    public void GreaterThan_ConservaElValorComparado()
    {
        var fallo = Validar(new Muestra { Nombre = "x" }, new MuestraValidator(), "Cantidad");

        var error = ValidationErrors.FromFluentValidation(fallo.ErrorCode);

        Assert.Equal("9303", error!.Value.Code);
        Assert.Equal("Cantidad debe ser mayor que 10.", Rellenar(error.Value.GetMessage("es"), fallo));
    }

    /// <summary>
    /// Un validador que no este en la tabla se queda con su mensaje original: mejor sin traducir que
    /// diciendo otra cosa.
    /// </summary>
    [Fact]
    public void ValidadorDesconocido_NoSeTraduce()
    {
        Assert.Null(ValidationErrors.FromFluentValidation("UnValidadorNuestro"));
        Assert.Null(ValidationErrors.FromFluentValidation(null));
    }

    /// <summary>
    /// El sobre tambien: antes el titulo y el resumen estaban escritos en espanol dentro del SDK.
    /// </summary>
    [Theory]
    [InlineData("es", "Error de validación.")]
    [InlineData("en", "Validation error.")]
    [InlineData("fr", "Erreur de validation.")]
    public void ElSobre_TambienSeTraduce(string idioma, string esperado)
    {
        Assert.Equal(esperado, ValidationErrors.ValidationTitle.GetMessage(idioma));
    }

    private static ValidationFailure Validar<TValidator>(
        Muestra muestra, TValidator validador, string propiedad)
        where TValidator : AbstractValidator<Muestra>
    {
        var resultado = validador.Validate(muestra);

        return resultado.Errors.First(e => e.PropertyName == propiedad);
    }

    /// <summary>Lo mismo que hace el middleware, para comprobar la plantilla ya rellena.</summary>
    private static string Rellenar(string plantilla, ValidationFailure fallo)
    {
        var valores = fallo.FormattedMessagePlaceholderValues;

        return System.Text.RegularExpressions.Regex.Replace(plantilla, @"\{(\w+)\}", m =>
            valores is not null && valores.TryGetValue(m.Groups[1].Value, out var valor)
                ? Convert.ToString(valor, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty
                : m.Value);
    }
}
