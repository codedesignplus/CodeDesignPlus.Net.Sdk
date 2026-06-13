namespace CodeDesignPlus.Net.Resilience.Factories;

public static class SoftErrorPipelineFactory
{
    public static ResiliencePipeline Create(ResilienceOptions options, ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        if (!options.Enable || !options.RetryOnSoftErrors)
            return ResiliencePipeline.Empty;

        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = options.MaxSoftErrorRetryAttempts,
                Delay = TimeSpan.FromSeconds(options.SoftErrorRetryBaseDelaySeconds),
                MaxDelay = TimeSpan.FromSeconds(options.SoftErrorMaxDelaySeconds),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = new PredicateBuilder().Handle<ResilienceSoftErrorException>()
            })
            .ConfigureTelemetry(loggerFactory)
            .Build();
    }
}
