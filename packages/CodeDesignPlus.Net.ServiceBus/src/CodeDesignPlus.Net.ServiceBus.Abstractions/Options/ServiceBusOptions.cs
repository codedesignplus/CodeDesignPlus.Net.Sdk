namespace CodeDesignPlus.Net.ServiceBus.Abstractions.Options;

/// <summary>
/// Options to setting of the Azure Service Bus transport for the pub/sub contract.
/// </summary>
public class ServiceBusOptions : PubSubOptions, IValidatableObject
{
    /// <summary>
    /// Name of the setings section on appsettings.
    /// </summary>
    public static new readonly string Section = "ServiceBus";

    /// <summary>
    /// Gets or sets a value indicating whether the Azure Service Bus transport is enabled.
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the readiness health checks are registered.
    /// </summary>
    public bool RegisterHealthCheck { get; set; } = true;

    /// <summary>
    /// Gets or sets the fully qualified namespace, such as <c>contoso.servicebus.windows.net</c>.
    /// </summary>
    /// <remarks>
    /// Es la via preferente: se combina con <c>DefaultAzureCredential</c> y no hay secreto que rotar.
    /// </remarks>
    public string FullyQualifiedNamespace { get; set; }

    /// <summary>
    /// Gets or sets the connection string used instead of Entra ID credentials.
    /// </summary>
    /// <remarks>
    /// Solo para desarrollo local y para el emulador, que unicamente admite su cadena estatica.
    /// Si tiene valor, tiene prioridad sobre <see cref="FullyQualifiedNamespace"/>.
    /// </remarks>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the connection string used for management operations, when it differs from <see cref="ConnectionString"/>.
    /// </summary>
    /// <remarks>
    /// Solo hace falta contra el emulador, que atiende el plano de gestion en un puerto distinto al de AMQP.
    /// Azure sirve ambos planos en el mismo extremo, asi que en cualquier entorno real se deja sin definir y
    /// se usa <see cref="ConnectionString"/>.
    /// </remarks>
    public string ManagementConnectionString { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether topics and subscriptions are created on demand.
    /// </summary>
    public bool AutoProvisionEntities { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of times a message is retried before it is dead-lettered.
    /// </summary>
    [Range(1, 100)]
    public int MaxRetry { get; set; } = 10;

    /// <summary>
    /// Gets or sets the base delay, in milliseconds, of the exponential backoff between retries.
    /// </summary>
    [Range(100, 60000)]
    public int RetryIntervalMs { get; set; } = 2000;

    /// <summary>
    /// Gets or sets the ceiling, in milliseconds, of the exponential backoff between retries.
    /// </summary>
    [Range(100, 300000)]
    public int MaxRetryIntervalMs { get; set; } = 60000;

    /// <summary>
    /// Gets or sets how many messages the processor handles concurrently per subscription.
    /// </summary>
    [Range(1, 100)]
    public int MaxConcurrentCalls { get; set; } = 4;

    /// <summary>
    /// Gets or sets how many messages are fetched ahead of time.
    /// </summary>
    /// <remarks>
    /// Cero a proposito: la espera del backoff retiene el bloqueo del mensaje, y un lote traido por adelantado
    /// consumiria su propio bloqueo mientras espera su turno.
    /// </remarks>
    [Range(0, 1000)]
    public int PrefetchCount { get; set; }

    /// <summary>
    /// Gets or sets the lock duration, in seconds, of a message being processed.
    /// </summary>
    [Range(5, 300)]
    public int LockDurationSeconds { get; set; } = 300;

    /// <summary>
    /// Gets or sets how long, in minutes, the client keeps renewing the lock of a message.
    /// </summary>
    [Range(1, 60)]
    public int MaxAutoLockRenewalMinutes { get; set; } = 10;

    /// <summary>
    /// Gets or sets the time to live, in hours, of a message sitting on a subscription.
    /// </summary>
    [Range(1, 8760)]
    public int MessageTimeToLiveHours { get; set; } = 48;

    /// <summary>
    /// Determines whether the specified object is valid.
    /// </summary>
    /// <param name="validationContext">The context that contains the object to validate.</param>
    /// <returns>A collection that holds the failed validation information.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (!this.Enable)
            return results;

        if (string.IsNullOrWhiteSpace(this.ConnectionString) && string.IsNullOrWhiteSpace(this.FullyQualifiedNamespace))
            results.Add(new ValidationResult(
                $"The {nameof(this.FullyQualifiedNamespace)} is required when no {nameof(this.ConnectionString)} is provided.",
                [nameof(this.FullyQualifiedNamespace)]));

        Validator.TryValidateProperty(this.MaxRetry, new ValidationContext(this, null, null) { MemberName = nameof(this.MaxRetry) }, results);
        Validator.TryValidateProperty(this.RetryIntervalMs, new ValidationContext(this, null, null) { MemberName = nameof(this.RetryIntervalMs) }, results);
        Validator.TryValidateProperty(this.MaxRetryIntervalMs, new ValidationContext(this, null, null) { MemberName = nameof(this.MaxRetryIntervalMs) }, results);
        Validator.TryValidateProperty(this.MaxConcurrentCalls, new ValidationContext(this, null, null) { MemberName = nameof(this.MaxConcurrentCalls) }, results);
        Validator.TryValidateProperty(this.PrefetchCount, new ValidationContext(this, null, null) { MemberName = nameof(this.PrefetchCount) }, results);
        Validator.TryValidateProperty(this.LockDurationSeconds, new ValidationContext(this, null, null) { MemberName = nameof(this.LockDurationSeconds) }, results);
        Validator.TryValidateProperty(this.MaxAutoLockRenewalMinutes, new ValidationContext(this, null, null) { MemberName = nameof(this.MaxAutoLockRenewalMinutes) }, results);
        Validator.TryValidateProperty(this.MessageTimeToLiveHours, new ValidationContext(this, null, null) { MemberName = nameof(this.MessageTimeToLiveHours) }, results);

        if (this.RetryIntervalMs > this.MaxRetryIntervalMs)
            results.Add(new ValidationResult(
                $"The {nameof(this.RetryIntervalMs)} cannot be greater than the {nameof(this.MaxRetryIntervalMs)}.",
                [nameof(this.RetryIntervalMs)]));

        // La espera del backoff transcurre con el bloqueo del mensaje retenido. Si el cliente deja de renovarlo
        // antes de que acabe la espera, el broker da el mensaje por abandonado y lo reentrega a otro consumidor:
        // el reintento se duplicaria y el contador de entregas correria el doble de rapido.
        if (this.MaxRetryIntervalMs >= this.MaxAutoLockRenewalMinutes * 60 * 1000)
            results.Add(new ValidationResult(
                $"The {nameof(this.MaxRetryIntervalMs)} must be lower than the {nameof(this.MaxAutoLockRenewalMinutes)} expressed in milliseconds, otherwise the message lock expires while the retry is waiting.",
                [nameof(this.MaxRetryIntervalMs)]));

        return results;
    }
}
