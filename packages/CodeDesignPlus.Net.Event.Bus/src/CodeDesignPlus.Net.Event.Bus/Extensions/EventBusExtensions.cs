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
}
