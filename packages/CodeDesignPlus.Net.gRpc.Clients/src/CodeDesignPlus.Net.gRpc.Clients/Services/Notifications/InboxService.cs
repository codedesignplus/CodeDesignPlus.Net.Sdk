using System.Threading.Channels;
using CodeDesignPlus.Net.gRpc.Clients.Services.Notification;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Notifications;

/// <summary>
/// Cliente de los avisos durables.
/// </summary>
/// <remarks>
/// Encola igual que el canal efimero, y por la misma razon: el emisor no puede quedarse esperando a
/// ms-notification en mitad de su transaccion.
/// <para>
/// La cola es en memoria, asi que un reinicio del pod emisor pierde lo encolado. Por eso el aviso
/// <b>nunca</b> es la fuente de verdad: el estado consultable del micro dueño lo es, y el aviso solo
/// acelera. Con el <c>Id</c> que genera el emisor, una reentrega del bus no duplica el aviso.
/// </para>
/// </remarks>
public class InboxService : IInboxGrpc, IDisposable, IAsyncDisposable
{
    private readonly Channel<NotificationRequest> channel = Channel.CreateUnbounded<NotificationRequest>();

    private readonly CancellationTokenSource cts = new();
    private readonly Task backgroundTask;
    private readonly ILogger<InboxService> logger;
    private readonly Inbox.InboxClient client;

    /// <summary>Inicializa el cliente y levanta la tarea de fondo del stream.</summary>
    /// <param name="client">El cliente gRPC generado del servicio <c>Inbox</c>.</param>
    /// <param name="logger">El registro de eventos del servicio.</param>
    public InboxService(Inbox.InboxClient client, ILogger<InboxService> logger)
    {
        this.logger = logger;
        this.client = client;

        var pump = new StreamPump<NotifyAck>(logger, "Inbox", ack => (ack.Success, ack.Message));

        backgroundTask = Task.Run(() => pump.RunAsync(channel.Reader, token => client.Notify(cancellationToken: token), cts.Token));
    }

    /// <inheritdoc/>
    public async Task NotifyAsync(NotificationRequest request, CancellationToken cancellationToken)
        => await channel.Writer.WriteAsync(request, cancellationToken);

    /// <inheritdoc/>
    /// <remarks>
    /// Va directo y no por la cola del stream: si llega antes que el aviso que cierra, el servicio guarda el cierre y el aviso
    /// nace cerrado, asi que el orden no importa.
    /// </remarks>
    public async Task<bool> ResolveAsync(ResolveRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var ack = await client.ResolveAsync(request, cancellationToken: cancellationToken);

            if (!ack.Success)
            {
                logger.LogWarning("Inbox resolve of {Kinds} for {Module}/{AggregateId} was rejected: {Message}", string.Join(",", request.Kinds), request.Resource?.Module, request.Resource?.AggregateId, ack.Message);
            }

            return ack.Success;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Inbox resolve of {Kinds} for {Module}/{AggregateId} failed", string.Join(",", request.Kinds), request.Resource?.Module, request.Resource?.AggregateId);

            return false;
        }
    }

    /// <summary>Cierra la cola y espera a que la tarea de fondo termine.</summary>
    public async ValueTask DisposeAsync()
    {
        await cts.CancelAsync();

        try
        {
            await backgroundTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during InboxService shutdown");
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
