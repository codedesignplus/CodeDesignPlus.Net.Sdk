using CodeDesignPlus.Net.Event.Bus.Abstractions;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers.Events
{
    [Topic("user-topic")]
    public class UserCreatedEvent : EventBase
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? Names { get; set; }
        public string? Lastnames { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
