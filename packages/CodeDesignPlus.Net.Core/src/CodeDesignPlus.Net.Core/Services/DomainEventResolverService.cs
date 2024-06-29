namespace CodeDesignPlus.Net.Core.Services;

/// <summary>
/// Service to resolve domain events.
/// </summary>
public class DomainEventResolverService : IDomainEventResolverService
{
    private readonly Dictionary<string, Type> eventTypes = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventResolverService"/> class.
    /// </summary>
    public DomainEventResolverService()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(x => typeof(DomainEvent).IsAssignableFrom(x) && !x.IsAbstract);

        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<EventKeyAttribute>();

            if (attribute is not null)
                eventTypes.Add(attribute.Key, type);
        }
    }

    /// <summary>
    /// Gets the domain event type by the event name.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <returns>The type of the domain event.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="eventName"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the event type does not exist.</exception>
    public Type GetDomainEventType(string eventName)
    {
        ArgumentNullException.ThrowIfNull(eventName);

        if (!eventTypes.TryGetValue(eventName, out var type))
            throw new CoreException($"The event type {eventName} does not exist");

        return type;
    }

    /// <summary>
    /// Gets the domain event type for the specified domain event.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <returns>The type of the domain event.</returns>
    public Type GetDomainEventType<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        return GetDomainEventType(GetKeyEvent<TDomainEvent>());
    }

    /// <summary>
    /// Gets the key for the specified domain event type.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <returns>The key of the domain event.</returns>
    public string GetKeyDomainEvent<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        return GetKeyEvent<TDomainEvent>();
    }

    /// <summary>
    /// Gets the key for the specified type.
    /// </summary>
    /// <param name="type">The type of the domain event.</param>
    /// <returns>The key of the domain event.</returns>
    public string GetKeyDomainEvent(Type type)
    {
        return GetKeyEvent(type);
    }

    /// <summary>
    /// Gets the key attribute for the specified domain event type.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <returns>The key attribute of the domain event.</returns>
    /// <exception cref="CoreException">Thrown when the event does not have the <see cref="EventKeyAttribute"/>.</exception>
    public static string GetKeyEvent<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        var attribute = typeof(TDomainEvent).GetCustomAttribute<EventKeyAttribute>();

        if (attribute is null)
            throw new CoreException($"The event {typeof(TDomainEvent).Name} does not have the KeyAttribute");

        return attribute.Key;
    }

    /// <summary>
    /// Gets the key attribute for the specified type.
    /// </summary>
    /// <param name="type">The type of the domain event.</param>
    /// <returns>The key attribute of the domain event.</returns>
    /// <exception cref="CoreException">Thrown when the event does not have the <see cref="EventKeyAttribute"/>.</exception>
    public static string GetKeyEvent(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var attribute = type.GetCustomAttribute<EventKeyAttribute>();

        if (attribute is null)
            throw new CoreException($"The event {type.Name} does not have the KeyAttribute");

        return attribute.Key;
    }

}
