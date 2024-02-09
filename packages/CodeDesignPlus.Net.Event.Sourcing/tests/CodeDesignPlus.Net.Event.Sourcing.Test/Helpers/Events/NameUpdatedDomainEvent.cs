using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Helpers.Events;

[Key("name.updated")]
public class NameUpdatedDomainEvent(
    Guid aggregateId, 
    string name,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; set; } = name;
}
