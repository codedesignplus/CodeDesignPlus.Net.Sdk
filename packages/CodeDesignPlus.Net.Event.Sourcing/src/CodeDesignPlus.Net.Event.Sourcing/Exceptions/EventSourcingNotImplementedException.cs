namespace CodeDesignPlus.Net.Event.Sourcing;

/// <summary>
/// Se genera cuando se quiere acceder a un evento que no esta registrado
/// </summary>
[Serializable]
public class EventSourcingNotImplementedException : Exception
{

    /// <summary>
    /// Crea una nueva instancia de <see cref="EventSourcingNotImplementedException"/>
    /// </summary>
    public EventSourcingNotImplementedException()
    {
    }

    /// <summary>
    /// Crea una nueva instancia de <see cref="PubSubException"/>
    /// </summary>
    /// <param name="message">Mensaje del error</param>
    public EventSourcingNotImplementedException(string message) : base(message)
    {
    }

    /// <summary>
    /// Crea una nueva instancia de <see cref="EventSourcingNotImplementedException"/>
    /// </summary>
    /// <param name="message">Mensaje del error</param>
    /// <param name="innerException">Inner Exception</param>
    public EventSourcingNotImplementedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Without this constructor, deserialization will fail
    /// </summary>
    /// <param name="info">Serialization Info</param>
    /// <param name="context">Streaming Context</param>
    protected EventSourcingNotImplementedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}