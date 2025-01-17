using System;
using CodeDesignPlus.Net.Event.Sourcing.Abstractions;
using CodeDesignPlus.Net.EventStore.PubSub.Consumer.Sample.Events;

namespace CodeDesignPlus.Net.EventStore.PubSub.Consumer.Sample.Aggregates;


public class OrderAggregate : AggregateRoot
{
    public string? Name { get; private set; }
    public Guid IdUser { get; private set; }
    public List<string> Products { get; private set; } = [];
    public override string Category { get; protected set; } = "Order";

    public OrderAggregate(Guid id) : base(id) { }

    private OrderAggregate(Guid id, string name, Guid idUser) : base(id)
    {
        this.Name = name;
        this.IdUser = idUser;
    }

    public static OrderAggregate Create(Guid id, string name, Guid idUser)
    {
        var aggregate = new OrderAggregate(id, name, idUser);

        aggregate.AddEvent(new OrderCreatedDomainEvent(id, name, idUser));

        return aggregate;
    }

    public void UpdateName(string name)
    {
        this.AddEvent(new NameUpdatedDomainEvent(this.Id, name, this.IdUser));
    }

    public void AddProduct(string product)
    {
        this.AddEvent(new ProductAddedDomainEvent(this.Id, product));
    }


    private void Apply(OrderCreatedDomainEvent @event)
    {
        this.Name = @event.Name;
        this.IdUser = @event.IdUser;
    }

    private void Apply(NameUpdatedDomainEvent @event)
    {
        this.Name = @event.Name;
    }

    private void Apply(ProductAddedDomainEvent @event)
    {
        this.Products.Add(@event.Product);
    }

}
