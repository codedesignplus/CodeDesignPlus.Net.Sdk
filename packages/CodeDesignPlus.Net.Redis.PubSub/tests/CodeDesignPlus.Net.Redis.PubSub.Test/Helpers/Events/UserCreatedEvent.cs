using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.Redis.PubSub.Test.Helpers.Events
{
    [EventKey("UserEntity", 1, "created")]
    public class UserCreatedEvent(
        Guid aggregateId,
        Guid? eventId = null,
        DateTime? occurredAt = null,
        Dictionary<string, object>? metadata = null
        ) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
    {
        public string? UserName { get; set; }
        public string? Names { get; set; }
        public string? Lastnames { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
