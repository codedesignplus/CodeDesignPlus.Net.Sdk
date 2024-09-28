namespace CodeDesignPlus.Net.PubSub.Exceptions;

/// <summary>
/// Thrown when attempting to access an event that is not implemented.
/// </summary>
public class EventNotImplementedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventNotImplementedException"/> class.
    /// </summary>
    public EventNotImplementedException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventNotImplementedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public EventNotImplementedException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventNotImplementedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public EventNotImplementedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}