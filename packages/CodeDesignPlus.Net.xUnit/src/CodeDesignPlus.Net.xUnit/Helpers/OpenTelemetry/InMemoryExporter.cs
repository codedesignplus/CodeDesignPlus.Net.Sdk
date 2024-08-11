using System.Diagnostics;
using OpenTelemetry;
using System.Collections.Concurrent;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;

namespace CodeDesignPlus.Net.xUnit.Helpers.OpenTelemetry;

public class InMemoryTraceExporter : BaseExporter<Activity>
{
    public static ConcurrentDictionary<string, Activity> Exporters { get; } = new ();
    public override ExportResult Export(in Batch<Activity> batch)
    {
        foreach (var activity in batch)
        {
            Exporters.TryAdd(activity.OperationName, activity);
        }

        return ExportResult.Success;
    }
}


public class InMemoryLogExporter : BaseExporter<LogRecord>
{
    public static ConcurrentDictionary<string, LogRecord> Exporters { get; } = new ();
    public override ExportResult Export(in Batch<LogRecord> batch)
    {
        foreach (var log in batch)
        {
            Exporters.TryAdd(log.SpanId.ToString(), log);
        }

        return ExportResult.Success;
    }
}

public class InMemoryMetricsExporter : BaseExporter<Metric>
{
    public static ConcurrentDictionary<string, Metric> Exporters { get; } = new ();
    public override ExportResult Export(in Batch<Metric> batch)
    {
        foreach (var metric in batch)
        {
            Exporters.TryAdd(metric.Name, metric);
        }

        return ExportResult.Success;
    }
}