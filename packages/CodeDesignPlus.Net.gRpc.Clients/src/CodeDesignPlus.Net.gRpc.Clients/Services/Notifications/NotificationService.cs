using System.Threading.Channels;
using Grpc.Core;
using CodeDesignPlus.Net.gRpc.Clients.Services.Notification;
using Google.Protobuf.WellKnownTypes;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Notifications;

public class NotificationService : INotificationGrpc, IDisposable, IAsyncDisposable
{
    private readonly Notifier.NotifierClient client;
    private readonly ILogger<NotificationService> logger;

    private readonly Channel<NotificationUserRequest> userChannel;
    private readonly Channel<NotificationBroadcastRequest> broadcastChannel;
    private readonly Channel<NotificationGroupRequest> groupChannel;

    private readonly CancellationTokenSource cts;
    private readonly List<Task> backgroundTasks;

    public NotificationService(Notifier.NotifierClient client, ILogger<NotificationService> logger)
    {
        this.client = client;
        this.logger = logger;
        cts = new CancellationTokenSource();

        userChannel = Channel.CreateUnbounded<NotificationUserRequest>();
        broadcastChannel = Channel.CreateUnbounded<NotificationBroadcastRequest>();
        groupChannel = Channel.CreateUnbounded<NotificationGroupRequest>();

        backgroundTasks =
        [
            Task.Run(() => HandleStreamAsync(userChannel.Reader,token => this.client.SendToUser(cancellationToken: token), "User Stream", cts.Token)),

            Task.Run(() => HandleStreamAsync(broadcastChannel.Reader,token => this.client.Broadcast(cancellationToken: token), "Broadcast Stream", cts.Token)),

            Task.Run(() => HandleStreamAsync(groupChannel.Reader,token => this.client.SendToGroup(cancellationToken: token), "Group Stream", cts.Token))
        ];
    }

    public async Task SendToUserAsync(NotificationUserRequest request, CancellationToken cancellationToken)
    {
        await userChannel.Writer.WriteAsync(request, cancellationToken);
    }

    public async Task BroadcastAsync(NotificationBroadcastRequest request, CancellationToken cancellationToken)
    {
        await broadcastChannel.Writer.WriteAsync(request, cancellationToken);
    }

    public async Task SendToGroupAsync(NotificationGroupRequest request, CancellationToken cancellationToken)
    {
        await groupChannel.Writer.WriteAsync(request, cancellationToken);
    }

    private async Task HandleStreamAsync<TRequest>(ChannelReader<TRequest> channelReader, Func<CancellationToken, AsyncClientStreamingCall<TRequest, Empty>> callFactory, string streamName, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                using var call = callFactory(token);

                await foreach (var request in channelReader.ReadAllAsync(token))
                {
                    await call.RequestStream.WriteAsync(request, token);
                }

                await call.RequestStream.CompleteAsync();
                await call;
            }
            catch (RpcException ex)
            {
                logger.LogError(ex, "gRPC Error in {StreamName}. Reconnecting in 5s...", streamName);
                try
                {
                    await Task.Delay(5000, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in {StreamName}. Reconnecting in 5s...", streamName);

                try
                {
                    await Task.Delay(5000, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await cts.CancelAsync();

        try
        {
            await Task.WhenAll(backgroundTasks);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during NotificationService shutdown");
        }

        cts.Cancel();
        cts.Dispose();

        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}