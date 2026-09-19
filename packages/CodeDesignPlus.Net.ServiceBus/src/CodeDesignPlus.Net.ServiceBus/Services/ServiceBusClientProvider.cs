namespace CodeDesignPlus.Net.ServiceBus.Services;

/// <summary>
/// Creates and caches the Azure Service Bus clients used by the transport.
/// </summary>
public sealed class ServiceBusClientProvider : IServiceBusClientProvider
{
    private readonly ServiceBusOptions options;
    private readonly ServiceBusClient client;
    private readonly ConcurrentDictionary<string, ServiceBusSender> senders = new();
    private readonly ConcurrentDictionary<string, ServiceBusProcessor> processors = new();

    /// <summary>
    /// Gets the administration client used to provision topics and subscriptions.
    /// </summary>
    public ServiceBusAdministrationClient AdministrationClient { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusClientProvider"/> class.
    /// </summary>
    /// <param name="options">The Azure Service Bus options.</param>
    public ServiceBusClientProvider(IOptions<ServiceBusOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        this.options = options.Value;

        // La cadena de conexion gana cuando esta presente porque es el unico modo que admite el emulador y el
        // desarrollo local. En el cluster no se define y entra Entra ID, que no deja secreto que rotar.
        if (!string.IsNullOrWhiteSpace(this.options.ConnectionString))
        {
            this.client = new ServiceBusClient(this.options.ConnectionString);

            // El emulador es el unico que separa ambos planos; en Azure los dos salen del mismo extremo.
            var managementConnectionString = string.IsNullOrWhiteSpace(this.options.ManagementConnectionString)
                ? this.options.ConnectionString
                : this.options.ManagementConnectionString;

            this.AdministrationClient = new ServiceBusAdministrationClient(managementConnectionString);
        }
        else
        {
            var credential = new DefaultAzureCredential();

            this.client = new ServiceBusClient(this.options.FullyQualifiedNamespace, credential);
            this.AdministrationClient = new ServiceBusAdministrationClient(this.options.FullyQualifiedNamespace, credential);
        }
    }

    /// <summary>
    /// Gets a cached sender for the specified topic.
    /// </summary>
    /// <param name="topic">The topic name.</param>
    /// <returns>The sender bound to the topic.</returns>
    public ServiceBusSender GetSender(string topic)
    {
        ArgumentException.ThrowIfNullOrEmpty(topic);

        return this.senders.GetOrAdd(topic, this.client.CreateSender);
    }

    /// <summary>
    /// Creates a processor for the specified topic and subscription and keeps it for later disposal.
    /// </summary>
    /// <param name="topic">The topic name.</param>
    /// <param name="subscription">The subscription name.</param>
    /// <returns>The processor bound to the subscription.</returns>
    public ServiceBusProcessor GetProcessor(string topic, string subscription)
    {
        ArgumentException.ThrowIfNullOrEmpty(topic);
        ArgumentException.ThrowIfNullOrEmpty(subscription);

        return this.processors.GetOrAdd(Key(topic, subscription), _ => this.client.CreateProcessor(topic, subscription, new ServiceBusProcessorOptions
        {
            // Manual: el mensaje solo se completa cuando el handler termina bien. Con la liquidacion automatica
            // un handler que falla quedaria completado igualmente y el evento se perderia sin pasar por la DLQ.
            ReceiveMode = ServiceBusReceiveMode.PeekLock,
            AutoCompleteMessages = false,
            MaxConcurrentCalls = this.options.MaxConcurrentCalls,
            PrefetchCount = this.options.PrefetchCount,
            MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(this.options.MaxAutoLockRenewalMinutes)
        }));
    }

    /// <summary>
    /// Gets the processor previously created for the specified topic and subscription, if any.
    /// </summary>
    /// <param name="topic">The topic name.</param>
    /// <param name="subscription">The subscription name.</param>
    /// <returns>The processor, or <see langword="null"/> when it was never created.</returns>
    public ServiceBusProcessor FindProcessor(string topic, string subscription)
    {
        this.processors.TryGetValue(Key(topic, subscription), out var processor);

        return processor;
    }

    /// <summary>
    /// Builds the cache key of a processor.
    /// </summary>
    /// <param name="topic">The topic name.</param>
    /// <param name="subscription">The subscription name.</param>
    /// <returns>The cache key.</returns>
    private static string Key(string topic, string subscription) => $"{topic}/{subscription}";

    /// <summary>
    /// Disposes the senders, the processors and the underlying client.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        foreach (var processor in this.processors.Values)
            await processor.DisposeAsync().ConfigureAwait(false);

        foreach (var sender in this.senders.Values)
            await sender.DisposeAsync().ConfigureAwait(false);

        this.processors.Clear();
        this.senders.Clear();

        await this.client.DisposeAsync().ConfigureAwait(false);
    }
}
