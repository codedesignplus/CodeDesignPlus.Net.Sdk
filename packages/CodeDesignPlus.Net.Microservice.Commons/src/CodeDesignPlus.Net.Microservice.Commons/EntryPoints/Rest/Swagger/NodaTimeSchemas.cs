using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using NodaTime;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Describe los tipos de NodaTime como viajan en el cable con <c>ConfigureForNodaTime</c>: texto, no objetos.
/// </summary>
/// <remarks>
/// Sin esto Swashbuckle los describe por reflexión: <see cref="Instant"/> sale como un objeto vacío y
/// <see cref="LocalDate"/> como un objeto con <c>calendar</c>, <c>era</c> y <c>year</c>, cuando el micro envía y
/// espera <c>"2026-11-25T15:00:00Z"</c> y <c>"2026-11-25"</c> (pendings/753).
/// </remarks>
public static class NodaTimeSchemas
{
    /// <summary>
    /// Registra el esquema de cada tipo de NodaTime, y el de su versión que admite nulo.
    /// </summary>
    /// <param name="options">Las opciones de SwaggerGen.</param>
    public static void MapNodaTimeTypes(this SwaggerGenOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        Map<Instant>(options, "date-time", "2026-11-25T15:00:00Z", "An instant in UTC, ISO 8601.");
        Map<OffsetDateTime>(options, "date-time", "2026-11-25T10:00:00-05:00", "A date and time with its UTC offset, ISO 8601.");
        Map<LocalDate>(options, "date", "2026-11-25", "A calendar date without time zone, yyyy-MM-dd.");
        Map<LocalDateTime>(options, null, "2026-11-25T10:00:00", "A date and time without time zone, yyyy-MM-ddTHH:mm:ss.");
        Map<LocalTime>(options, null, "10:00:00", "A time of day without time zone, HH:mm:ss.");
        Map<ZonedDateTime>(options, null, "2026-11-25T10:00:00-05 America/Bogota", "A date and time in an IANA time zone.");
        Map<Duration>(options, null, "1:30:00", "A duration in hours, minutes and seconds, H:mm:ss; the hours can exceed 24.");
        Map<Offset>(options, null, "-05", "A UTC offset.");
        MapReference<Period>(options, null, "P1DT2H", "A period, ISO 8601.");
        MapReference<DateTimeZone>(options, null, "America/Bogota", "An IANA time zone identifier.");
    }

    /// <summary>
    /// Un tipo valor: se registra él y su <see cref="Nullable{T}"/>.
    /// </summary>
    private static void Map<T>(SwaggerGenOptions options, string format, string example, string description) where T : struct
    {
        options.MapType<T>(() => Create(format, example, description, false));
        options.MapType<T?>(() => Create(format, example, description, true));
    }

    /// <summary>
    /// Un tipo referencia: un solo registro, que admite nulo.
    /// </summary>
    private static void MapReference<T>(SwaggerGenOptions options, string format, string example, string description) where T : class
    {
        options.MapType<T>(() => Create(format, example, description, true));
    }

    private static OpenApiSchema Create(string format, string example, string description, bool nullable) => new()
    {
        Type = "string",
        Format = format,
        Example = new OpenApiString(example),
        Description = description,
        Nullable = nullable
    };
}
