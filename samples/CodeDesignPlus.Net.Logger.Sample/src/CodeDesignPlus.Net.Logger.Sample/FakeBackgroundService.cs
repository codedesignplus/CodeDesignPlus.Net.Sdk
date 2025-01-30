using System;

namespace CodeDesignPlus.Net.Logger.Sample;

public class FakeBackgroundService(ILogger<FakeBackgroundService> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("The FakeBackgroundService is running.");

        return Task.CompletedTask;
    }
}
