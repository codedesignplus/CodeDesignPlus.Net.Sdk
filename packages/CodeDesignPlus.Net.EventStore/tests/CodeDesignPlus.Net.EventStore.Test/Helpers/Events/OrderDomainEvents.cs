using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers.Events;

public class OrderCreatedEvent : ThinDomainEventBase<OrderMetadata>
{
    public DateTime DateCreated { get; }
    public Guid ClientId { get; }

    public OrderCreatedEvent(Guid orderId, DateTime dateCreated, Guid clientId, OrderMetadata metadata)
        : base(orderId, metadata)
    {
        DateCreated = dateCreated;
        ClientId = clientId;
    }
}

public class OrderUpdatedEvent : ThinDomainEventBase<OrderMetadata>
{
    public DateTime UpdatedDate { get; }

    public OrderUpdatedEvent(Guid orderId, DateTime updatedDate, OrderMetadata metadata)
        : base(orderId, metadata)
    {
        UpdatedDate = updatedDate;
    }
}

public class OrderCompletedEvent : ThinDomainEventBase<OrderMetadata>
{
    public DateTime CompletionDate { get; }

    public OrderCompletedEvent(Guid orderId, DateTime completionDate, OrderMetadata metadata)
        : base(orderId, metadata)
    {
        CompletionDate = completionDate;
    }
}

public class OrderCancelledEvent : DomainEventBase
{
    public DateTime CancellationDate { get; }
    public string Reason { get; }

    public OrderCancelledEvent(Guid orderId, DateTime cancellationDate, string reason)
        : base(orderId)
    {
        CancellationDate = cancellationDate;
        Reason = reason;
    }
}