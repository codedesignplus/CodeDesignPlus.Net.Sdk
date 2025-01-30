﻿using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Domain;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Events;

[EventKey<OrderAggregateRoot>(1, "created")]
public class OrderCreatedEvent(
    Guid aggregateId,
    Guid idUserCreator,
    OrderStatus orderStatus,
    Client client,
    Instant dateCreated,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public Client Client { get; } = client;
    public OrderStatus OrderStatus { get; } = orderStatus;
    public Guid IdUserCreator { get; } = idUserCreator;
    public Instant DateCreated { get; } = dateCreated;
}

[EventKey<OrderAggregateRoot>(1, "updated")]
public class OrderCompletedEvent(
    Guid aggregateId,
    Instant completionDate,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
    ) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public Instant CompletionDate { get; } = completionDate;
}

[EventKey<OrderAggregateRoot>(1, "cancelled")]
public class OrderCancelledEvent(
    Guid aggregateId,
    Instant cancellationDate,
    string reason,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public Instant CancellationDate { get; } = cancellationDate;
    public string Reason { get; } = reason;
}