using CodeDesignPlus.Net.Core.Abstractions.Outbox;

namespace CodeDesignPlus.Net.PubSub.Abstractions;

/// <summary>
/// Un transporte que sabe enviar un evento ya serializado, que es lo que guarda la bandeja de salida (pendings/245).
/// </summary>
/// <remarks>
/// Lo implementa <c>CodeDesignPlus.Net.ServiceBus</c>. Con un transporte que no lo implementa, los eventos se publican
/// directo y la bandeja no se usa.
/// </remarks>
public interface IOutboxTransport
{
    /// <summary>
    /// Envía el mensaje a su destino, con el mismo identificador y el mismo cuerpo con que se encoló.
    /// </summary>
    /// <param name="message">El mensaje de la bandeja.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    Task SendAsync(OutboxMessage message, CancellationToken cancellationToken);
}
