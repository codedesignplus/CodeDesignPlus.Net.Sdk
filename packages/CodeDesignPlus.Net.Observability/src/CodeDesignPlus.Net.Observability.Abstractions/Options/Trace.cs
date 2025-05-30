namespace CodeDesignPlus.Net.Observability.Abstractions.Options;

/// <summary>
/// Represents the trace options for configuring observability.
/// </summary>
public class Trace
{
    /// <summary>
    /// Gets or sets a value indicating whether tracing is enabled.
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether ASP.NET Core instrumentation is enabled for tracing.
    /// </summary>
    public bool AspNetCore { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether CodeDesignPlus SDK instrumentation is enabled for tracing.
    /// </summary>
    public bool CodeDesignPlusSdk { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether Redis instrumentation is enabled for tracing.
    /// </summary>
    public bool Redis { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether Kafka instrumentation is enabled for tracing.
    /// </summary>
    public bool Kafka { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether SQL Client instrumentation is enabled for tracing.
    /// </summary>
    public bool SqlClient { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether gRPC Client instrumentation is enabled for tracing.
    /// </summary>
    public bool GrpcClient { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether RabbitMQ instrumentation is enabled for tracing.
    /// </summary>
    public bool RabbitMQ { get; set; }
}