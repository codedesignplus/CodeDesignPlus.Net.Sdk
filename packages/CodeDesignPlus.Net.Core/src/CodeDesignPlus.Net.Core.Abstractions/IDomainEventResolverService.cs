namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Service in charge of resolving the type of domain event.
/// </summary>
public interface IDomainEventResolverService
{
    /// <summary>
    /// Get the type of domain event from the name.
    /// </summary>
    /// <param name="eventName">The name of the domain event.</param>
    /// <returns>The type of domain event.</returns>
    Type GetDomainEventType(string eventName);
    /// <summary>
    /// Get the type of domain event from the name.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of domain event.</typeparam>
    /// <returns>The type of domain event.</returns>
    Type GetDomainEventType<TDomainEvent>() where TDomainEvent : IDomainEvent;
    /// <summary>
    /// Get the type of the attribute that represents the key of the domain event.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of domain event.</typeparam>
    /// <returns>The type of the attribute that represents the key of the domain event.</returns>
    string GetKeyDomainEvent<TDomainEvent>() where TDomainEvent : IDomainEvent;
    /// <summary>
    /// Get the type of the attribute that represents the key of the domain event.
    /// </summary>
    /// <param name="type">The type of domain event.</param>
    /// <returns>The type of the attribute that represents the key of the domain event.</returns>
    string GetKeyDomainEvent(Type type);
}
