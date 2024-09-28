namespace CodeDesignPlus.Net.RabbitMQ.Abstractions;

/// <summary>
/// Represents a RabbitMQ connection.
/// </summary>
public interface IRabbitConnection : IDisposable
{
    /// <summary>
    /// Gets the RabbitMQ connection.
    /// </summary>
    IConnection Connection { get; }
}