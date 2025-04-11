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
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() => logger.LogInformation("RefreshRbacBackgroundService is stopping"));

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await rbacService.LoadRbacAsync(stoppingToken);

                logger.LogDebug("The RBAC was refreshed successfully");

                await Task.Delay(TimeSpan.FromMinutes(options.Value.RefreshRbacInterval), stoppingToken);
            }
        } 
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while refreshing the RBAC | {Message}", ex.Message);
        }

        logger.LogInformation("RefreshRbacBackgroundService is running");
    }
}
