namespace CodeDesignPlus.Net.PubSub.Exceptions;

/// <summary>
/// Se genera cuando se quiere acceder a un evento que no esta registrado
/// </summary>
public class EventNotImplementedException : Exception
{

    /// <summary>
    /// Crea una nueva instancia de <see cref="EventNotImplementedException"/>
    /// </summary>
    public EventNotImplementedException()
    {
    }

    /// <summary>
    /// Crea una nueva instancia de <see cref="PubSubException"/>
    /// </summary>
    /// <param name="message">Mensaje del error</param>
    public EventNotImplementedException(string message) : base(message)
    {
    }

    /// <summary>
    /// Crea una nueva instancia de <see cref="EventNotImplementedException"/>
    /// </summary>
    /// <param name="message">Mensaje del error</param>
    /// <param name="innerException">Inner Exception</param>
    public EventNotImplementedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
