namespace CodeDesignPlus.Net.Event.Sourcing.Exceptions;

/// <summary>
/// The exception that is thrown when an error occurs within CodeDesignPlus.Net.Event.Sourcing. 
/// </summary>
public class EventSourcingException : Exception
{
    /// <summary>
    /// Contains the errors.
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventSourcingException"/> class.
    /// </summary>
    public EventSourcingException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventSourcingException"/> class with a specified error message and a collection of errors.
    /// </summary>
    public EventSourcingException(IEnumerable<string> errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventSourcingException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public EventSourcingException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventSourcingException"/> class with a specified error message and a collection of errors.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The collection of errors.</param>
    public EventSourcingException(string message, IEnumerable<string> errors) : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventSourcingException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public EventSourcingException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventSourcingException"/> class with a specified error message, a collection of errors, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errors">The collection of errors.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public EventSourcingException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
    {
        this.Errors = errors;
    }
}