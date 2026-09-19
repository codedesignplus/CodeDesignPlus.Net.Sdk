namespace CodeDesignPlus.Net.ServiceBus.Abstractions;

/// <summary>
/// Provides and caches the Azure Service Bus clients used to publish and consume domain events.
/// </summary>
/// <remarks>
/// Abrir un emisor o un procesador por mensaje agotaria el limite de conexiones concurrentes de la entidad,
/// asi que se cachean por destino, igual que el proveedor de canales de RabbitMQ.
/// </remarks>
public interface IServiceBusClientProvider : IAsyncDisposable
{
    /// <summary>
    /// Gets the administration client used to provision topics and subscriptions.
    /// </summary>
    ServiceBusAdministrationClient AdministrationClient { get; }

    /// <summary>
    /// Gets a cached sender for the specified topic.
    /// </summary>
    /// <param name="topic">The topic name.</param>
    /// <returns>The sender bound to the topic.</returns>
    ServiceBusSender GetSender(string topic);

    /// <summary>
    /// Creates a processor for the specified topic and subscription and keeps it for later disposal.
    /// </summary>
    /// <param name="topic">The topic name.</param>
    /// <param name="subscription">The subscription name.</param>
    /// <returns>The processor bound to the subscription.</returns>
    ServiceBusProcessor GetProcessor(string topic, string subscription);

    /// <summary>
    /// Gets the processor previously created for the specified topic and subscription, if any.
    /// </summary>
    /// <param name="topic">The topic name.</param>
    /// <param name="subscription">The subscription name.</param>
    /// <returns>The processor, or <see langword="null"/> when it was never created.</returns>
    ServiceBusProcessor FindProcessor(string topic, string subscription);
}
