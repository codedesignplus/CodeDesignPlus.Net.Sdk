namespace CodeDesignPlus.Net.Core.Abstractions;


public interface IDomainEvent
{
    Guid AggregateId { get; }
}

// /// <summary>
// /// Represents a lightweight domain event. Lightweight events, often referred to as thin events, 
// /// typically carry the minimal information necessary to describe a change in the system. 
// /// They often contain only the identifiers or values that have changed, without the full context.
// /// </summary>
// /// <typeparam name="TMetadata">The type of metadata associated with the event.</typeparam>
// public interface IThinDomainEvent<TUserKey> : IDomainEvent<TUserKey>
// {

// }

// /// <summary>
// /// Represents a lightweight domain event. Lightweight events, often referred to as thin events, 
// /// typically carry the minimal information necessary to describe a change in the system. 
// /// They often contain only the identifiers or values that have changed, without the full context.
// /// </summary>
// /// <typeparam name="TMetadata">The type of metadata associated with the event.</typeparam>
// public interface IThinDomainEvent<TUserKey, TMetadata> : IDomainEvent<TUserKey, TMetadata>, IThinDomainEvent<TUserKey>
//     where TMetadata : class
// {

// }

// /// <summary>
// /// Represents a rich domain event. Rich events carry more information than thin events, 
// /// providing a greater context about a change. This includes the state of the aggregate 
// /// before and after the event occurred.
// /// </summary>
// /// <typeparam name="TAggregate">The type of the aggregate root associated with this event.</typeparam>
// /// <typeparam name="TMetadata">The type of metadata associated with the event.</typeparam>
// public interface IRichDomainEvent<TKey, TUserKey, TAggregate> : IDomainEvent<TUserKey>
//     where TAggregate : IAggregateRoot<TKey>
// {
//     /// <summary>
//     /// Gets the state of the aggregate before the event occurred.
//     /// </summary>
//     TAggregate PreviousState { get; }

//     /// <summary>
//     /// Gets the state of the aggregate after the event occurred.
//     /// </summary>
//     TAggregate CurrentState { get; }
// }

// /// <summary>
// /// Represents a rich domain event. Rich events carry more information than thin events, 
// /// providing a greater context about a change. This includes the state of the aggregate 
// /// before and after the event occurred.
// /// </summary>
// /// <typeparam name="TAggregate">The type of the aggregate root associated with this event.</typeparam>
// /// <typeparam name="TMetadata">The type of metadata associated with the event.</typeparam>
// public interface IRichDomainEvent<TKey, TUserKey, TAggregate, TMetadata> : IDomainEvent<TUserKey, TMetadata>, IRichDomainEvent<TKey, TUserKey, TAggregate>
//     where TAggregate : IAggregateRoot<TKey>
//     where TMetadata : class
// {

// }