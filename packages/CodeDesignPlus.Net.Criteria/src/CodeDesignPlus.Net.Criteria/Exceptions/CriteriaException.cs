namespace CodeDesignPlus.Net.Criteria.Exceptions;

/// <summary>
/// Represents an exception that is thrown when there is an error related to criteria.
/// </summary>
public class CriteriaException : Exception
{
    /// <summary>
    /// Gets or sets the collection of errors associated with the exception.
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CriteriaException"/> class.
    /// </summary>
    public CriteriaException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CriteriaException"/> class with a collection of errors.
    /// </summary>
    /// <param name="errors">The collection of errors associated with the exception.</param>
    public CriteriaException(IEnumerable<string> errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CriteriaException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public CriteriaException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CriteriaException"/> class with a specified error message and a collection of errors.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errors">The collection of errors associated with the exception.</param>
    public CriteriaException(string message, IEnumerable<string> errors) : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CriteriaException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CriteriaException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CriteriaException"/> class with a specified error message, a collection of errors, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errors">The collection of errors associated with the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CriteriaException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
    {
        this.Errors = errors;
    }
}
