using System.Text.Json;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Como se serializa el payload de cualquier notificacion.
/// </summary>
/// <remarks>
/// <para>
/// Vive en un solo sitio a proposito. Los tres serializadores del repo producen PascalCase por defecto y
/// el frontend deja de leer el payload <b>sin fallar y sin log</b>: no hay excepcion, no hay 500, solo un
/// campo que llega vacio (regla 12, seccion 5). Repetir las opciones en cada extension es repetir la
/// oportunidad de olvidarlas.
/// </para>
/// <para>
/// Lo mismo pasa con las fechas de NodaTime: sin sus convertidores, System.Text.Json escribe un <see cref="Instant"/>
/// como <c>{}</c> (no tiene propiedades publicas) y el aviso se guarda sin la fecha, otra vez sin error. Con ellos, un
/// <see cref="Instant"/> sale en ISO-8601 UTC con <c>Z</c>, el contrato de fechas de la plataforma, y una
/// <see cref="LocalDate"/> como <c>yyyy-MM-dd</c>.
/// </para>
/// </remarks>
internal static class NotificationSerialization
{
    /// <summary>
    /// camelCase, sin indentar y con las fechas de NodaTime en ISO-8601: lo que el frontend espera leer.
    /// </summary>
    internal static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    }.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

    /// <summary>
    /// Serializa el payload con las opciones de la plataforma.
    /// </summary>
    internal static string Serialize(object payload) => JsonSerializer.Serialize(payload, Options);
}
