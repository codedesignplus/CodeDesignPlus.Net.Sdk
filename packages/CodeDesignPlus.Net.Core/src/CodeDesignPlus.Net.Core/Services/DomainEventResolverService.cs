using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using System.Reflection;

namespace CodeDesignPlus.Net.Core.Services;


public class DomainEventResolverService : IDomainEventResolverService
{
    private readonly Dictionary<string, Type> _eventTypes = [];

    public DomainEventResolverService()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(x => typeof(DomainEvent).IsAssignableFrom(x) && !x.IsAbstract);

        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<KeyAttribute>();

            if (attribute is null)
                throw new InvalidOperationException($"The event {type.Name} does not have the KeyAttribute");

            _eventTypes.Add(attribute.Key, type);
        }
    }

    public Type GetDomainEventType(string eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName))
            throw new ArgumentNullException(nameof(eventName));

        if (!_eventTypes.TryGetValue(eventName, out var type))
            throw new ArgumentException($"The event type {eventName} does not exist");

        return type;
    }

    public Type GetDomainEventType<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        return GetDomainEventType(GetKeyEvent<TDomainEvent>());
    }

    public string GetKeyDomainEvent<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        return GetKeyEvent<TDomainEvent>();
    }


    public string GetKeyDomainEvent(Type type)
    {
        return GetKeyEvent(type);
    }

    public static string GetKeyEvent<TDomainEvent>() where TDomainEvent : IDomainEvent
    {
        var attribute = typeof(TDomainEvent).GetCustomAttribute<KeyAttribute>();

        if (attribute is null)
            throw new InvalidOperationException($"The event {typeof(TDomainEvent).Name} does not have the KeyAttribute");

        return attribute.Key;
    }

    public static string GetKeyEvent(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var attribute = type.GetCustomAttribute<KeyAttribute>();

        if (attribute is null)
            throw new InvalidOperationException($"The event {type.Name} does not have the KeyAttribute");

        return attribute.Key;
    }

}
