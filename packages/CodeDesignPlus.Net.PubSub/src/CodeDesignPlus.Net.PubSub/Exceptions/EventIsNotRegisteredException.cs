namespace CodeDesignPlus.Net.PubSub.Exceptions;

/// <summary>
/// This exception is thrown when an event handler is not registered in the subscription manager.
/// </summary>
public class EventIsNotRegisteredException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventIsNotRegisteredException"/> class.
    /// </summary>
    public EventIsNotRegisteredException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventIsNotRegisteredException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public EventIsNotRegisteredException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventIsNotRegisteredException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public EventIsNotRegisteredException(string message, Exception innerException) : base(message, innerException)
    {
    }
}