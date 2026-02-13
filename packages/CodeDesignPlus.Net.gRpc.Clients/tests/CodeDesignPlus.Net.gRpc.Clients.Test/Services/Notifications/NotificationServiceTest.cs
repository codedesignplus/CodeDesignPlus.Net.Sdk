using System.Reflection;
using CodeDesignPlus.Net.gRpc.Clients.Services.Notification;
using CodeDesignPlus.Net.gRpc.Clients.Services.Notifications;
using CodeDesignPlus.Net.xUnit.Extensions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CodeDesignPlus.Net.gRpc.Clients.Test.Services.Notifications;

public class NotificationServiceTests
{
    private readonly Mock<Notifier.NotifierClient> mockClient;
    private readonly Mock<ILogger<NotificationService>> mockLogger;
    private readonly Mock<IClientStreamWriter<NotificationUserRequest>> mockUserStream;
    private readonly Mock<IClientStreamWriter<NotificationBroadcastRequest>> mockBroadcastStream;
    private readonly Mock<IClientStreamWriter<NotificationGroupRequest>> mockGroupStream;

    public NotificationServiceTests()
    {
        mockClient = new Mock<Notifier.NotifierClient>();
        mockLogger = new Mock<ILogger<NotificationService>>();
        mockUserStream = new Mock<IClientStreamWriter<NotificationUserRequest>>();
        mockBroadcastStream = new Mock<IClientStreamWriter<NotificationBroadcastRequest>>();
        mockGroupStream = new Mock<IClientStreamWriter<NotificationGroupRequest>>();
    }

    [Fact]
    public async Task SendToUserAsync_ShouldWriteToGrpcStream()
    {
        // Arrange
        var request = new NotificationUserRequest { Id = "1", UserId = "U1" };
        var resetEvent = new ManualResetEventSlim(false);

        mockUserStream
            .Setup(x => x.WriteAsync(It.Is<NotificationUserRequest>(r => r.Id == "1"), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(() => resetEvent.Set());

        var call = CreateCall(mockUserStream.Object);
        mockClient.Setup(x => x.SendToUser(It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
                   .Returns(call);

        using var service = new NotificationService(mockClient.Object, mockLogger.Object);

        // Act
        await service.SendToUserAsync(request, CancellationToken.None);

        // Assert
        Assert.True(resetEvent.Wait(1000), "El mensaje no fue procesado por el stream a tiempo.");

        mockUserStream.Verify(x => x.WriteAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task BroadcastAsync_ShouldWriteToGrpcStream()
    {
        // Arrange
        var request = new NotificationBroadcastRequest { Id = "2" };
        var resetEvent = new ManualResetEventSlim(false);

        mockBroadcastStream
            .Setup(x => x.WriteAsync(It.IsAny<NotificationBroadcastRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(() => resetEvent.Set());

        var call = CreateCall(mockBroadcastStream.Object);
        mockClient.Setup(x => x.Broadcast(It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
                   .Returns(call);

        using var service = new NotificationService(mockClient.Object, mockLogger.Object);

        // Act
        await service.BroadcastAsync(request, CancellationToken.None);

        // Assert
        Assert.True(resetEvent.Wait(1000));
        mockBroadcastStream.Verify(x => x.WriteAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendToGroupAsync_ShouldWriteToGrpcStream()
    {
        // Arrange
        var request = new NotificationGroupRequest { Id = "3" };
        var resetEvent = new ManualResetEventSlim(false);

        mockGroupStream
            .Setup(x => x.WriteAsync(It.IsAny<NotificationGroupRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(() => resetEvent.Set());

        var call = CreateCall(mockGroupStream.Object);
        mockClient.Setup(x => x.SendToGroup(It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
                   .Returns(call);

        using var service = new NotificationService(mockClient.Object, mockLogger.Object);

        // Act
        await service.SendToGroupAsync(request, CancellationToken.None);

        // Assert
        Assert.True(resetEvent.Wait(1000));
        mockGroupStream.Verify(x => x.WriteAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleStream_OnRpcException_ShouldLogAndRetryThenCancel()
    {
        // Arrange
        mockClient.Setup(x => x.SendToUser(It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
                   .Throws(new RpcException(new Status(StatusCode.Unavailable, "Server down")));

        var service = new NotificationService(mockClient.Object, mockLogger.Object);

        // Act
        await Task.Delay(100);

        await service.DisposeAsync();

        // Assert
        mockLogger.VerifyLogging("gRPC Error in User Stream", LogLevel.Error, Times.AtLeastOnce());
    }

    [Fact]
    public async Task HandleStream_OnGenericException_ShouldLogAndRetryThenCancel()
    {
        // Arrange
        mockClient.Setup(x => x.Broadcast(It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
                   .Throws(new Exception("Generic failure"));

        var service = new NotificationService(mockClient.Object, mockLogger.Object);

        // Act
        await Task.Delay(100);
        service.Dispose();

        // Assert
        mockLogger.VerifyLogging("Unexpected error in Broadcast Stream", LogLevel.Error, Times.AtLeastOnce());
    }

    [Fact]
    public async Task DisposeAsync_ShouldHandleExceptionDuringShutdown()
    {
        // Arrange
        using var service = new NotificationService(mockClient.Object, mockLogger.Object);

        var fieldInfo = typeof(NotificationService).GetField("backgroundTasks", BindingFlags.NonPublic | BindingFlags.Instance);
        var tasks = (List<Task>)fieldInfo!.GetValue(service)!;

        // Añadimos una tarea que ya ha fallado
        tasks.Add(Task.FromException(new Exception("Shutdown Error")));

        // Act
        await service.DisposeAsync();

        // Assert
        mockLogger.VerifyLogging("Error during NotificationService shutdown", LogLevel.Error);
    }

    private AsyncClientStreamingCall<TRequest, Empty> CreateCall<TRequest>(IClientStreamWriter<TRequest> requestStream)
    {
        return new AsyncClientStreamingCall<TRequest, Empty>(
            requestStream,
            Task.FromResult(new Empty()),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => [],
            () => { }
        );
    }
}