using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Vault.Services;

/// <summary>
/// BackgroundService que renueva periódicamente el token de Vault antes de que expire.
/// Intenta RenewSelf; si falla (max_ttl alcanzado), re-autentica recreando el VaultClient.
/// </summary>
public class VaultTokenRenewalService(
    VaultClientProvider clientProvider,
    IOptions<VaultOptions> options,
    ILogger<VaultTokenRenewalService> logger
) : BackgroundService
{
    /// <summary>
    /// Intervalo por defecto para renovar el token (cada 1 hora).
    /// </summary>
    private static readonly TimeSpan DefaultRenewalInterval = TimeSpan.FromHours(1);

    /// <summary>
    /// Ejecuta el ciclo de renovación de token de forma periódica hasta que se solicita la cancelación.
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

        logger.LogInformation("Vault token renewal service started. Renewal interval: {Interval}.", DefaultRenewalInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(DefaultRenewalInterval, stoppingToken);

                if (stoppingToken.IsCancellationRequested)
                    break;

                await RenewOrReAuthenticateAsync(vaultOptions);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error renewing Vault token. Will retry in {Interval}.", DefaultRenewalInterval);
            }
        }

        logger.LogInformation("Vault token renewal service stopped.");
    }

    /// <summary>
    /// Intenta renovar el token actual. Si falla, re-autentica creando un nuevo VaultClient.
    /// </summary>
    /// <param name="vaultOptions">Opciones de configuración de Vault.</param>
    /// <returns>Tarea que representa la operación de renovación.</returns>
    private async Task RenewOrReAuthenticateAsync(VaultOptions vaultOptions)
    {
        try
        {
            var result = await clientProvider.Client.V1.Auth.Token.RenewSelfAsync();

            logger.LogInformation(
                "Vault token renewed successfully. New TTL: {TTL}s, Accessor: {Accessor}.",
                result.LeaseDurationSeconds,
                result.ClientTokenAccessor);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Token self-renewal failed. Attempting full re-authentication.");
            ReAuthenticate(vaultOptions);
        }
    }

    /// <summary>
    /// Recrea el VaultClient con credenciales frescas cuando la renovación del token falla.
    /// </summary>
    /// <param name="vaultOptions">Opciones de configuración de Vault.</param>
    private void ReAuthenticate(VaultOptions vaultOptions)
    {
        clientProvider.ReAuthenticate(vaultOptions);
        logger.LogInformation("Vault client re-authenticated successfully using {AuthType}.", vaultOptions.TypeAuth);
    }
}
