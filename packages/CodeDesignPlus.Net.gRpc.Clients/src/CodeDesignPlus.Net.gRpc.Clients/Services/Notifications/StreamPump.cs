using System.Threading.Channels;
using Grpc.Core;

namespace CodeDesignPlus.Net.gRpc.Clients.Services.Notifications;

/// <summary>
/// Bombea una cola en memoria hacia un stream bidireccional de gRPC, reconectando cuando se cae.
/// </summary>
/// <remarks>
/// Es el patron que ya usaba <see cref="NotificationService"/> repetido tres veces; aqui se saca a un
/// solo sitio porque los dos clientes nuevos suman tres streams mas y el patron tiene una parte facil de
/// equivocar: la tarea que lee el stream de respuesta tiene que correr <b>a la vez</b> que la escritura,
/// no despues, o el emisor se bloquea cuando el servidor llena su ventana.
/// <para>
/// La cola vive en memoria: si el pod emisor se reinicia, lo encolado se pierde en silencio. Es
/// aceptable para el canal efimero, y es exactamente por lo que un aviso durable <b>no</b> puede existir
/// solo como un mensaje emitido — el estado consultable es la fuente de verdad.
/// </para>
/// </remarks>
/// <typeparam name="TResponse">El tipo del acuse que devuelve el servidor.</typeparam>
internal sealed class StreamPump<TResponse>
{
    private readonly ILogger logger;
    private readonly string streamName;
    private readonly Func<TResponse, (bool Success, string Message)> readAck;

    internal StreamPump(ILogger logger, string streamName, Func<TResponse, (bool, string)> readAck)
    {
        this.logger = logger;
        this.streamName = streamName;
        this.readAck = readAck;
    }

    /// <summary>Escribe todo lo que llegue a la cola, reabriendo el stream a los 5s si se cae.</summary>
    /// <remarks>
    /// El mensaje que se estaba escribiendo cuando el stream falló ya salió de la cola: se guarda y es lo primero que se escribe
    /// al reconectar. Antes se perdía, y era justo el primero después de un rato sin tráfico, cuando el stream se había caído
    /// sin que nadie lo notara. Reenviarlo no duplica un aviso durable: la bandeja lo reconoce por su id.
    /// </remarks>
    internal async Task RunAsync<TRequest>(
        ChannelReader<TRequest> channelReader,
        Func<CancellationToken, AsyncDuplexStreamingCall<TRequest, TResponse>> callFactory,
        CancellationToken token)
    {
        var unsent = new Queue<TRequest>(1);

        while (!token.IsCancellationRequested)
        {
            try
            {
                using var call = callFactory(token);

                var responseTask = Task.Run(async () =>
                {
                    await foreach (var response in call.ResponseStream.ReadAllAsync(token))
                    {
                        var (success, message) = readAck(response);

                        if (!success)
                            logger.LogWarning("{StreamName} server reported failure: {Message}", streamName, message);
                    }
                }, token);

                while (unsent.TryPeek(out var pending))
                {
                    await call.RequestStream.WriteAsync(pending, token);

                    unsent.Dequeue();
                }

                await foreach (var request in channelReader.ReadAllAsync(token))
                {
                    unsent.Enqueue(request);

                    await call.RequestStream.WriteAsync(request, token);

                    unsent.Dequeue();
                }

                await call.RequestStream.CompleteAsync();
                await responseTask;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in {StreamName}. Reconnecting in 5s...", streamName);

                try { await Task.Delay(5000, token); }
                catch (OperationCanceledException) { break; }
            }
        }
    }
}
