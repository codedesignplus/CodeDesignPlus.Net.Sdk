
namespace CodeDesignPlus.Net.Event.Sourcing.Test;

public class OrderCreatedEvent : DomainEventBase
{
    public string Name { get; set; }
    public OrderCreatedEvent(Guid aggregateId, string name) : base(aggregateId)
    {
        this.Name = name;
    }
}
