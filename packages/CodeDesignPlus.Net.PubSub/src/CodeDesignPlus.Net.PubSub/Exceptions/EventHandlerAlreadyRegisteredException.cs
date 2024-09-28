namespace CodeDesignPlus.Net.PubSub.Exceptions;

/// <summary>
/// Exception thrown when an event handler is already registered in the subscription manager.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
/// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
public class EventHandlerAlreadyRegisteredException<TEvent, TEventHandler> : Exception
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
{
    /// <summary>
    /// Gets the event type.
    /// </summary>
    public Type EventType => typeof(TEvent);

    /// <summary>
    /// Gets the event handler type.
    /// </summary>
    public Type EventHandlerType => typeof(TEventHandler);

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerAlreadyRegisteredException{TEvent, TEventHandler}"/> class.
    /// </summary>
    public EventHandlerAlreadyRegisteredException() :
        base($"Handler type {typeof(TEventHandler).Name} already registered for '{typeof(TEvent).Name}'")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerAlreadyRegisteredException{TEvent, TEventHandler}"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public EventHandlerAlreadyRegisteredException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHandlerAlreadyRegisteredException{TEvent, TEventHandler}"/> class with a specified error
    /// message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public EventHandlerAlreadyRegisteredException(string message, Exception innerException) : base(message, innerException)
    {
    }
}