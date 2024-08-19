namespace CodeDesignPlus.Net.Observability.Exceptions;

/// <summary>
/// Represents errors that occur during observability operations.
/// </summary>
public class ObservabilityException : Exception
{
    /// <summary>
    /// Gets or sets the collection of errors.
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservabilityException"/> class.
    /// </summary>
    public ObservabilityException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservabilityException"/> class with a collection of errors.
    /// </summary>
    /// <param name="errors">The collection of errors.</param>
    public ObservabilityException(IEnumerable<string> errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservabilityException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ObservabilityException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservabilityException"/> class with a specified error message and a collection of errors.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The collection of errors.</param>
    public ObservabilityException(string message, IEnumerable<string> errors) : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservabilityException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ObservabilityException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservabilityException"/> class with a specified error message, a collection of errors, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The collection of errors.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ObservabilityException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
    {
        this.Errors = errors;
    }
}