namespace CodeDesignPlus.Net.Exceptions.Models;

/// <summary>
/// Represents the error details of a request.
/// </summary>
/// <param name="Code">The error code.</param>
/// <param name="Field">The field that contains the error.</param>
/// <param name="Message">The error message.</param>
public record ErrorDetail(string Code, string Field, string Message)
{
    /// <summary>
    /// The layer that declares the error: <c>Domain</c>, <c>Application</c> or <c>Infrastructure</c>.
    /// </summary>
    /// <remarks>
    /// Va como propiedad y no como parametro posicional para no tocar los cinco sitios que ya construyen
    /// este record. Se deduce del espacio de nombres del catalogo que lo declara, que es donde vive el dato:
    /// el codigo por si solo no dice la capa, y hasta ahora `/errors` obligaba a deducirla por centenas.
    /// </remarks>
    public string? Layer { get; init; }
}