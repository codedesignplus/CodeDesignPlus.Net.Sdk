

namespace CodeDesignPlus.Net.Event.Sourcing.Extensions;

/// <summary>
/// Class that contains the extension methods for the event sourcing service.
/// </summary>
public static class EventSourcingExtesions
{
    /// <summary>
    /// Get the type of the event sourcing service.
    /// </summary>
    /// <returns>The type of the event sourcing service.</returns>
    public static Type GetEventSourcing()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .FirstOrDefault(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventSourcingService)));
    }
}
