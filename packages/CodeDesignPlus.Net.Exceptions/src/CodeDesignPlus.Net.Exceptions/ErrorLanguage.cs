namespace CodeDesignPlus.Net.Exceptions;

/// <summary>
/// El idioma en el que se traducen los errores de la peticion en curso.
/// </summary>
/// <remarks>
/// Es un <b>codigo de idioma</b> —<c>es</c>, <c>fr-CA</c>—, no una <see cref="System.Globalization.CultureInfo"/>.
/// La diferencia no es cosmetica: los entrypoints se compilan con
/// <c>&lt;InvariantGlobalization&gt;true&lt;/InvariantGlobalization&gt;</c> para no arrastrar ICU al
/// contenedor, y ahi <c>CultureInfo.GetCultureInfo("es")</c> <b>lanza excepcion</b>. Para elegir una
/// traduccion en un diccionario no hace falta una cultura, basta su nombre.
/// <para>
/// Viaja en un <see cref="AsyncLocal{T}"/> porque quien lanza el error esta en el dominio, sin acceso al
/// <c>HttpContext</c>, y quien lo traduce esta en el borde. Es lo mismo que hacia
/// <c>CurrentUICulture</c>, sin depender de ICU.
/// </para>
/// </remarks>
public static class ErrorLanguage
{
    private static readonly AsyncLocal<string?> Value = new();

    /// <summary>
    /// El idioma pedido por el cliente, o <c>null</c> para responder en ingles.
    /// </summary>
    public static string? Current
    {
        get => Value.Value;
        set => Value.Value = value;
    }
}
