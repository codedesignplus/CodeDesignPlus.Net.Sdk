namespace CodeDesignPlus.Net.RabbitMQ.Exceptions;

/// <summary>
/// Represents errors that occur during RabbitMQ operations.
/// </summary>
public class RabbitMQException : Exception
{
    /// <summary>
    /// Gets or sets the collection of error messages.
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQException"/> class.
    /// </summary>
    public RabbitMQException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQException"/> class with a specified error message collection.
    /// </summary>
    /// <param name="errors">The collection of error messages.</param>
    public RabbitMQException(IEnumerable<string> errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public RabbitMQException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQException"/> class with a specified error message and error message collection.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The collection of error messages.</param>
    public RabbitMQException(string message, IEnumerable<string> errors) : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public RabbitMQException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQException"/> class with a specified error message, error message collection, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The collection of error messages.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public RabbitMQException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
    {
        this.Errors = errors;
    }
}