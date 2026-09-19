namespace CodeDesignPlus.Net.ServiceBus.Abstractions;

/// <summary>
/// Translates the logical queue name owned by an event handler into a valid Azure Service Bus subscription name.
/// </summary>
/// <remarks>
/// Un topic admite 260 caracteres pero una suscripcion solo 50, y casi todos los nombres logicos del sistema
/// los superan. La traduccion tiene que ser determinista: el mismo handler debe resolver siempre al mismo
/// nombre, o cada despliegue crearia una suscripcion nueva y abandonaria la anterior con sus mensajes dentro.
/// </remarks>
public interface ISubscriptionNameResolver
{
    /// <summary>
    /// Resolves the subscription name for the specified event handler type.
    /// </summary>
    /// <param name="eventHandlerType">The event handler type, annotated with <c>QueueNameAttribute</c>.</param>
    /// <returns>A subscription name of at most 50 valid characters.</returns>
    string GetSubscriptionName(Type eventHandlerType);
}
