using System.Text.Json;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Como se serializa el payload de cualquier notificacion.
/// </summary>
/// <remarks>
/// Vive en un solo sitio a proposito. Los tres serializadores del repo producen PascalCase por defecto y
/// el frontend deja de leer el payload <b>sin fallar y sin log</b>: no hay excepcion, no hay 500, solo un
/// campo que llega vacio (regla 12, seccion 5). Repetir las opciones en cada extension es repetir la
/// oportunidad de olvidarlas.
/// </remarks>
internal static class NotificationSerialization
{
    /// <summary>camelCase, sin indentar: lo que el frontend espera leer.</summary>
    internal static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>Serializa el payload con las opciones de la plataforma.</summary>
    internal static string Serialize(object payload) => JsonSerializer.Serialize(payload, Options);
}
