

namespace CodeDesignPlus.Net.Event.Sourcing.Extensions;

public static class EventSourcingExtesions
{
    public static Type GetEventSourcing()
    {
        return AppDomain.CurrentDomain
           .GetAssemblies()
           .SelectMany(assembly => assembly.GetTypes())
           .FirstOrDefault(t => typeof(IEventSourcingService).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
    }
}
