using System.Diagnostics;
using OpenTelemetry;

namespace CodeDesignPlus.Net.Observability.Processors;

/// <summary>
/// Descarta los spans que no cuentan nada de lo que hace el servicio: los comandos Redis sin traza
/// padre y las llamadas del propio envío de telemetría al colector.
/// </summary>
/// <remarks>
/// <para>
/// Un comando Redis sin padre no está en el camino de ningún usuario: lo lanza un hilo de fondo, casi
/// siempre el sondeo de Hangfire, que comparte la conexión del micro. En ms-filestorage eran el 84 % de
/// todos los spans del sistema. Los comandos que cuelgan de una petición o de un evento se conservan,
/// porque son los que miden la latencia de Redis donde alguien espera.
/// </para>
/// <para>
/// El sink OTLP de Serilog exporta los logs por gRPC, y la instrumentación del cliente gRPC lo traza:
/// cada lote de logs abría una traza nueva hacia el colector. El exportador de trazas de OpenTelemetry
/// ya suprime su propia instrumentación; el de Serilog no.
/// </para>
/// <para>
/// Se descarta quitando la marca <see cref="ActivityTraceFlags.Recorded"/>: los procesadores de
/// exportación ignoran un span sin ella. Por eso tiene que registrarse antes del exportador.
/// </para>
/// </remarks>
public class TelemetryNoiseFilterProcessor : BaseProcessor<Activity>
{
    /// <summary>
    /// Nombre de la fuente de la instrumentación de StackExchange.Redis.
    /// </summary>
    public const string RedisSourceName = "OpenTelemetry.Instrumentation.StackExchangeRedis";

    /// <summary>
    /// Prefijo de los servicios gRPC del protocolo OTLP (trazas, métricas y logs).
    /// </summary>
    public const string CollectorServicePrefix = "opentelemetry.proto.collector.";

    /// <inheritdoc/>
    public override void OnEnd(Activity data)
    {
        if (data is null)
            return;

        if (IsOrphanRedisCommand(data) || IsExportToCollector(data))
            data.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;
    }

    /// <summary>
    /// Si el span es un comando Redis lanzado fuera de cualquier traza.
    /// </summary>
    private static bool IsOrphanRedisCommand(Activity activity)
        => activity.Source.Name == RedisSourceName && activity.ParentSpanId == default;

    /// <summary>
    /// Si el span es una llamada al colector OTLP: el span gRPC lleva el servicio en su nombre, y el
    /// span HTTP que cuelga de él lo lleva en la URL.
    /// </summary>
    private static bool IsExportToCollector(Activity activity)
    {
        if (activity.DisplayName.StartsWith(CollectorServicePrefix, StringComparison.Ordinal))
            return true;

        return activity.GetTagItem("url.full") is string url
            && url.Contains($"/{CollectorServicePrefix}", StringComparison.Ordinal);
    }
}
