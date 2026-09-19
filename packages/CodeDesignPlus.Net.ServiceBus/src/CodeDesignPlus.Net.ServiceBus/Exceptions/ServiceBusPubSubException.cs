namespace CodeDesignPlus.Net.ServiceBus.Exceptions;

/// <summary>
/// The exception that is thrown when the Azure Service Bus transport cannot complete an operation.
/// </summary>
/// <remarks>
/// No se llama <c>ServiceBusException</c> a proposito, aunque la convencion del SDK seria esa: ese nombre ya
/// lo ocupa <see cref="Azure.Messaging.ServiceBus.ServiceBusException"/> y, con los <c>global using</c> de este
/// paquete, las dos referencias quedarian ambiguas en cada fichero.
/// </remarks>
public class ServiceBusPubSubException : Exception
{
    /// <summary>
    /// Gets or sets the errors.
    /// </summary>
    public IEnumerable<string> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusPubSubException"/> class.
    /// </summary>
    public ServiceBusPubSubException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusPubSubException"/> class.
    /// </summary>
    /// <param name="errors">The collection of errors.</param>
    public ServiceBusPubSubException(IEnumerable<string> errors)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusPubSubException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ServiceBusPubSubException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusPubSubException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The collection of errors.</param>
    public ServiceBusPubSubException(string message, IEnumerable<string> errors) : base(message)
    {
        this.Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusPubSubException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ServiceBusPubSubException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusPubSubException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errors">The collection of errors.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ServiceBusPubSubException(string message, IEnumerable<string> errors, Exception innerException) : base(message, innerException)
    {
        this.Errors = errors;
    }
}
