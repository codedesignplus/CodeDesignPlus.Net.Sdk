using CodeDesignPlus.Net.Serializers;
using Newtonsoft.Json.Serialization;

namespace CodeDesignPlus.Net.EventStore.Serializer;

/// <summary>
/// Custom JSON contract resolver for configuring serialization of domain events.
/// </summary>
public class EventStoreContratResolver : EventContractResolver
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreContratResolver"/> class.
    /// </summary>
    public EventStoreContratResolver() : base()
    {
    }

    /// <summary>
    /// Creates a custom JSON object contract for the specified type.
    /// </summary>
    /// <param name="objectType">The type for which to create the JSON object contract.</param>
    /// <returns>A <see cref="JsonObjectContract"/> for the specified type.</returns>
    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
        var contract = base.CreateObjectContract(objectType);

        if (!typeof(Net.Event.Sourcing.Abstractions.AggregateRoot).IsAssignableFrom(objectType))
            return contract;

        var constructor = objectType.GetConstructor([typeof(Guid)]);

        contract.DefaultCreator = () => constructor.Invoke([Guid.Empty]);

        return contract;
    }
}
