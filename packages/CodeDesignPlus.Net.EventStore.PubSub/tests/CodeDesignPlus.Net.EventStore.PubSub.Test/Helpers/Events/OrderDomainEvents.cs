using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributees;
using CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Domain;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Events;

[EventKey<OrderAggregateRoot>(1, "created")]
public class OrderCreatedEvent(
    Guid aggregateId,
    Guid idUserCreator,
    OrderStatus orderStatus,
    Client client,
    DateTime dateCreated,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public Client Client { get; } = client;
    public OrderStatus OrderStatus { get; } = orderStatus;
    public Guid IdUserCreator { get; } = idUserCreator;
    public DateTime DateCreated { get; } = dateCreated;
}

[EventKey<OrderAggregateRoot>(1, "updated")]
public class OrderCompletedEvent(
    Guid aggregateId,
    DateTime completionDate,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
    ) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public DateTime CompletionDate { get; } = completionDate;
}

[EventKey<OrderAggregateRoot>(1, "cancelled")]
public class OrderCancelledEvent(
    Guid aggregateId,
    DateTime cancellationDate,
    string reason,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public DateTime CancellationDate { get; } = cancellationDate;
    public string Reason { get; } = reason;
}