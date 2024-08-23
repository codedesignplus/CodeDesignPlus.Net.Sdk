namespace CodeDesignPlus.Net.PubSub.Extensions;

/// <summary>
/// Provides extension methods related to the PubSub functionality.
/// </summary>
public static class PubSubExtensions
{
    /// <summary>
    /// Determines whether an instance of a specified type can be assigned to a variable of the current type.
    /// </summary>
    /// <param name="type">The current type.</param>
    /// <param name="interface">The type to compare with the current type.</param>
    /// <returns>Returns true if <paramref name="type"/> implements <paramref name="interface"/>.</returns>
    public static bool IsAssignableGenericFrom(this Type type, Type @interface)
    {
        return Array.Exists(type.GetInterfaces(), x => x.IsGenericType && x.GetGenericTypeDefinition() == @interface);
    }

    /// <summary>
    /// Retrieves all non-abstract classes that derive from <see cref="IDomainEvent"/>.
    /// </summary>
    /// <returns>A list of event types.</returns>
    public static List<Type> GetEvents()
    {
        return AppDomain.CurrentDomain
           .GetAssemblies()
           .SelectMany(assembly => assembly.GetTypes())
           .Where(t => typeof(IDomainEvent).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsInterface)
           .ToList();
    }

    /// <summary>
    /// Scans and returns classes that implement the <see cref="IEventHandler{TEvent}"/> interface.
    /// </summary>
    /// <returns>A list of event handler types.</returns>
    public static List<Type> GetEventHandlers()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(assembly => assembly.GetName().Name != "DynamicProxyGenAssembly2")
            .SelectMany(x => x.GetTypes())
            .Where(x =>
                x.IsClass &&
                x.IsAssignableGenericFrom(typeof(IEventHandler<>))
            ).ToList();
    }

    /// <summary>
    /// Retrieves the first interface of a given type that is a generic type instance of the <see cref="IEventHandler{TEvent}"/> interface.
    /// </summary>
    /// <param name="eventHandler">The type from which to retrieve the interface.</param>
    /// <returns>
    /// The first matching interface of type <see cref="IEventHandler{TEvent}"/>, or null if no such interface is found.
    /// </returns>
    public static Type GetInterfaceEventHandlerGeneric(this Type eventHandler)
    {
        return Array.Find(eventHandler.GetInterfaces(), x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));
    }

    /// <summary>
    /// Given an interface of type <see cref="IEventHandler{TEvent}"/>, retrieves the event type it handles.
    /// </summary>
    /// <param name="interfaceEventHandlerGeneric">The <see cref="IEventHandler{TEvent}"/> type interface from which to retrieve the event type.</param>
    /// <returns>
    /// The event type handled by the interface, or null if not found.
    /// </returns>
    /// <remarks>
    /// This method assumes that the provided type is an instance of the generic interface <see cref="IEventHandler{TEvent}"/>. 
    /// If this is not the case, the method may return unexpected results.
    /// </remarks>
    public static Type GetEventType(this Type interfaceEventHandlerGeneric)
    {
        return Array.Find(interfaceEventHandlerGeneric.GetGenericArguments(), x => x.IsClass && !x.IsAbstract && typeof(IDomainEvent).IsAssignableFrom(x));
    }
}