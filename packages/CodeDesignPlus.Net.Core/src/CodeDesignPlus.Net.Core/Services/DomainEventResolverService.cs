using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Core.Services;

/// <summary>
/// Service to resolve domain events.
/// </summary>
public class DomainEventResolverService : IDomainEventResolverService
{
    private readonly Dictionary<string, Type> eventTypes = [];
    private readonly CoreOptions coreOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventResolverService"/> class.
    /// </summary>
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
    public string GetKeyEvent<TDomainEvent>() where TDomainEvent : IDomainEvent => this.GetKeyEvent(typeof(TDomainEvent));

    /// <summary>
    /// Gets the key attribute for the specified type.
    /// </summary>
    /// <param name="type">The type of the domain event.</param>
    /// <returns>The key attribute of the domain event.</returns>
    /// <exception cref="CoreException">Thrown when the event does not have the <see cref="EventKeyAttribute"/>.</exception>
    public string GetKeyEvent(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var attribute = type.GetCustomAttribute<EventKeyAttribute>();

        if (attribute is null)
            throw new CoreException($"The event {type.Name} does not have the KeyAttribute");

        return $"{coreOptions.Business}.{coreOptions.AppName}.{attribute.Version}.{attribute.Entity}.{attribute.Event}".ToLower();
    }

}
