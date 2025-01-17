using System.Diagnostics.CodeAnalysis;

namespace CodeDesignPlus.Net.Mongo.Exceptions;

/// <summary>
/// The exception that is thrown when an error occurs within CodeDesignPlus.Net.Mongo.
/// </summary>
public class MongoException : Exception
{
    /// <summary>
    /// Gets or sets the collection of errors.
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoException"/> class.
    /// </summary>
    public MongoException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoException"/> class with custom errors.
    /// </summary>
    /// <param name="errors">The custom errors.</param>
    public MongoException(IEnumerable<string> errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MongoException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoException"/> class with a specified error message and custom errors.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The custom errors.</param>
    public MongoException(string message, IEnumerable<string> errors) : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MongoException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoException"/> class with a specified error message, custom errors, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The custom errors.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MongoException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Throws an <see cref="MongoException"/> if the argument is null.
    /// </summary>
    /// <param name="argument">The argument to check.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <exception cref="MongoException">Thrown when the argument is null.</exception>
    [DoesNotReturn]
    public static void ThrowIfNull([NotNull] object argument, string message)
    {
        if (argument is null)
            throw new MongoException(message);
    }
}