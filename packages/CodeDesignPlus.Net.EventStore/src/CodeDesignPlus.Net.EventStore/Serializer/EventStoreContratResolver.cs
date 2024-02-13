using CodeDesignPlus.Net.Serializers;
using Newtonsoft.Json.Serialization;

namespace CodeDesignPlus.Net.EventStore.Serializer;

public class EventStoreContratResolver : EventContractResolver
{

    public EventStoreContratResolver() : base()
    {
    }

    public EventStoreContratResolver(string[] converters) : base(converters)
    {
    }

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
