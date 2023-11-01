using CodeDesignPlus.Net.Event.Bus.Abstractions;

namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions;

public abstract class DomainEventBase : EventBase, IDomainEvent
{
    public Guid AggregateId { get; private set; }
    public string EventType => GetType().Name;

    public DomainEventBase(Guid aggregateId)
    {
        this.AggregateId = aggregateId;
    }
}

// /// <summary>
// /// Provides a base implementation for lightweight domain events.
// /// </summary>
// /// <typeparam name="TMetadata">The type of metadata associated with the event.</typeparam>
// public abstract class ThinDomainEventBase<TUserKey, TMetadata> : DomainEventBase<TUserKey, TMetadata>, IThinDomainEvent<TUserKey, TMetadata>
//     where TMetadata : class
// {
//     /// <summary>
//     /// Initializes a new instance of the <see cref="ThinDomainEventBase{TMetadata}"/> class with the specified aggregate ID and metadata.
//     /// </summary>
//     /// <param name="aggregateId">The unique identifier of the aggregate root associated with this lightweight domain event.</param>
//     /// <param name="metadata">The metadata associated with the lightweight domain event.</param>
//     protected ThinDomainEventBase(Guid eventId, Guid aggregateId, long version, DateTime occurredOn, TUserKey idUser, TMetadata metadata)
//         : base(eventId, aggregateId, version, occurredOn, idUser, metadata)
//     {
//     }
// }

// /// <summary>
// /// Provides a base implementation for rich domain events.
// /// </summary>
// /// <typeparam name="TAggregate">The type of the aggregate root associated with this event.</typeparam>
// /// <typeparam name="TMetadata">The type of metadata associated with the event.</typeparam>
// public abstract class RichDomainEventBase<TKey, TUserKey, TAggregate, TMetadata> : DomainEventBase<TUserKey, TMetadata>, IRichDomainEvent<TKey, TUserKey, TAggregate, TMetadata>
//     where TAggregate : IAggregateRoot<TKey>
//     where TMetadata : class
// {
//     /// <summary>
//     /// Represents the state of the aggregate before the event occurred. 
//     /// This provides context about the changes that led to this event.
//     /// </summary>
//     public TAggregate PreviousState { get; protected set; }

//     /// <summary>
//     /// Represents the state of the aggregate after the event occurred.
//     /// This reflects the changes that were made due to this event.
//     /// </summary>
//     public TAggregate CurrentState { get; protected set; }

//     /// <summary>
//     /// Initializes a new instance of the <see cref="RichDomainEventBase{TAggregate, TMetadata}"/> class 
//     /// with the specified aggregate ID, metadata, previous state, and current state.
//     /// </summary>
//     /// <param name="aggregateId">The unique identifier of the aggregate root associated with this rich domain event.</param>
//     /// <param name="metadata">The metadata associated with the rich domain event.</param>
//     /// <param name="previousState">The state of the aggregate before the event occurred.</param>
//     /// <param name="currentState">The state of the aggregate after the event occurred.</param>
//     protected RichDomainEventBase(Guid eventId, Guid aggregateId, long version, DateTime occurredOn, TUserKey idUser, TMetadata metadata, TAggregate previousState, TAggregate currentState)
//         : base(eventId, aggregateId, version, occurredOn, idUser, metadata)
//     {
//         this.PreviousState = previousState;
//         this.CurrentState = currentState;
//     }
// }