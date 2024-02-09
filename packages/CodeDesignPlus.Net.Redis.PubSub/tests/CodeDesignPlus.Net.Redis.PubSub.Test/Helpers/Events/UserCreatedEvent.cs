using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.PubSub.Abstractions;

namespace CodeDesignPlus.Net.Redis.PubSub.Test.Helpers.Events
{
    [Key("user.create.event")]
    public class UserCreatedEvent : DomainEvent
    {
        public UserCreatedEvent(
            Guid aggregateId,
            Guid? eventId = null,
            DateTime? occurredAt = null,
            Dictionary<string, object> metadata = null
        ) : base(aggregateId, eventId, occurredAt, metadata)
        {
        }

        public string? UserName { get; set; }
        public string? Names { get; set; }
        public string? Lastnames { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
