namespace CodeDesignPlus.Net.ServiceBus.Services;

/// <summary>
/// Creates the topics and subscriptions the transport needs, on demand and only once per process.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EntityProvisioner"/> class.
/// </remarks>
/// <param name="logger">The logger instance.</param>
/// <param name="clientProvider">The provider of the administration client.</param>
/// <param name="coreOptions">The core options that identify the owner of a topic.</param>
/// <param name="serviceBusOptions">The Azure Service Bus options.</param>
public class EntityProvisioner(ILogger<EntityProvisioner> logger, IServiceBusClientProvider clientProvider, IOptions<CoreOptions> coreOptions, IOptions<ServiceBusOptions> serviceBusOptions) : IEntityProvisioner
{
    private readonly CoreOptions coreOptions = coreOptions.Value;
    private readonly ServiceBusOptions options = serviceBusOptions.Value;
    private readonly ConcurrentDictionary<string, bool> provisionedTopics = new();
    private readonly ConcurrentDictionary<string, bool> provisionedSubscriptions = new();

    /// <summary>
    /// Ensures the specified topic exists.
    /// </summary>
    /// <param name="topic">The topic name.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task EnsureTopicAsync(string topic, CancellationToken cancellationToken)
    {
        if (!this.options.AutoProvisionEntities || this.provisionedTopics.ContainsKey(topic))
            return;

        try
        {
            if (!await clientProvider.AdministrationClient.TopicExistsAsync(topic, cancellationToken).ConfigureAwait(false))
            {
                var createTopicOptions = new CreateTopicOptions(topic)
                {
                    DefaultMessageTimeToLive = TimeSpan.FromHours(this.options.MessageTimeToLiveHours),
                    // Service Bus no admite argumentos libres como un exchange de AMQP, asi que la autoria del
                    // evento viaja aqui: es lo unico que permite saber en el portal de quien es cada topic.
                    UserMetadata = $"business={this.coreOptions.Business};microservice={this.coreOptions.AppName}"
                };

                await clientProvider.AdministrationClient.CreateTopicAsync(createTopicOptions, cancellationToken).ConfigureAwait(false);

                logger.LogInformation("Topic {Topic} created.", topic);
            }
        }
        catch (ServiceBusException exception) when (exception.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
        {
            // Otra replica gano la carrera. Es el resultado que se buscaba.
            logger.LogDebug("Topic {Topic} already created by another instance.", topic);
        }

        this.provisionedTopics.TryAdd(topic, true);
    }

    /// <summary>
    /// Ensures the specified subscription exists under the specified topic.
    /// </summary>
    /// <param name="topic">The topic name.</param>
    /// <param name="subscription">The subscription name.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task EnsureSubscriptionAsync(string topic, string subscription, CancellationToken cancellationToken)
    {
        if (!this.options.AutoProvisionEntities)
            return;

        await this.EnsureTopicAsync(topic, cancellationToken).ConfigureAwait(false);

        var key = $"{topic}/{subscription}";

        if (this.provisionedSubscriptions.ContainsKey(key))
            return;

        try
        {
            if (!await clientProvider.AdministrationClient.SubscriptionExistsAsync(topic, subscription, cancellationToken).ConfigureAwait(false))
            {
                var createSubscriptionOptions = new CreateSubscriptionOptions(topic, subscription)
                {
                    // Una entrega por encima del tope del consumidor. Decide primero el consumidor, que es quien
                    // sabe distinguir un error de negocio; el broker queda solo como red de seguridad para el caso
                    // de que el proceso muera antes de decidir.
                    MaxDeliveryCount = this.options.MaxRetry + 1,
                    DefaultMessageTimeToLive = TimeSpan.FromHours(this.options.MessageTimeToLiveHours),
                    LockDuration = TimeSpan.FromSeconds(this.options.LockDurationSeconds),
                    DeadLetteringOnMessageExpiration = true
                };

                await clientProvider.AdministrationClient.CreateSubscriptionAsync(createSubscriptionOptions, cancellationToken).ConfigureAwait(false);

                logger.LogInformation("Subscription {Subscription} created on topic {Topic}.", subscription, topic);
            }
        }
        catch (ServiceBusException exception) when (exception.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
        {
            logger.LogDebug("Subscription {Subscription} already created by another instance.", subscription);
        }

        this.provisionedSubscriptions.TryAdd(key, true);
    }
}
