using CodeDesignPlus.Net.gRpc.Clients.Services.Notification;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// La puerta publica del canal efimero: arma el mensaje y serializa el payload en camelCase.
/// </summary>
public static class LiveChannelExtensions
{
    /// <summary>
    /// Empuja un mensaje efimero a un usuario. Si no esta conectado, se pierde.
    /// </summary>
    /// <param name="grpc">El cliente del canal en vivo.</param>
    /// <param name="userId">El usuario destinatario.</param>
    /// <param name="eventName">El nombre que el frontend escucha (ej. <c>charge.generation.progress</c>).</param>
    /// <param name="payload">El objeto que se serializa como payload JSON.</param>
    /// <param name="tenant">La copropiedad a la que pertenece el usuario.</param>
    /// <param name="correlationId">Para correlacionar trazas. No es idempotencia: aqui no hay nada que deduplicar.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Una tarea que representa la operacion asincronica.</returns>
    public static Task PushToUserAsync(
        this ILiveChannelGrpc grpc,
        Guid userId,
        string eventName,
        object payload,
        Guid tenant,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        var request = new LiveUserPush
        {
            Tenant = tenant.ToString(),
            UserId = userId.ToString(),
            EventName = eventName,
            JsonPayload = NotificationSerialization.Serialize(payload),
            CorrelationId = correlationId ?? string.Empty
        };

        return grpc.PushToUserAsync(request, cancellationToken);
    }

    /// <summary>
    /// Empuja un mensaje efimero a un grupo de la copropiedad.
    /// </summary>
    /// <param name="grpc">El cliente del canal en vivo.</param>
    /// <param name="groupName">
    /// El nombre del grupo <b>sin calificar</b>, ej. <c>assembly-42</c>. El servidor le antepone
    /// <c>Tenant:{tenant}:</c>: calificarlo aqui produciria un grupo con el prefijo dos veces, al que
    /// nadie esta suscrito y cuyo fallo es el silencio.
    /// </param>
    /// <param name="eventName">El nombre que el frontend escucha.</param>
    /// <param name="payload">El objeto que se serializa como payload JSON.</param>
    /// <param name="tenant">La copropiedad dueña del grupo.</param>
    /// <param name="correlationId">Para correlacionar trazas.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Una tarea que representa la operacion asincronica.</returns>
    public static Task PushToGroupAsync(
        this ILiveChannelGrpc grpc,
        string groupName,
        string eventName,
        object payload,
        Guid tenant,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        var request = new LiveGroupPush
        {
            Tenant = tenant.ToString(),
            GroupName = groupName,
            EventName = eventName,
            JsonPayload = NotificationSerialization.Serialize(payload),
            CorrelationId = correlationId ?? string.Empty
        };

        return grpc.PushToGroupAsync(request, cancellationToken);
    }
}
