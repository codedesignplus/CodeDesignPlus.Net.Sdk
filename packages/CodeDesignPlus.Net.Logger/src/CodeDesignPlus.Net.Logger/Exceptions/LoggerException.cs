namespace CodeDesignPlus.Net.Logger.Exceptions;


/// <summary>
/// The exception that is thrown when an error occurs within CodeDesignPlus.Net.Kafka. 
/// </summary>
public class LoggerException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerException"/> class.
    /// </summary>
    public LoggerException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public LoggerException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic)
    /// if no inner exception is specified.
    /// </param>
    public LoggerException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
