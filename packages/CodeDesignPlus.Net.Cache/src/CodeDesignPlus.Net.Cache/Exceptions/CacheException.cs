namespace CodeDesignPlus.Net.Cache.Exceptions;

/// <summary>
/// The exception that is thrown when an error occurs within CodeDesignPlus.Net.Cache. 
/// </summary>
public class CacheException : Exception
{
    /// <summary>
    /// Contains the errors
    /// </summary>
    public IEnumerable<string> Errors { get; set; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheException"/> class.
    /// </summary>
    public CacheException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheException"/> class.
    /// </summary>
    /// <param name="errors">The custom errors</param>
    public CacheException(IEnumerable<string> errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CacheException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The custom errors</param>
    public CacheException(string message, IEnumerable<string> errors) : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic)
    /// if no inner exception is specified.
    /// </param>
    public CacheException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The custom errors</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic)
    /// if no inner exception is specified.
    /// </param>
    public CacheException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
    {
        this.Errors = errors;
    }
}
