namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents a service for resolving domain event types and keys.
/// </summary>
public interface IDomainEventResolverService
{
    /// <summary>
    /// Gets the type of the domain event based on the event name.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <returns>The type of the domain event.</returns>
    Type GetDomainEventType(string eventName);

    /// <summary>
    /// Gets the type of the domain event based on the generic type parameter.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <returns>The type of the domain event.</returns>
    Type GetDomainEventType<TDomainEvent>() where TDomainEvent : IDomainEvent;

    /// <summary>
    /// Gets the key of the domain event based on the generic type parameter.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <returns>The key of the domain event.</returns>
    string GetKeyDomainEvent<TDomainEvent>() where TDomainEvent : IDomainEvent;

    /// <summary>
    /// Gets the key of the domain event based on the type parameter.
    /// </summary>
    /// <param name="type">The type of the domain event.</param>
    /// <returns>The key of the domain event.</returns>
    string GetKeyDomainEvent(Type type);
}
