using System.Threading.Channels;
using CodeDesignPlus.Net.gRpc.Clients.Services.Notification;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Notifications;

/// <summary>
/// Cliente del canal efimero.
/// </summary>
/// <remarks>
/// Encola y devuelve de inmediato: el emisor esta en mitad de su propia operacion de negocio y un push
/// no puede hacerle esperar ni tumbarle la transaccion.
/// </remarks>
public class LiveChannelService : ILiveChannelGrpc, IDisposable, IAsyncDisposable
{
    private readonly Channel<LiveUserPush> userChannel = Channel.CreateUnbounded<LiveUserPush>();
    private readonly Channel<LiveGroupPush> groupChannel = Channel.CreateUnbounded<LiveGroupPush>();

    private readonly CancellationTokenSource cts = new();
    private readonly List<Task> backgroundTasks;
    private readonly ILogger<LiveChannelService> logger;

    /// <summary>Inicializa el cliente y levanta una tarea de fondo por stream.</summary>
    /// <param name="client">El cliente gRPC generado del servicio <c>LiveChannel</c>.</param>
    /// <param name="logger">El registro de eventos del servicio.</param>
    public LiveChannelService(LiveChannel.LiveChannelClient client, ILogger<LiveChannelService> logger)
    {
        this.logger = logger;

        var pump = new StreamPump<PushAck>(logger, "LiveChannel", ack => (ack.Success, ack.Message));

        backgroundTasks =
        [
            Task.Run(() => pump.RunAsync(userChannel.Reader, token => client.PushToUser(cancellationToken: token), cts.Token)),
            Task.Run(() => pump.RunAsync(groupChannel.Reader, token => client.PushToGroup(cancellationToken: token), cts.Token))
        ];
    }

    /// <inheritdoc/>
    public async Task PushToUserAsync(LiveUserPush request, CancellationToken cancellationToken)
        => await userChannel.Writer.WriteAsync(request, cancellationToken);

    /// <inheritdoc/>
    public async Task PushToGroupAsync(LiveGroupPush request, CancellationToken cancellationToken)
        => await groupChannel.Writer.WriteAsync(request, cancellationToken);

    /// <summary>Cierra las colas y espera a que las tareas de fondo terminen.</summary>
    public async ValueTask DisposeAsync()
    {
        await cts.CancelAsync();

        try
        {
            await Task.WhenAll(backgroundTasks);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during LiveChannelService shutdown");
        }

        cts.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
