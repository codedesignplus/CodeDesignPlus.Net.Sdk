using Microsoft.Extensions.Http.Resilience;

namespace CodeDesignPlus.Net.Resilience.Extensions;

public static class HttpClientBuilderExtensions
{
    public static IHttpClientBuilder AddResiliencePolicies(
        this IHttpClientBuilder builder,
        ResilienceOptions options,
        string pipelineName = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        if (!options.Enable)
            return builder;

        var name = pipelineName ?? $"{builder.Name}-transport";

        builder.AddResilienceHandler(name, resilienceBuilder =>
        {
            resilienceBuilder
                .AddTimeout(TimeSpan.FromSeconds(options.TimeoutSeconds))
                .AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = options.MaxRetryAttempts,
                    Delay = TimeSpan.FromSeconds(options.RetryBaseDelaySeconds),
                    MaxDelay = TimeSpan.FromSeconds(options.RetryMaxDelaySeconds),
                    BackoffType = DelayBackoffType.Exponential
                })
                .AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                {
                    FailureRatio = options.CircuitBreakerFailureRatio,
                    MinimumThroughput = options.CircuitBreakerMinimumThroughput,
                    BreakDuration = TimeSpan.FromSeconds(options.CircuitBreakerBreakDurationSeconds),
                    SamplingDuration = TimeSpan.FromSeconds(options.CircuitBreakerSamplingDurationSeconds)
                });
        });

        return builder;
    }
}
