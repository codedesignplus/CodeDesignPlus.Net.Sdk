namespace CodeDesignPlus.Net.Core.Services;

/// <summary>
/// Service responsible for resolving domain event types based on event names and attributes.
/// </summary>
public class DomainEventResolverService : IDomainEventResolverService
{
    /// <summary>
    /// Dictionary that maps event names to their corresponding types.
    /// </summary>
    private readonly Dictionary<string, Type> eventTypes = [];

    /// <summary>
    /// The options for the core functionality of the application.
    /// </summary>
    private readonly CoreOptions coreOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventResolverService"/> class.
    /// </summary>
    /// <param name="options">The options for the core functionality of the application.</param>
    public DomainEventResolverService(IOptions<CoreOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        this.coreOptions = options.Value;

        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(x => typeof(DomainEvent).IsAssignableFrom(x) && !x.IsAbstract);

        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<EventKeyAttribute>();

            if (attribute is not null)
            {
                var key = $"{coreOptions.Business}.{coreOptions.AppName}.{attribute.Version}.{attribute.Entity}.{attribute.Event}".ToLower();

                eventTypes.Add(key, type);
            }
        }
    }

    /// <summary>
    /// Gets the type of the domain event based on the event name.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <returns>The type of the domain event.</returns>
    public Type GetDomainEventType(string eventName)
    {
        ArgumentNullException.ThrowIfNull(eventName);

        if (!eventTypes.TryGetValue(eventName, out var type))
            throw new CoreException($"The event type {eventName} does not exist");

        return type;
    }

    /// <summary>
    /// Gets the type of the domain event based on the generic type parameter.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <returns>The type of the domain event.</returns>
    public Type GetDomainEventType<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        return GetDomainEventType(GetKeyDomainEvent<TDomainEvent>());
    }

    /// <summary>
    /// Gets the key of the domain event based on the generic type parameter.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <returns>The key of the domain event.</returns>
    public string GetKeyDomainEvent<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        return this.GetKeyDomainEvent(typeof(TDomainEvent));
    }

    /// <summary>
    /// Gets the key of the domain event based on the type.
    /// </summary>
    /// <param name="type">The type of the domain event.</param>
    /// <exception cref="CoreException">The event does not have the KeyAttribute.</exception>
    /// <returns>The key of the domain event.</returns>
    public string GetKeyDomainEvent(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var attribute = type.GetCustomAttribute<EventKeyAttribute>();

        if (attribute is null)
            throw new CoreException($"The event {type.Name} does not have the KeyAttribute");

        return $"{coreOptions.Business}.{coreOptions.AppName}.{attribute.Version}.{attribute.Entity}.{attribute.Event}".ToLower();
    }
}
