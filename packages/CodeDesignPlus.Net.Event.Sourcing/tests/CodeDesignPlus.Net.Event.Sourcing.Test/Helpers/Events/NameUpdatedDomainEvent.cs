using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Helpers.Events;


public class NameUpdatedDomainEvent(
    Guid aggregateId, 
    string name,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; set; } = name;

    public override string GetEventType()
    {
        return "order.created";
    }
}
