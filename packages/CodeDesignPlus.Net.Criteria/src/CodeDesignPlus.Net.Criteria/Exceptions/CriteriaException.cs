namespace CodeDesignPlus.Net.Criteria.Exceptions;

/// <summary>
/// Represents an exception that is thrown when there is an error related to criteria.
/// </summary>
/// <remarks>
/// It is a <see cref="CodeDesignPlusException"/> of the application layer so that the microservice answers a malformed
/// filter as what it is, a bad request: 400 with a ProblemDetails (RFC 9457) in REST and <c>InvalidArgument</c> in gRPC.
/// Before it derived from <see cref="Exception"/> and every filter error became a 500 (Kappali pendings/403).
/// <para>
/// The SDK throws it with a <see cref="CriteriaErrors"/> entry, translated at the edge. The constructors that take a
/// message are kept for compatibility and carry the code of <see cref="CriteriaErrors.InvalidFilter"/>.
/// </para>
/// </remarks>
public class CriteriaException : CodeDesignPlusException
{
    /// <summary>
    /// Gets or sets the collection of errors associated with the exception.
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CriteriaException"/> class.
    /// </summary>
    public CriteriaException() : base(Layer.Application, CriteriaErrors.InvalidFilter.Code)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CriteriaException"/> class from a catalog error.
    /// </summary>
    /// <param name="error">The catalog error that explains why the filter is not valid.</param>
    public CriteriaException(Error error) : base(Layer.Application, error)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CriteriaException"/> class from a catalog error and the exception that caused it.
    /// </summary>
    /// <param name="error">The catalog error that explains why the filter is not valid.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CriteriaException(Error error, Exception innerException) : base(Layer.Application, error, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CriteriaException"/> class with a collection of errors.
    /// </summary>
    /// <param name="errors">The collection of errors associated with the exception.</param>
    public CriteriaException(IEnumerable<string> errors) : this()
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CriteriaException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public CriteriaException(string message) : base(Layer.Application, CriteriaErrors.InvalidFilter.Code, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CriteriaException"/> class with a specified error message and a collection of errors.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errors">The collection of errors associated with the exception.</param>
    public CriteriaException(string message, IEnumerable<string> errors) : this(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CriteriaException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CriteriaException(string message, Exception innerException) : base(Layer.Application, CriteriaErrors.InvalidFilter.Code, message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CriteriaException"/> class with a specified error message, a collection of errors, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errors">The collection of errors associated with the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CriteriaException(string message, IEnumerable<string> errors, Exception innerException) : this(message, innerException)
    {
        this.Errors = errors;
    }
}
