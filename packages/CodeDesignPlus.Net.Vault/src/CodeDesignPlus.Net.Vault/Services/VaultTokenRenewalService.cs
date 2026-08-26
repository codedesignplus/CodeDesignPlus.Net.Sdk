using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Vault.Services;

/// <summary>
/// BackgroundService que mantiene viva la autenticacion contra Vault reautenticandose ANTES de
/// que el token caduque.
///
/// POR QUE SE REAUTENTICA EN VEZ DE RENOVAR. La version anterior llamaba a RenewSelf y, si
/// fallaba, reautenticaba como plan B. En Kubernetes ese plan B era el camino normal, no la
/// excepcion, y ademas fallaba siempre por el medio:
///
///   - El token proyectado del ServiceAccount caduca a los ~3607 s.
///   - El lease del token de Vault dura 3600 s.
///   - El intervalo estaba fijado a 1 hora exacta.
///
/// Los tres relojes vencian a la vez, asi que al despertar el token de Vault ya estaba muerto,
/// VaultSharp intentaba reloguearse con el JWT que habia capturado al arrancar —tambien muerto—
/// y Vault contestaba 403 "token is expired". Luego el plan B leia el fichero de nuevo y todo
/// seguia. Funcionaba, pero dejaba un error por pod y por hora: 884 de 916 llamadas a Vault
/// marcadas como fallo, suficiente para enterrar los errores de verdad de la plataforma.
///
/// Reautenticarse SIEMPRE elimina la causa entera: VaultClientFactory.Create lee el JWT del disco
/// en cada llamada, asi que el credencial nunca es viejo. Y hacerlo a una fraccion del TTL REAL
/// —no de una constante— garantiza que se hace con el token todavia vivo.
///
/// El coste es un token nuevo por ciclo. Como el viejo caduca solo, coexisten dos como mucho.
/// </summary>
public class VaultTokenRenewalService(
    VaultClientProvider clientProvider,
    IOptions<VaultOptions> options,
    ILogger<VaultTokenRenewalService> logger
) : BackgroundService
{
    /// <summary>
    /// Fraccion del TTL restante tras la cual se reautentica. A 2/3 quedan dos intentos completos
    /// antes de que el token muera: si uno falla por un corte de red, el siguiente aun llega a
    /// tiempo. Mas cerca de 1 no deja margen; mas cerca de 0 reautentica sin necesidad.
    /// </summary>
    private const double TtlFraction = 2.0 / 3.0;

    /// <summary>
    /// Suelo de espera. Evita un bucle caliente si Vault devolviera un TTL diminuto o cero.
    /// </summary>
    private static readonly TimeSpan MinimumDelay = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Techo de espera. Un token de TTL muy largo no justifica dejar de comprobar durante dias.
    /// </summary>
    private static readonly TimeSpan MaximumDelay = TimeSpan.FromHours(12);

    /// <summary>
    /// Espera cuando no se puede saber el TTL, y tambien entre reintentos tras un fallo.
    /// Deliberadamente corta: si no sabemos cuanto vive el token, conviene volver pronto.
    /// </summary>
    private static readonly TimeSpan FallbackDelay = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Ejecuta el ciclo de reautenticacion hasta que se solicita la cancelacion.
    /// </summary>
    /// <param name="stoppingToken">Token de cancelación para detener el servicio.</param>
    /// <returns>Tarea que representa la ejecución del servicio en segundo plano.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var vaultOptions = options.Value;

        if (!vaultOptions.Enable)
        {
            logger.LogInformation("Vault is disabled. Token renewal service will not run.");
            return;
        }

        logger.LogInformation("Vault token renewal service started using {AuthType}.", vaultOptions.TypeAuth);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var delay = await GetDelayAsync();

                logger.LogInformation("Next Vault re-authentication in {Delay}.", delay);

                await Task.Delay(delay, stoppingToken);

                if (stoppingToken.IsCancellationRequested)
                    break;

                clientProvider.ReAuthenticate(vaultOptions);

                logger.LogInformation("Vault client re-authenticated successfully using {AuthType}.", vaultOptions.TypeAuth);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error re-authenticating against Vault. Will retry in {Delay}.", FallbackDelay);

                try
                {
                    await Task.Delay(FallbackDelay, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        logger.LogInformation("Vault token renewal service stopped.");
    }

    /// <summary>
    /// Calcula cuanto esperar preguntando a Vault el TTL que le queda al token actual.
    /// </summary>
    /// <remarks>
    /// Se pregunta al propio Vault en vez de asumir el TTL porque el valor depende del rol y
    /// puede cambiarse sin tocar el codigo. Fijarlo aqui es justo el error que se esta corrigiendo.
    /// </remarks>
    /// <returns>El tiempo a esperar antes de la siguiente reautenticacion.</returns>
    private async Task<TimeSpan> GetDelayAsync()
    {
        try
        {
            var info = await clientProvider.Client.V1.Auth.Token.LookupSelfAsync();

            var timeToLive = TimeSpan.FromSeconds(info.Data.TimeToLive);

            if (timeToLive <= TimeSpan.Zero)
                return MinimumDelay;

            var delay = timeToLive * TtlFraction;

            if (delay < MinimumDelay)
                return MinimumDelay;

            if (delay > MaximumDelay)
                return MaximumDelay;

            return delay;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not read the Vault token TTL. Falling back to {Delay}.", FallbackDelay);

            return FallbackDelay;
        }
    }
}
