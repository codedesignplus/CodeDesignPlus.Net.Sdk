using CodeDesignPlus.Net.RabbitMQ.Abstractions.Options;

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
    
    /// <summary>
    /// Connects to the RabbitMQ server.
    /// </summary>
    /// <param name="appName">The name of the application.</param>
    /// <param name="settings">The options for configuring the RabbitMQ connection.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="settings"/> or <paramref name="appName"/> is null.</exception>
    Task ConnectAsync(RabbitMQOptions settings, string appName);
}