namespace CodeDesignPlus.Net.Security.Exceptions;

/// <summary>
/// Represents errors that occur during security operations.
/// </summary>
public class SecurityException : Exception
{
    /// <summary>
    /// Gets or sets the collection of error messages.
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityException"/> class.
    /// </summary>
    public SecurityException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityException"/> class with a specified collection of error messages.
    /// </summary>
    /// <param name="errors">The collection of error messages.</param>
    public SecurityException(IEnumerable<string> errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public SecurityException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityException"/> class with a specified error message and a collection of error messages.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The collection of error messages.</param>
    public SecurityException(string message, IEnumerable<string> errors) : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public SecurityException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityException"/> class with a specified error message, a collection of error messages, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The collection of error messages.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public SecurityException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
    {
        this.Errors = errors;
    }
}