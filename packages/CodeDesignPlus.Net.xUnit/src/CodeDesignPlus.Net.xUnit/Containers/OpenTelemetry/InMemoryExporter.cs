using System.Diagnostics;
using OpenTelemetry;
using System.Collections.Concurrent;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;

namespace CodeDesignPlus.Net.xUnit.Containers.OpenTelemetry;

/// <summary>
/// An in-memory exporter for tracing activities, used for testing purposes.
/// </summary>
public class InMemoryTraceExporter : BaseExporter<Activity>
{
    /// <summary>
    /// Gets the collection of exported activities.
    /// </summary>
    public static ConcurrentDictionary<string, Activity> Exporters { get; } = new ();

    /// <summary>
    /// Exports a batch of activities to the in-memory collection.
    /// </summary>
    /// <param name="batch">The batch of activities to export.</param>
    /// <returns>The result of the export operation.</returns>
    public override ExportResult Export(in Batch<Activity> batch)
    {
        foreach (var activity in batch)
        {
            Exporters.TryAdd(activity.OperationName, activity);
        }

        return ExportResult.Success;
    }
}

/// <summary>
/// An in-memory exporter for log records, used for testing purposes.
/// </summary>
public class InMemoryLogExporter : BaseExporter<LogRecord>
{
    /// <summary>
    /// Gets the collection of exported log records.
    /// </summary>
    public static ConcurrentDictionary<string, LogRecord> Exporters { get; } = new ();

    /// <summary>
    /// Exports a batch of log records to the in-memory collection.
    /// </summary>
    /// <param name="batch">The batch of log records to export.</param>
    /// <returns>The result of the export operation.</returns>
    public override ExportResult Export(in Batch<LogRecord> batch)
    {
        foreach (var log in batch)
        {
            Exporters.TryAdd(log.SpanId.ToString(), log);
        }

        return ExportResult.Success;
    }
}

/// <summary>
/// An in-memory exporter for metrics, used for testing purposes.
/// </summary>
public class InMemoryMetricsExporter : BaseExporter<Metric>
{
    /// <summary>
    /// Gets the collection of exported metrics.
    /// </summary>
    public static ConcurrentDictionary<string, Metric> Exporters { get; } = new ();

    /// <summary>
    /// Exports a batch of metrics to the in-memory collection.
    /// </summary>
    /// <param name="batch">The batch of metrics to export.</param>
    /// <returns>The result of the export operation.</returns>
    public override ExportResult Export(in Batch<Metric> batch)
    {
        foreach (var metric in batch)
        {
            Exporters.TryAdd(metric.Name, metric);
        }

        return ExportResult.Success;
    }
}