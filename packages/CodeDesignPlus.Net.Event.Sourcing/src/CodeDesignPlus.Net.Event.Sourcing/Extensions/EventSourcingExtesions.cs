

namespace CodeDesignPlus.Net.Event.Sourcing.Extensions;

public static class EventSourcingExtesions
{
    public static Type GetEventSourcing()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .FirstOrDefault(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventSourcingService<>)));
    }
}
