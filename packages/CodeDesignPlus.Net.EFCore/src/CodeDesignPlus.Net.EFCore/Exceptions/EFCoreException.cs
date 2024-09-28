namespace CodeDesignPlus.Net.EFCore.Exceptions;

/// <summary>
/// Represents an exception specific to EFCore operations.
/// </summary>
/// <remarks>
/// This exception is thrown when an error occurs during EFCore operations.
/// </remarks>
public class EFCoreException : Exception
{
    /// <summary>
    /// Gets or sets the collection of error messages associated with the exception.
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFCoreException"/> class.
    /// </summary>
    public EFCoreException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFCoreException"/> class with a specified collection of error messages.
    /// </summary>
    /// <param name="errors">The collection of error messages.</param>
    public EFCoreException(IEnumerable<string> errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFCoreException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public EFCoreException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFCoreException"/> class with a specified error message and a collection of error messages.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errors">The collection of error messages.</param>
    public EFCoreException(string message, IEnumerable<string> errors) : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFCoreException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public EFCoreException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFCoreException"/> class with a specified error message, a collection of error messages, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errors">The collection of error messages.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public EFCoreException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
    {
        this.Errors = errors;
    }
}
