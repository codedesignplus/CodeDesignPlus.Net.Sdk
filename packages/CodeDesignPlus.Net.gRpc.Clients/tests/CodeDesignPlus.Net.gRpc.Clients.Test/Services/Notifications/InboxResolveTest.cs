using CodeDesignPlus.Net.gRpc.Clients.Services.Notification;
using CodeDesignPlus.Net.gRpc.Clients.Services.Notifications;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.Notifications;

/// <summary>
/// Cerrar un aviso no puede tumbar el hecho que lo resolvio: si ms-notification no responde, el cliente lo registra y
/// devuelve <c>false</c> en vez de lanzar.
/// </summary>
public class InboxResolveTest
{
    [Fact]
    public async Task ResolveAsync_ServiceUnavailable_ReturnsFalseInsteadOfThrowing()
    {
        // Arrange
        var client = new Mock<Inbox.InboxClient>();
        client
            .Setup(x => x.ResolveAsync(It.IsAny<ResolveRequest>(), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Throws(new RpcException(new Status(StatusCode.Unavailable, "down")));

        await using var service = new InboxService(client.Object, Mock.Of<ILogger<InboxService>>());

        // Act
        var accepted = await service.ResolveAsync(new ResolveRequest { Tenant = Guid.NewGuid().ToString() }, CancellationToken.None);

        // Assert
        Assert.False(accepted);
    }

    [Fact]
    public async Task ResolveAsync_ServiceAccepts_ReturnsTrue()
    {
        // Arrange
        var client = new Mock<Inbox.InboxClient>();
        client
            .Setup(x => x.ResolveAsync(It.IsAny<ResolveRequest>(), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<ResolveAck>(
                Task.FromResult(new ResolveAck { Success = true, Resolved = 2 }),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }));

        await using var service = new InboxService(client.Object, Mock.Of<ILogger<InboxService>>());

        // Act
        var accepted = await service.ResolveAsync(new ResolveRequest { Tenant = Guid.NewGuid().ToString() }, CancellationToken.None);

        // Assert
        Assert.True(accepted);
    }
}
