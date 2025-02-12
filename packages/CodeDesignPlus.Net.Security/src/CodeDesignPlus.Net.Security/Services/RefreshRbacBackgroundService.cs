using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Security.Services;

public class RefreshRbacBackgroundService(ILogger<RefreshRbacBackgroundService> logger, IRbac rbacService, IOptions<SecurityOptions> options) : BackgroundService
{
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
