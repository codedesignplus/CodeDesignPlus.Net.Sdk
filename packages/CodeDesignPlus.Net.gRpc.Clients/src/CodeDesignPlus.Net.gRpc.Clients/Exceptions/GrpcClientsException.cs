using System.Runtime.Serialization;

namespace CodeDesignPlus.Net.gRpc.Clients.Exceptions;

/// <summary>
/// The exception that is thrown when an error occurs within CodeDesignPlus.Net.gRpc.Clients. 
/// </summary>
public class GrpcClientsException : Exception
{
    /// <summary>
    /// Contains the errors
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrpcClientsException"/> class.
    /// </summary>
    public GrpcClientsException()
    {
        this.Errors = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrpcClientsException"/> class.
    /// </summary>
    /// <param name="errors">The custom errors</param>
    public GrpcClientsException(IEnumerable<string> errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrpcClientsException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public GrpcClientsException(string message) : base(message)
    {
        this.Errors = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrpcClientsException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The custom errors</param>
    public GrpcClientsException(string message, IEnumerable<string> errors) : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrpcClientsException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic)
    /// if no inner exception is specified.
    /// </param>
    public GrpcClientsException(string message, Exception innerException) : base(message, innerException)
    {
        this.Errors = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrpcClientsException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The custom errors</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic)
    /// if no inner exception is specified.
    /// </param>
    public GrpcClientsException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
    {
        this.Errors = errors;
    }
}
