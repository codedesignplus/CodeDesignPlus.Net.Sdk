using CodeDesignPlus.Net.gRpc.Clients.Services.Notification;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Empuja mensajes efimeros por SignalR. <b>No persiste nada.</b>
/// </summary>
/// <remarks>
/// Es para lo que pierde valor en segundos: barras de progreso, turnos de palabra en una asamblea, el
/// estado de una sala en vivo. Si el destinatario no esta conectado el mensaje se pierde, y esta bien.
/// <para>
/// La pregunta que decide entre esto y <see cref="IInboxGrpc"/>, en cada punto de llamada: <b>le sirve
/// esto a alguien que no estaba mirando la pantalla?</b> Si la respuesta es si, no va por aqui.
/// </para>
/// <para>
/// Use los metodos de extension de <see cref="LiveChannelExtensions"/> en vez de estos: serializan el
/// payload en camelCase, que es lo que el frontend lee.
/// </para>
/// </remarks>
public interface ILiveChannelGrpc
{
    /// <summary>Empuja un mensaje efimero a un usuario concreto.</summary>
    /// <param name="request">El mensaje, con su tenant y su destinatario.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    Task PushToUserAsync(LiveUserPush request, CancellationToken cancellationToken);

    /// <summary>Empuja un mensaje efimero a un grupo de la copropiedad.</summary>
    /// <param name="request">El mensaje, con el nombre del grupo sin calificar: el servidor le antepone el tenant.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    Task PushToGroupAsync(LiveGroupPush request, CancellationToken cancellationToken);
}
