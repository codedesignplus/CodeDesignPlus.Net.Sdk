namespace CodeDesignPlus.Net.Serializers.Exceptions;

/// <summary>
/// The exception that is thrown when an error occurs within CodeDesignPlus.Net.Serializers. 
/// </summary>
public class SerializersException : Exception
{
    /// <summary>
    /// Gets or sets the collection of custom errors.
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializersException"/> class.
    /// </summary>
    public SerializersException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializersException"/> class with a specified collection of custom errors.
    /// </summary>
    /// <param name="errors">The custom errors.</param>
    public SerializersException(IEnumerable<string> errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializersException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public SerializersException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializersException"/> class with a specified error message and a collection of custom errors.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The custom errors.</param>
    public SerializersException(string message, IEnumerable<string> errors) : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializersException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic)
    /// if no inner exception is specified.
    /// </param>
    public SerializersException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializersException"/> class with a specified error message, a collection of custom errors, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The custom errors.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic)
    /// if no inner exception is specified.
    /// </param>
    public SerializersException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
    {
        this.Errors = errors;
    }
}