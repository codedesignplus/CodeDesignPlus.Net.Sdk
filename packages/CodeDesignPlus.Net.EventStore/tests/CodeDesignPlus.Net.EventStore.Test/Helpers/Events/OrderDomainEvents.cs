using Newtonsoft.Json;
using CodeDesignPlus.Net.Event.Sourcing.Abstractions;
using CodeDesignPlus.Net.EventStore.Test.Helpers.Domain;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers.Events;

public class OrderCreatedEvent : DomainEventBase
{
    public OrderCreatedEvent(Guid aggregateId, Guid idUserCreator, OrderStatus orderStatus, Domain.Client client, DateTime dateCreated)
        : base(aggregateId)
    {
        this.DateCreated = dateCreated;
        this.Client = client;
        this.OrderStatus = orderStatus;
        this.IdUserCreator = idUserCreator;
    }

    public Domain.Client Client { get; }
    public OrderStatus OrderStatus { get; }
    public Guid IdUserCreator { get; }
    public DateTime DateCreated { get; }
}

public class OrderCompletedEvent : DomainEventBase
{
    public DateTime CompletionDate { get; }

    public OrderCompletedEvent(Guid aggregateId, DateTime completionDate)
      : base(aggregateId)
    {
        this.CompletionDate = completionDate;
    }
}

public class OrderCancelledEvent : DomainEventBase
{
    public DateTime CancellationDate { get; }
    public string Reason { get; }

    public OrderCancelledEvent(Guid aggregateId,  DateTime cancellationDate, string reason)
        : base(aggregateId)
    {
        this.CancellationDate = cancellationDate;
        this.Reason = reason;
    }
}