using CodeDesignPlus.Net.Event.Bus.Abstractions;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Redis.Event.Bus.Test.Helpers.Events
{
    public class UserCreatedEvent : EventBase
    {
        public UserCreatedEvent() : base()
        {

        }

        [JsonConstructor]
        public UserCreatedEvent([JsonProperty("IdEvent")] Guid idEvent, [JsonProperty("EventDate")] DateTime eventDate)
            : base(idEvent, eventDate)
        {

        }

        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? Names { get; set; }
        public string? Lastnames { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
