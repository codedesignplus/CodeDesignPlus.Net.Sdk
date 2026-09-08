using CodeDesignPlus.Net.gRpc.Clients.Services.Notification;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Guarda un aviso durable y, si hay alguien conectado, lo empuja tambien.
/// </summary>
/// <remarks>
/// Todo lo que entra por aqui se persiste <b>pase lo que pase con el push</b>, y por eso la campana tiene
/// contenido y un flujo no se pierde porque el residente reinicio el navegador.
/// <para>
/// Use los metodos de extension de <see cref="InboxExtensions"/> en vez de este: arman la audiencia, el
/// identificador de idempotencia y la marca de tiempo, y serializan el payload en camelCase.
/// </para>
/// </remarks>
public interface IInboxGrpc
{
    /// <summary>Persiste el aviso y lo empuja a su audiencia.</summary>
    /// <param name="request">El aviso completo: audiencia, tipo, titulo, cuerpo y recurso al que lleva.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    Task NotifyAsync(NotificationRequest request, CancellationToken cancellationToken);
}
