namespace CodeDesignPlus.Net.Core.Abstractions;

public class Event(IDomainEvent domainEvent, EventMetadata metadata = null)
{
    public EventData Data { get; private set; } = new(domainEvent);
    public EventMetadata Metadata { get; private set; } = metadata;
}

public class EventData(IDomainEvent domainEvent)
{
    public Guid Id { get; private set; } = domainEvent.EventId;
    public string Type { get; private set; } = domainEvent.EventType;
    public IDomainEvent DomainEvent { get; private set; } = domainEvent;
    public DateTime OccurredAt { get; private set; } = domainEvent.OccurredAt;
}

public class EventMetadata(string key, object value)
{
    public string Key { get; private set; } = key;
    public object Value { get; private set; } = value;
}
