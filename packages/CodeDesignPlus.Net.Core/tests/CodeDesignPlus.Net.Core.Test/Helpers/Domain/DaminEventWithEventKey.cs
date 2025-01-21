
namespace CodeDesignPlus.Net.Core.Test.Helpers.Domain;

public class DaminEventWithEventKey(Guid aggregateId, Guid? eventId = null, Instant? occurredAt = null, Dictionary<string, object>? metadata = null)
    : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
}
