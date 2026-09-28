namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Longitudes máximas estándar de los campos de texto libre (regla 48 de rules/).
/// </summary>
/// <remarks>
/// El validador del command y el <c>maxlength</c> de la pantalla usan el mismo número, así que un texto que la
/// pantalla acepta nunca lo rechaza el servidor. Los campos con formato propio (códigos ISO, correo, teléfono,
/// documento) no entran aquí: su longitud la fija su formato.
/// </remarks>
public static class FieldLength
{
    /// <summary>
    /// Nombre de un registro: «Residencial», «Torre A», el nombre de una persona.
    /// </summary>
    public const int Name = 100;

    /// <summary>
    /// Título de un documento, un aviso o una publicación.
    /// </summary>
    public const int Title = 200;

    /// <summary>
    /// Descripción de un registro, en una o dos frases.
    /// </summary>
    public const int Description = 500;

    /// <summary>
    /// Texto largo: notas, observaciones, comentarios, el cuerpo de un reglamento.
    /// </summary>
    public const int Notes = 2000;
}
