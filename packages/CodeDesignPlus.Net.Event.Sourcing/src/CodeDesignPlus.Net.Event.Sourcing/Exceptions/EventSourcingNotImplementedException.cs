namespace CodeDesignPlus.Net.Event.Sourcing.Exceptions;

/// <summary>
/// Se genera cuando se quiere acceder a un evento que no esta registrado
/// </summary>
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
}