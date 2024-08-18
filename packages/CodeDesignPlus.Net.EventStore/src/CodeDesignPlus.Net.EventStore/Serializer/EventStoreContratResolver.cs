namespace CodeDesignPlus.Net.EventStore.Serializer;

/// <summary>
/// Custom contract resolver for EventStore that extends the EventContractResolver.
/// </summary>
public class EventStoreContratResolver : EventContractResolver
{
    /// <summary>
    /// Creates a JsonObjectContract for the given type.
    /// </summary>
    /// <param name="objectType">The type of the object.</param>
    /// <returns>A JsonObjectContract for the given type.</returns>
    protected override Newtonsoft.Json.Serialization.JsonObjectContract CreateObjectContract(Type objectType)
    {
        var contract = base.CreateObjectContract(objectType);

        if (!typeof(Net.Event.Sourcing.Abstractions.AggregateRoot).IsAssignableFrom(objectType))
            return contract;

        var constructor = objectType.GetConstructor([typeof(Guid)]);

        contract.DefaultCreator = () => constructor.Invoke([Guid.Empty]);

        return contract;
    }
}