using CodeDesignPlus.Net.PubSub.Abstractions;

namespace CodeDesignPlus.Net.Redis.PubSub.Test.Helpers.Events
{
    public class UserCreatedEvent : EventBase
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? Names { get; set; }
        public string? Lastnames { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
