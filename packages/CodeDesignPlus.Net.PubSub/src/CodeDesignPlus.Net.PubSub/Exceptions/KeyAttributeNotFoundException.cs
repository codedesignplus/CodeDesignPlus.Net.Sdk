namespace CodeDesignPlus.Net.PubSub.Exceptions;

/// <summary>
/// The exception that is thrown when an error occurs within CodeDesignPlus.Net.PubSub. 
/// </summary>
public class KeyAttributeNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyAttributeNotFoundException"/> class.
    /// </summary>
    public KeyAttributeNotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyAttributeNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public KeyAttributeNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyAttributeNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic)
    /// if no inner exception is specified.
    /// </param>
    public KeyAttributeNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
