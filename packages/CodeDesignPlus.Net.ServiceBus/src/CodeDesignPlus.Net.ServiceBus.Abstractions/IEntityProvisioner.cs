namespace CodeDesignPlus.Net.ServiceBus.Abstractions;

/// <summary>
/// Creates the Azure Service Bus entities the transport needs.
/// </summary>
/// <remarks>
/// El publicador crea su topic y el consumidor su suscripcion, para que un entorno recien creado arranque en
/// frio sin intervencion. Un consumidor suscrito a un evento que nadie publica seguira pareciendo correcto en
/// el broker: eso no lo detecta el runtime, lo detecta el guardarrail estatico de gemelos.
/// </remarks>
public interface IEntityProvisioner
{
    /// <summary>
    /// Ensures the specified topic exists.
    /// </summary>
    /// <param name="topic">The topic name.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task EnsureTopicAsync(string topic, CancellationToken cancellationToken);

    /// <summary>
    /// Ensures the specified subscription exists under the specified topic.
    /// </summary>
    /// <param name="topic">The topic name.</param>
    /// <param name="subscription">The subscription name.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task EnsureSubscriptionAsync(string topic, string subscription, CancellationToken cancellationToken);
}
