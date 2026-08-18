namespace CodeDesignPlus.Net.RabbitMQ.Abstractions.Options;

/// <summary>
/// Represents the arguments for configuring a RabbitMQ queue.
/// </summary>
public class QueueArguments : IValidatableObject
{
    /// <summary>
    /// The queue type that keeps track of how many times a message was delivered.
    /// </summary>
    public const string Quorum = "quorum";

    /// <summary>
    /// Gets or sets the queue type (classic or quorum).
    /// </summary>
    /// <remarks>
    /// Solo las colas quorum publican la cabecera x-delivery-count y solo ellas admiten x-delivery-limit,
    /// que es lo unico que impide que un error de infraestructura reencole el mismo mensaje para siempre.
    /// Una clasica ademas vive en un unico nodo, asi que se cae con el.
    /// </remarks>
    [RegularExpression(@"^(classic|quorum)?$")]
    public string QueueType { get; set; } = Quorum;

    /// <summary>
    /// Gets or sets the maximum number of deliveries of a message before the broker dead-letters it.
    /// </summary>
    /// <remarks>
    /// Es la red de seguridad del broker: aunque el consumidor muera antes de decidir, o no sepa leer la
    /// cuenta de entregas, el mensaje acaba en la DLQ. Solo aplica a colas quorum.
    /// </remarks>
    public int? DeliveryLimit { get; set; }

    /// <summary>
    /// Gets or sets the message time-to-live (TTL) in milliseconds.
    /// </summary>
    public int MessageTtl { get; set; } = 172800000;

    /// <summary>
    /// Gets or sets the expiration time for the queue in milliseconds.
    /// </summary>
    public int? Expires { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of messages in the queue.
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Gets or sets the maximum size of the queue in bytes.
    /// </summary>
    public int? MaxLengthBytes { get; set; }

    /// <summary>
    /// Gets or sets the maximum priority for the queue.
    /// </summary>
    [Range(0, 255)]
    public int? MaxPriority { get; set; }

    /// <summary>
    /// Gets or sets the queue mode. Only valid on classic queues.
    /// </summary>
    [RegularExpression(@"^(default|lazy)?$")]
    public string QueueMode { get; set; }

    /// <summary>
    /// Gets or sets the queue master locator. Only valid on classic queues.
    /// </summary>
    [RegularExpression(@"^(min-masters)?$")]
    public string QueueMasterLocator { get; set; }

    /// <summary>
    /// Gets or sets the overflow behavior for the queue.
    /// </summary>
    [RegularExpression(@"^(drop-head|reject-publish)?$")]
    public string Overflow { get; set; }

    /// <summary>
    /// Gets or sets the overflow reject publish value.
    /// </summary>
    public int? OverflowRejectPublish { get; set; }

    /// <summary>
    /// Gets or sets additional arguments for the queue.
    /// </summary>
    public Dictionary<string, object> ExtraArguments { get; set; }

    /// <summary>
    /// Gets a value indicating whether the queue is declared as a quorum queue.
    /// </summary>
    public bool IsQuorum => string.Equals(this.QueueType, Quorum, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the dictionary of arguments for the queue.
    /// </summary>
    /// <returns>A dictionary of queue arguments.</returns>
    public Dictionary<string, object> GetArguments()
    {
        var arguments = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(this.QueueType))
            arguments.Add("x-queue-type", this.QueueType);

        if (this.MessageTtl > 0)
            arguments.Add("x-message-ttl", this.MessageTtl);

        if (this.Expires.HasValue)
            arguments.Add("x-expires", this.Expires.Value);

        if (this.MaxLength.HasValue)
            arguments.Add("x-max-length", this.MaxLength.Value);

        if (this.MaxLengthBytes.HasValue)
            arguments.Add("x-max-length-bytes", this.MaxLengthBytes.Value);

        // x-delivery-limit es lo que corta el reencolado infinito, pero solo lo entiende una cola quorum.
        if (this.IsQuorum && this.DeliveryLimit.HasValue)
            arguments.Add("x-delivery-limit", this.DeliveryLimit.Value);

        // Una cola quorum rechaza estos tres argumentos y el broker cierra el canal al declararla, con lo que el
        // consumidor se queda sin suscripcion y en silencio. Se emiten solo cuando la cola es clasica.
        if (!this.IsQuorum)
        {
            if (this.MaxPriority.HasValue)
                arguments.Add("x-max-priority", this.MaxPriority.Value);

            if (!string.IsNullOrWhiteSpace(this.QueueMode))
                arguments.Add("x-queue-mode", this.QueueMode);

            if (!string.IsNullOrWhiteSpace(this.QueueMasterLocator))
                arguments.Add("x-queue-master-locator", this.QueueMasterLocator);
        }

        // x-ha-policy no se emite: el espejado de colas clasicas desaparecio en RabbitMQ 4 y ademas nunca fue un
        // argumento de cola, sino una politica del broker. Declararlo solo ensuciaba los argumentos.

        if (!string.IsNullOrWhiteSpace(this.Overflow))
            arguments.Add("x-overflow", this.Overflow);

        if (this.OverflowRejectPublish.HasValue)
            arguments.Add("x-overflow-reject-publish", this.OverflowRejectPublish);

        if (this.ExtraArguments != null)
            arguments.Add("x-arguments", this.ExtraArguments);

        return arguments;
    }

    /// <summary>
    /// Validates the properties of the <see cref="QueueArguments"/> instance.
    /// </summary>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (this.MessageTtl < 0)
            results.Add(new ValidationResult("The field MessageTtl must be greater than or equal to zero.", new[] { nameof(this.MessageTtl) }));

        if (this.Expires.HasValue && this.Expires.Value < 0)
            results.Add(new ValidationResult("The field Expires must be greater than or equal to zero.", new[] { nameof(this.Expires) }));

        if (this.MaxLength.HasValue && this.MaxLength.Value < 0)
            results.Add(new ValidationResult("The field MaxLength must be greater than or equal to zero.", new[] { nameof(this.MaxLength) }));

        if (this.MaxLengthBytes.HasValue && this.MaxLengthBytes.Value < 0)
            results.Add(new ValidationResult("The field MaxLengthBytes must be greater than or equal to zero.", new[] { nameof(this.MaxLengthBytes) }));

        if (this.MaxPriority.HasValue && this.MaxPriority.Value < 0)
            Validator.TryValidateProperty(this.MaxPriority, new ValidationContext(this, null, null) { MemberName = nameof(this.MaxPriority) }, results);

        if (this.OverflowRejectPublish.HasValue && this.OverflowRejectPublish.Value < 0)
            results.Add(new ValidationResult("The field OverflowRejectPublish must be greater than or equal to zero.", new[] { nameof(this.OverflowRejectPublish) }));

        if (this.DeliveryLimit.HasValue && this.DeliveryLimit.Value < 1)
            results.Add(new ValidationResult("The field DeliveryLimit must be greater than or equal to one.", new[] { nameof(this.DeliveryLimit) }));

        ValidateQueueType(results);

        ValidateExtraArguments(results);

        return results;
    }

    /// <summary>
    /// Validates that the queue type does not clash with arguments that only exist on classic queues.
    /// </summary>
    /// <param name="results">The list of validation results.</param>
    /// <remarks>
    /// Se avisa al arrancar y no al declarar la cola: un argumento incompatible se manifiesta como un canal
    /// cerrado por el broker en mitad de la suscripcion, que es mucho mas caro de leer que un error de opciones.
    /// </remarks>
    private void ValidateQueueType(List<ValidationResult> results)
    {
        if (!this.IsQuorum)
            return;

        if (this.MaxPriority.HasValue)
            results.Add(new ValidationResult("The field MaxPriority is not supported by quorum queues.", new[] { nameof(this.MaxPriority) }));

        if (!string.IsNullOrWhiteSpace(this.QueueMode))
            results.Add(new ValidationResult("The field QueueMode is not supported by quorum queues.", new[] { nameof(this.QueueMode) }));

        if (!string.IsNullOrWhiteSpace(this.QueueMasterLocator))
            results.Add(new ValidationResult("The field QueueMasterLocator is not supported by quorum queues.", new[] { nameof(this.QueueMasterLocator) }));
    }

    /// <summary>
    /// Validates the extra arguments for the queue.
    /// </summary>
    /// <param name="results">The list of validation results.</param>
    private void ValidateExtraArguments(List<ValidationResult> results)
    {
        if (this.ExtraArguments != null)
        {
            foreach (var item in this.ExtraArguments)
            {
                if (item.Value == null)
                    results.Add(new ValidationResult($"The field {item.Key} of the ExtraArguments cannot be null.", new[] { nameof(this.ExtraArguments) }));
            }
        }
    }
}