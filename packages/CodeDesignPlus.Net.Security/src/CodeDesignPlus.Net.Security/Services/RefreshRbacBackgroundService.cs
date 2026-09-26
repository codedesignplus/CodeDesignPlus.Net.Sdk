using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Security.Services;

/// <summary>
/// Service to refresh the role-based access control of the application.
/// </summary>
/// <param name="logger">The logger service.</param>
/// <param name="rbacService">The service to manage the role-based access control of the application.</param>
/// <param name="options">The security options of the application.</param>
public class RefreshRbacBackgroundService(ILogger<RefreshRbacBackgroundService> logger, IRbac rbacService, IOptions<SecurityOptions> options) : BackgroundService
{
    /// <summary>
    /// The service to manage the role-based access control of the application.
    /// </summary>
    /// <param name="stoppingToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Return a <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <summary>
    /// La espera antes de reintentar tras un error al cargar los permisos.
    /// </summary>
    internal static TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() => logger.LogInformation("RefreshRbacBackgroundService is stopping"));

        // El try va DENTRO del bucle: un error de gRPC (ms-rbac reiniciandose, por ejemplo) no puede parar el refresco
        // para siempre. Tras un error se reintenta antes que el intervalo normal, porque sin permisos cargados se niega
        // todo (plan 031 de pendings).
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = TimeSpan.FromMinutes(options.Value.RefreshRbacInterval);

            try
            {
                await rbacService.LoadRbacAsync(stoppingToken);

                logger.LogDebug("The RBAC was refreshed successfully");
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while refreshing the RBAC | {Message}", ex.Message);

                delay = RetryDelay < delay ? RetryDelay : delay;
            }

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        logger.LogInformation("RefreshRbacBackgroundService is running");
    }
}
