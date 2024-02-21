using CodeDesignPlus.Net.EventStore.Test.Helpers.Domain;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using StackExchange.Redis;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers.Events;

[Key("order.created")]
public class OrderCreatedEvent(
    Guid aggregateId,
    Guid idUserCreator,
    OrderStatus orderStatus,
    Domain.Client client,
    DateTime dateCreated,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public Domain.Client Client { get; } = client;
    public OrderStatus OrderStatus { get; } = orderStatus;
    public Guid IdUserCreator { get; } = idUserCreator;
    public DateTime DateCreated { get; } = dateCreated;
}

[Key("name.updated")]
public class OrderCompletedEvent(
    Guid aggregateId,
    DateTime completionDate,
    Guid? eventId = null,
    DateTime? occurredAt = null,
    Dictionary<string, object>? metadata = null
    ) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public DateTime CompletionDate { get; } = completionDate;
    public OrderStatus OrderStatus { get; } = OrderStatus.Completed;
}

[Key("order.cancelled")]
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