using System.Collections.Concurrent;
using CodeDesignPlus.Net.gRpc.Clients.Services.Notification;
using CodeDesignPlus.Net.gRpc.Clients.Services.Notifications;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.Notifications;

/// <summary>
/// A notice taken from the queue when the stream had already dropped is sent again after reconnecting, instead of being lost.
/// </summary>
/// <remarks>
/// Kappali, 7-nov-2026: the first notice of the morning run of ms-parking hit a stream that had died while idle («Error in Inbox.
/// Reconnecting in 5s...») and never reached ms-notification; the ones after the reconnection did. Resending is safe because the
/// inbox recognizes a notice by its id.
/// </remarks>
public class InboxReconnectTest
{
    [Fact]
    public async Task NotifyAsync_StreamDroppedWhileIdle_SendsTheNoticeAfterReconnecting()
    {
        // Arrange
        var delivered = new ConcurrentQueue<NotificationRequest>();
        var calls = 0;
        var client = new Mock<Inbox.InboxClient>();
        client
            .Setup(x => x.Notify(It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Returns(() => Interlocked.Increment(ref calls) == 1 ? Call(new BrokenWriter()) : Call(new RecordingWriter(delivered)));

        await using var service = new InboxService(client.Object, Mock.Of<ILogger<InboxService>>());
        var notice = new NotificationRequest { Id = Guid.NewGuid().ToString(), Title = "Perdiste el cupo del sorteo" };

        // Act
        await service.NotifyAsync(notice, CancellationToken.None);

        // Assert
        Assert.True(await DeliveredAsync(delivered, notice.Id, TimeSpan.FromSeconds(15)));
    }

    private static async Task<bool> DeliveredAsync(ConcurrentQueue<NotificationRequest> delivered, string id, TimeSpan timeout)
    {
        var until = DateTime.UtcNow + timeout;

        while (DateTime.UtcNow < until)
        {
            if (delivered.Any(x => x.Id == id))
            {
                return true;
            }

            await Task.Delay(100);
        }

        return false;
    }

    private static AsyncDuplexStreamingCall<NotificationRequest, NotifyAck> Call(IClientStreamWriter<NotificationRequest> writer) =>
        new(writer, new IdleReader(), Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => [], () => { });

    private sealed class BrokenWriter : IClientStreamWriter<NotificationRequest>
    {
        public WriteOptions? WriteOptions { get; set; }

        public Task CompleteAsync() => Task.CompletedTask;

        public Task WriteAsync(NotificationRequest message) => throw new RpcException(new Status(StatusCode.Unavailable, "The stream dropped while idle."));

        public Task WriteAsync(NotificationRequest message, CancellationToken cancellationToken) => WriteAsync(message);
    }

    private sealed class RecordingWriter(ConcurrentQueue<NotificationRequest> delivered) : IClientStreamWriter<NotificationRequest>
    {
        public WriteOptions? WriteOptions { get; set; }

        public Task CompleteAsync() => Task.CompletedTask;

        public Task WriteAsync(NotificationRequest message)
        {
            delivered.Enqueue(message);

            return Task.CompletedTask;
        }

        public Task WriteAsync(NotificationRequest message, CancellationToken cancellationToken) => WriteAsync(message);
    }

    private sealed class IdleReader : IAsyncStreamReader<NotifyAck>
    {
        public NotifyAck Current => new();

        public async Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);

            return false;
        }
    }
}
