namespace CodeDesignPlus.Net.Core;

public class EventBinder
{
    private readonly Dictionary<Type, string> eventTypes = [];

    public void AddEventType<TEvent>(string eventType)
        where TEvent : IDomainEvent
    {
        if (!this.eventTypes.ContainsKey(typeof(TEvent)))
            this.eventTypes.Add(typeof(TEvent), eventType);
    }
}
