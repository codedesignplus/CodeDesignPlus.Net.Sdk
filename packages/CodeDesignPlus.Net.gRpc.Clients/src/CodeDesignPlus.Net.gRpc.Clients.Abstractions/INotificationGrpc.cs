using System;
using CodeDesignPlus.Net.gRpc.Clients.Services.Notification;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Service to send notification messages using websockets with SignalR Core.
/// </summary>
public interface INotificationGrpc
{
    /// <summary>
    /// Sends a notification to a specific user identified by their ID.
    /// </summary>
    /// <param name="request">The notification request containing user ID and message details.</param>
    /// <returns>A task that represents the asynchronous operation. </returns>
    Task SendToUserAsync(NotificationUserRequest request, CancellationToken cancellationToken);
    /// <summary>
    /// Sends a broadcast notification to all connected users within a tenant.
    /// </summary>
    /// <param name="request">The broadcast notification request containing event details and payload.</param>
    /// <returns>A task that represents the asynchronous operation. </returns>
    Task BroadcastAsync(NotificationBroadcastRequest request, CancellationToken cancellationToken);
    /// <summary>
    /// Sends notifications to a logical group of connections (e.g., "Administrators", "Project_A").
    /// </summary>
    /// <param name="request">The group notification request containing group name, event details, and payload.</param>
    /// <returns>A task that represents the asynchronous operation. </returns>
    Task SendToGroupAsync(NotificationGroupRequest request, CancellationToken cancellationToken);
}
