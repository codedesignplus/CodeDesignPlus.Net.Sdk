namespace CodeDesignPlus.Net.EventStore.Exceptions;

/// <summary>
/// The exception that is thrown when an error occurs within CodeDesignPlus.Net.EventStore. 
/// </summary>
public class EventStoreException : Exception
{
    /// <summary>
    /// Contains the errors
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreException"/> class.
    /// </summary>
    public EventStoreException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreException"/> class with custom errors.
    /// </summary>
    /// <param name="errors">The custom errors</param>
    public EventStoreException(IEnumerable<string> errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public EventStoreException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreException"/> class with a specified error message and custom errors.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The custom errors</param>
    public EventStoreException(string message, IEnumerable<string> errors) : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic)
    /// if no inner exception is specified.
    /// </param>
    public EventStoreException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreException"/> class with a specified error message, custom errors, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The custom errors</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic)
    /// if no inner exception is specified.
    /// </param>
    public EventStoreException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
    {
        this.Errors = errors;
    }
}