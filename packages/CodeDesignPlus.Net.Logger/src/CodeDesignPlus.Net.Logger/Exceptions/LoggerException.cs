namespace CodeDesignPlus.Net.Logger.Exceptions;

/// <summary>
/// The exception that is thrown when an error occurs within CodeDesignPlus.Net.Logger.
/// </summary>
public class LoggerException : Exception
{
    /// <summary>
    /// Contains the errors.
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerException"/> class.
    /// </summary>
    public LoggerException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerException"/> class with custom errors.
    /// </summary>
    /// <param name="errors">The custom errors.</param>
    public LoggerException(IEnumerable<string> errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public LoggerException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerException"/> class with a specified error message and custom errors.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The custom errors.</param>
    public LoggerException(string message, IEnumerable<string> errors) : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public LoggerException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerException"/> class with a specified error message, custom errors, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The custom errors.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public LoggerException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
    {
        this.Errors = errors;
    }
}