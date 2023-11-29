namespace CodeDesignPlus.Net.Event.Sourcing.Test;

public class OrderAggregateRoot : AggregateRootBase<Guid>
{
    public string Name { get; set; }
    public override string Category { get; protected set; } = "Order";
    public OrderAggregateRoot()
    {
    }

    public OrderAggregateRoot(string name, Guid idUser)
    {
        this.OrderCreated(name, idUser);
    }

    public OrderAggregateRoot(Guid id, string name, Guid idUser)
    {
        Id = id;
        this.OrderCreated(name, idUser);
    }

    public void OrderCreated(string name, Guid idUser)
    {
        this.ApplyChange(new OrderCreatedEvent(base.Id, name), idUser);
    }

    private void Apply(OrderCreatedEvent @event, Metadata<Guid> metadata)
    {
        this.Id = metadata.AggregateId;
        this.Name = @event.Name;
    }

}
