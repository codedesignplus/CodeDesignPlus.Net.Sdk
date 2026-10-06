namespace CodeDesignPlus.Net.Mongo.Abstractions.Exceptions;

/// <summary>
/// The exception that is thrown when a versioned aggregate changed in the store since it was read.
/// </summary>
/// <remarks>
/// It is an infrastructure error, not a business one: it does not derive from <c>CodeDesignPlusException</c>, so a
/// message bus retries the message instead of dead-lettering it. Retrying is the right answer, because reading again
/// and reapplying the change succeeds once the other writer is done.
/// </remarks>
public class ConcurrencyConflictException : Exception
{
    /// <summary>
    /// Gets the name of the aggregate type whose write was rejected.
    /// </summary>
    public string EntityType { get; }

    /// <summary>
    /// Gets the identifier of the aggregate whose write was rejected.
    /// </summary>
    public Guid EntityId { get; }

    /// <summary>
    /// Gets the version the writer read, and expected to find still stored.
    /// </summary>
    public long ExpectedVersion { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrencyConflictException"/> class.
    /// </summary>
    public ConcurrencyConflictException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrencyConflictException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ConcurrencyConflictException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrencyConflictException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ConcurrencyConflictException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrencyConflictException"/> class for a rejected write.
    /// </summary>
    /// <param name="entityType">The aggregate type.</param>
    /// <param name="entityId">The aggregate identifier.</param>
    /// <param name="expectedVersion">The version the writer read.</param>
    /// <param name="innerException">The exception that is the cause of the conflict, if any.</param>
    public ConcurrencyConflictException(Type entityType, Guid entityId, long expectedVersion, Exception innerException = null)
        : base($"{entityType?.Name} {entityId} changed since version {expectedVersion} was read.", innerException)
    {
        this.EntityType = entityType?.Name;
        this.EntityId = entityId;
        this.ExpectedVersion = expectedVersion;
    }
}
