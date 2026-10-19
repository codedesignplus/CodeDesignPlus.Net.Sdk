namespace CodeDesignPlus.Net.PubSub.Services;

/// <summary>
/// Service responsible for publishing domain events.
/// </summary>
/// <remarks>
/// Con la bandeja de salida activa (<see cref="OutboxOptions"/>), publicar es encolar el evento en la base del micro y
/// avisar al despachador, que lo envía después. Sin ella, se envía directo. En los dos casos se usa un plazo propio y no
/// el token de quien publica: el de una petición HTTP se cancela cuando el cliente se va, y un registro ya guardado se
/// quedaba sin su evento (pendings/245).
/// </remarks>
public class PubSubService : IPubSub
{
    private readonly IMessage message;
    private readonly PubSubOptions options;
    private readonly IDomainEventResolver domainEventResolver;
    private readonly IOutboxSignal outboxSignal;
    private readonly ILogger<PubSubService> logger;
    private readonly IOutboxStore outboxStore;
    private readonly IActivityService activityService;
    private readonly bool useOutbox;

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubService"/> class.
    /// </summary>
    /// <param name="message">The message service used for publishing events.</param>
    /// <param name="options">The options for configuring the PubSub service.</param>
    /// <param name="domainEventResolver">Resuelve la clave de cada evento, que es su destino.</param>
    /// <param name="outboxSignal">El aviso al despachador de la bandeja de salida.</param>
    /// <param name="logger">The logger for logging information.</param>
    /// <param name="outboxStore">La bandeja de salida, si hay una registrada.</param>
    /// <param name="outboxTransport">El transporte que envía lo encolado, si el registrado sabe hacerlo.</param>
    /// <param name="activityService">The activity service for distributed tracing (optional).</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the required parameters are null.</exception>
    public PubSubService(IMessage message, IOptions<PubSubOptions> options, IDomainEventResolver domainEventResolver, IOutboxSignal outboxSignal, ILogger<PubSubService> logger, IOutboxStore outboxStore = null, IOutboxTransport outboxTransport = null, IActivityService activityService = null)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(domainEventResolver);
        ArgumentNullException.ThrowIfNull(outboxSignal);
        ArgumentNullException.ThrowIfNull(logger);

        this.message = message;
        this.options = options.Value;
        this.domainEventResolver = domainEventResolver;
        this.outboxSignal = outboxSignal;
        this.logger = logger;
        this.outboxStore = outboxStore;
        this.activityService = activityService;
        this.useOutbox = this.options.Outbox.Enable && outboxStore is not null && outboxTransport is not null;

        if (this.options.Outbox.Enable && !this.useOutbox)
        {
            this.logger.LogWarning("The outbox is enabled but there is no {Store} or no {Transport}: events are published directly.", nameof(IOutboxStore), nameof(IOutboxTransport));
        }

        this.logger.LogDebug("PubSubService initialized.");
    }

    /// <summary>
    /// Publishes a single domain event asynchronously.
    /// </summary>
    /// <param name="event">The domain event to publish.</param>
    /// <param name="cancellationToken">El token de quien publica. No se usa para encolar ni para enviar (ver la clase).</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    public Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(@event);

        return this.PublishAsync([@event], cancellationToken);
    }

    /// <summary>
    /// Publishes a list of domain events asynchronously.
    /// </summary>
    /// <param name="event">The list of domain events to publish.</param>
    /// <param name="cancellationToken">El token de quien publica. No se usa para encolar ni para enviar (ver la clase).</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    public async Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(@event);

        if (@event.Count == 0)
        {
            return;
        }

        if (!this.useOutbox)
        {
            await this.PublishDirectlyAsync(@event).ConfigureAwait(false);

            return;
        }

        try
        {
            await this.EnqueueAsync(@event).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            // Lo ya guardado no se queda sin su evento porque la bandeja falle: se intenta el envío directo, y solo si
            // también falla le llega el error a quien publica.
            this.logger.LogError(exception, "Could not enqueue {Count} event(s) in the outbox; publishing them directly.", @event.Count);

            await this.PublishDirectlyAsync(@event).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Serializa los eventos, los escribe en la bandeja y avisa al despachador.
    /// </summary>
    private async Task EnqueueAsync(IReadOnlyList<IDomainEvent> events)
    {
        var now = SystemClock.Instance.GetCurrentInstant();

        var messages = events.Select(x => this.ToOutboxMessage(x, now)).ToList();

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(this.options.PublishTimeoutSeconds));

        await this.outboxStore.AddAsync(messages, timeout.Token).ConfigureAwait(false);

        this.outboxSignal.Notify();

        this.logger.LogDebug("{Count} event(s) enqueued in the outbox.", messages.Count);
    }

    /// <summary>
    /// Convierte un evento en el mensaje que guarda la bandeja, con la traza en curso en sus metadatos y en las cabeceras.
    /// </summary>
    private OutboxMessage ToOutboxMessage(IDomainEvent @event, Instant now)
    {
        var topic = this.domainEventResolver.GetKeyDomainEvent(@event.GetType());

        using var activity = this.activityService?.StartActivity($"outbox {topic}", ActivityKind.Producer);

        activity?.AddTag("messaging.operation.type", "create");
        activity?.AddTag("messaging.destination.name", topic);
        activity?.AddTag("event.type", @event.GetType().Name);
        activity?.AddTag("event.id", @event.EventId.ToString());
        activity?.AddTag("event.aggregate_id", @event.AggregateId.ToString());

        this.activityService?.Inject(activity, @event);

        var headers = new Dictionary<string, object>();

        this.activityService?.InjectToHeaders(activity, headers);

        var body = JsonSerializer.Serialize(@event);

        return new OutboxMessage(@event.EventId, topic, @event.GetType().Name, @event.AggregateId, body, headers.ToDictionary(x => x.Key, x => x.Value?.ToString()), now, 0);
    }

    /// <summary>
    /// Envía los eventos al bus con un plazo propio.
    /// </summary>
    private async Task PublishDirectlyAsync(IReadOnlyList<IDomainEvent> events)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(this.options.PublishTimeoutSeconds));

        await Task.WhenAll(events.Select(x => this.message.PublishAsync(x, timeout.Token))).ConfigureAwait(false);
    }
}
