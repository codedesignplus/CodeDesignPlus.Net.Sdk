namespace CodeDesignPlus.Net.PubSub.Exceptions;
/// <summary>
/// Se genera cuando se quiere acceder a un evento que no esta registrado
/// </summary>
public class EventNotExistException : Exception
{

    /// <summary>
    /// Crea una nueva instancia de <see cref="EventNotExistException"/>
    /// </summary>
    public EventNotExistException()
    {
    }

    /// <summary>
    /// Crea una nueva instancia de <see cref="PubSubException"/>
    /// </summary>
    /// <param name="message">Mensaje del error</param>
    public EventNotExistException(string message) : base(message)
    {
    }

    /// <summary>
    /// Crea una nueva instancia de <see cref="EventNotExistException"/>
    /// </summary>
    /// <param name="message">Mensaje del error</param>
    /// <param name="innerException">Inner Exception</param>
    public EventNotExistException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
