namespace CodeDesignPlus.Net.ServiceBus.Services;

/// <summary>
/// Resolves the Azure Service Bus subscription name owned by an event handler.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SubscriptionNameResolver"/> class.
/// </remarks>
/// <param name="coreOptions">The core options that provide the application identity.</param>
public class SubscriptionNameResolver(IOptions<CoreOptions> coreOptions) : ISubscriptionNameResolver
{
    /// <summary>
    /// Maximum length Azure Service Bus accepts for a subscription name.
    /// </summary>
    public const int MaxLength = 50;

    /// <summary>
    /// Number of hexadecimal characters of the discriminator appended to a truncated name.
    /// </summary>
    public const int HashLength = 8;

    private readonly CoreOptions coreOptions = coreOptions?.Value ?? throw new ArgumentNullException(nameof(coreOptions));
    private readonly ConcurrentDictionary<Type, string> names = new();

    /// <summary>
    /// Resolves the subscription name for the specified event handler type.
    /// </summary>
    /// <param name="eventHandlerType">The event handler type, annotated with <see cref="QueueNameAttribute"/>.</param>
    /// <returns>A subscription name of at most <see cref="MaxLength"/> valid characters.</returns>
    /// <exception cref="ServiceBusPubSubException">The handler is not annotated with <see cref="QueueNameAttribute"/>.</exception>
    public string GetSubscriptionName(Type eventHandlerType)
    {
        ArgumentNullException.ThrowIfNull(eventHandlerType);

        return this.names.GetOrAdd(eventHandlerType, this.Resolve);
    }

    /// <summary>
    /// Builds the subscription name for the specified event handler type.
    /// </summary>
    /// <param name="eventHandlerType">The event handler type.</param>
    /// <returns>The subscription name.</returns>
    private string Resolve(Type eventHandlerType)
    {
        var attribute = eventHandlerType.GetCustomAttribute<QueueNameAttribute>()
            ?? throw new ServiceBusPubSubException($"The event handler {eventHandlerType.Name} is not decorated with the QueueNameAttribute.");

        // El nombre logico es el mismo que RabbitMQ usa como nombre de cola. No se manda a Service Bus, pero es
        // lo que se resume cuando hay que acortar: lleva negocio, version y entidad, que es justo lo que se
        // omite del nombre corto porque el topic ya lo dice.
        var logicalName = attribute.GetQueueName(this.coreOptions.AppName, this.coreOptions.Business, this.coreOptions.Version);

        var candidate = Sanitize($"{this.coreOptions.AppName}.{attribute.Action}");

        if (candidate.Length <= MaxLength)
            return candidate;

        var prefix = candidate[..(MaxLength - HashLength - 1)].TrimEnd('.', '-', '_');

        return $"{prefix}-{Discriminator(logicalName)}";
    }

    /// <summary>
    /// Removes every character Azure Service Bus does not accept in a subscription name.
    /// </summary>
    /// <param name="value">The raw name.</param>
    /// <returns>A name made of lowercase alphanumerics, periods, hyphens and underscores.</returns>
    /// <remarks>
    /// Un nombre debe empezar y terminar en caracter alfanumerico, de ahi el recorte de los extremos.
    /// </remarks>
    public static string Sanitize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var builder = new StringBuilder(value.Length);

        foreach (var character in value.ToLowerInvariant())
        {
            if (char.IsAsciiLetterOrDigit(character) || character is '.' or '-' or '_')
                builder.Append(character);
            else
                builder.Append('-');
        }

        return builder.ToString().Trim('.', '-', '_');
    }

    /// <summary>
    /// Builds the deterministic discriminator appended to a name that had to be truncated.
    /// </summary>
    /// <param name="logicalName">The full logical name of the queue owned by the handler.</param>
    /// <returns>The first <see cref="HashLength"/> hexadecimal characters of its SHA-256 digest.</returns>
    /// <remarks>
    /// Se calcula sobre el nombre completo, no sobre el truncado, para que dos handlers cuyo prefijo coincide
    /// tras el recorte sigan resolviendo a suscripciones distintas.
    /// </remarks>
    public static string Discriminator(string logicalName)
    {
        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(logicalName));

        return Convert.ToHexString(digest)[..HashLength].ToLowerInvariant();
    }
}
