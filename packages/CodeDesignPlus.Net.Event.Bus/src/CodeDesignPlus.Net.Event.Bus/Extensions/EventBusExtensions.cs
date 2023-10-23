using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Event.Bus.Extensions;

public static class EventBusExtensions
{

    /// <summary>
    /// Determines whether an instance of a specified type can be assigned to a variable of the current type.
    /// </summary>
    /// <param name="type">Current type.</param>
    /// <param name="interface">The type to compare with the current type.</param>
    /// <returns>Return true if type implemented <paramref name="interface"/></returns>
    public static bool IsAssignableGenericFrom(this Type type, Type @interface)
    {
        return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == @interface);
    }

    /// <summary>
    /// Escanea y retorna las clase que implementan la interfaz <see cref="IEventHandler{TEvent}"/>
    /// </summary>
    /// <typeparam name="TStartupLogic">Clase de inicio que implementa la interfaz <see cref="IStartupServices"/></typeparam>
    /// <returns>Return a list of type</returns>
    public static List<Type> GetEventHandlers()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x =>
                x.IsClass &&
                x.IsAssignableGenericFrom(typeof(IEventHandler<>))
            ).ToList();
    }

    /// <summary>
    /// Retrieves the first interface of a given type that is a generic type instance of the IEventHandler<> interface.
    /// </summary>
    /// <param name="eventHandler">The type from which to retrieve the interface.</param>
    /// <returns>
    /// The first matching interface of type IEventHandler<>, or null if no such interface is found.
    /// </returns>
    /// <example>
    /// <code>
    /// var myEventHandlerType = typeof(MyEventHandler);
    /// var interfaceType = myEventHandlerType.GetInterfaceEventHandlerGeneric();
    /// Console.WriteLine(interfaceType);  // Outputs something like "IEventHandler<MyEvent>"
    /// </code>
    /// </example>
    public static Type GetInterfaceEventHandlerGeneric(this Type eventHandler)
    {
        return eventHandler.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));
    }

    /// <summary>
    /// Given an interface of type IEventHandler<>, retrieves the event type it handles.
    /// </summary>
    /// <param name="interfaceEventHandlerGeneric">The IEventHandler<> type interface from which to retrieve the event type.</param>
    /// <returns>
    /// The event type handled by the interface, or null if not found.
    /// </returns>
    /// <remarks>
    /// This method assumes that the provided type is an instance of the generic interface IEventHandler<>. 
    /// If this is not the case, the method may return unexpected results.
    /// </remarks>
    /// <example>
    /// <code>
    /// var myInterfaceType = typeof(IEventHandler<MyEvent>);
    /// var eventType = myInterfaceType.GetEventType();
    /// Console.WriteLine(eventType);  // Outputs "MyEvent"
    /// </code>
    /// </example>
    public static Type GetEventType(this Type interfaceEventHandlerGeneric)
    {
        return interfaceEventHandlerGeneric.GetGenericArguments().FirstOrDefault(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(EventBase)));
    }
}
