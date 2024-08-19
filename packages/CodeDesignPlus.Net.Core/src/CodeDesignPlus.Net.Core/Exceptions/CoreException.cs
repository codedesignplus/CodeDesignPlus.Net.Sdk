namespace CodeDesignPlus.Net.Core.Exceptions;

/// <summary>
/// Represents an exception specific to the core functionality of the application.
/// </summary>
/// <remarks>
/// This exception is typically thrown when an error occurs in the core logic of the application.
/// It can contain a collection of error messages associated with the exception.
/// </remarks>
public class CoreException : Exception
{
    /// <summary>
    /// Gets or sets the collection of error messages associated with the exception.
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreException"/> class.
    /// </summary>
    public CoreException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreException"/> class with a collection of error messages.
    /// </summary>
    /// <param name="errors">The collection of error messages.</param>
    public CoreException(IEnumerable<string> errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public CoreException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreException"/> class with a specified error message and a collection of error messages.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errors">The collection of error messages.</param>
    public CoreException(string message, IEnumerable<string> errors) : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CoreException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreException"/> class with a specified error message, a collection of error messages, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errors">The collection of error messages.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CoreException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
    {
        this.Errors = errors;
    }
}
