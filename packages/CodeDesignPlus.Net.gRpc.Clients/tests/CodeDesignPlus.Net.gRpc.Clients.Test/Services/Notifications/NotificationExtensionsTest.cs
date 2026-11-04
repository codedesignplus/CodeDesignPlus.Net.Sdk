using CodeDesignPlus.Net.gRpc.Clients.Services.Notification;
using Moq;
using NodaTime;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.Notifications;

/// <summary>
/// Cubre la serializacion del payload de las notificaciones en tiempo real.
/// </summary>
public class NotificationExtensionsTest
{
    /// <summary>
    /// Las notificaciones en tiempo real usan la misma serializacion que la bandeja: camelCase y las fechas de NodaTime en
    /// ISO-8601, nunca <c>{}</c>.
    /// </summary>
    [Fact]
    public async Task NotifyUserAsync_PayloadWithInstant_SerializesCamelCaseIsoUtc()
    {
        // Arrange
        NotificationUserRequest? captured = null;

        var grpc = new Mock<INotificationGrpc>();
        grpc.Setup(x => x.SendToUserAsync(It.IsAny<NotificationUserRequest>(), It.IsAny<CancellationToken>()))
            .Callback<NotificationUserRequest, CancellationToken>((r, _) => captured = r)
            .Returns(Task.CompletedTask);

        // Act
        await grpc.Object.NotifyUserAsync(Guid.NewGuid(), "license.expiring", new { EndDate = Instant.FromUtc(2026, 12, 1, 5, 0) }, Guid.NewGuid(), Guid.Empty);

        // Assert
        Assert.Equal("{\"endDate\":\"2026-12-01T05:00:00Z\"}", captured!.JsonPayload);
    }
}
