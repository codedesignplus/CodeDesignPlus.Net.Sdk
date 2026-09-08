using System.Text.Json;
using CodeDesignPlus.Net.gRpc.Clients.Services.Notification;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Métodos de extensión sobre <see cref="INotificationGrpc"/> que simplifican el envío
/// de notificaciones al construir y serializar automáticamente el payload JSON.
/// </summary>
public static class NotificationExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Envía una notificación a un usuario específico, serializando el payload a JSON.
    /// </summary>
    /// <param name="grpc">La instancia del cliente de notificaciones.</param>
    /// <param name="userId">El identificador del usuario destinatario.</param>
    /// <param name="eventName">
    /// El nombre del evento que el frontend escucha (usar constantes de <see cref="NotificationKinds"/>).
    /// </param>
    /// <param name="payload">El objeto que se serializa como payload JSON de la notificación.</param>
    /// <param name="tenant">El identificador del tenant al que pertenece el usuario.</param>
    /// <param name="sentBy">El identificador del sistema o usuario que origina la notificación.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>Una tarea que representa la operación asincrónica.</returns>
    public static Task NotifyUserAsync(
        this INotificationGrpc grpc,
        Guid userId,
        string eventName,
        object payload,
        Guid tenant,
        Guid sentBy,
        CancellationToken cancellationToken = default)
    {
        var request = new NotificationUserRequest
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId.ToString(),
            EventName = eventName,
            JsonPayload = JsonSerializer.Serialize(payload, SerializerOptions),
            Tenant = tenant.ToString(),
            SentBy = sentBy.ToString()
        };

        return grpc.SendToUserAsync(request, cancellationToken);
    }

    /// <summary>
    /// Envía una notificación broadcast a todos los usuarios conectados del tenant,
    /// serializando el payload a JSON.
    /// </summary>
    /// <param name="grpc">La instancia del cliente de notificaciones.</param>
    /// <param name="eventName">
    /// El nombre del evento que el frontend escucha (usar constantes de <see cref="NotificationKinds"/>).
    /// </param>
    /// <param name="payload">El objeto que se serializa como payload JSON de la notificación.</param>
    /// <param name="tenant">El identificador del tenant al que pertenecen los usuarios.</param>
    /// <param name="sentBy">El identificador del sistema o usuario que origina la notificación.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>Una tarea que representa la operación asincrónica.</returns>
    public static Task BroadcastToTenantAsync(
        this INotificationGrpc grpc,
        string eventName,
        object payload,
        Guid tenant,
        Guid sentBy,
        CancellationToken cancellationToken = default)
    {
        var request = new NotificationBroadcastRequest
        {
            Id = Guid.NewGuid().ToString(),
            EventName = eventName,
            JsonPayload = JsonSerializer.Serialize(payload, SerializerOptions),
            Tenant = tenant.ToString(),
            SentBy = sentBy.ToString()
        };

        return grpc.BroadcastAsync(request, cancellationToken);
    }

    /// <summary>
    /// Envía una notificación a un grupo lógico de conexiones SignalR,
    /// serializando el payload a JSON.
    /// </summary>
    /// <param name="grpc">La instancia del cliente de notificaciones.</param>
    /// <param name="groupName">
    /// El nombre del grupo SignalR (ej. "Administrators", "Accountants").
    /// Debe coincidir con el grupo al que el frontend se unió mediante JoinGroup.
    /// </param>
    /// <param name="eventName">
    /// El nombre del evento que el frontend escucha (usar constantes de <see cref="NotificationKinds"/>).
    /// </param>
    /// <param name="payload">El objeto que se serializa como payload JSON de la notificación.</param>
    /// <param name="tenant">El identificador del tenant.</param>
    /// <param name="sentBy">El identificador del sistema o usuario que origina la notificación.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>Una tarea que representa la operación asincrónica.</returns>
    public static Task NotifyGroupAsync(
        this INotificationGrpc grpc,
        string groupName,
        string eventName,
        object payload,
        Guid tenant,
        Guid sentBy,
        CancellationToken cancellationToken = default)
    {
        var request = new NotificationGroupRequest
        {
            Id = Guid.NewGuid().ToString(),
            GroupName = groupName,
            EventName = eventName,
            JsonPayload = JsonSerializer.Serialize(payload, SerializerOptions),
            Tenant = tenant.ToString(),
            SentBy = sentBy.ToString()
        };

        return grpc.SendToGroupAsync(request, cancellationToken);
    }
}
