namespace CodeDesignPlus.Net.Resilience.Abstractions.Options;

public class ResilienceOptions
{
    public bool Enable { get; set; } = true;

    public int MaxRetryAttempts { get; set; } = 3;
    public double RetryBaseDelaySeconds { get; set; } = 1.0;
    public double RetryMaxDelaySeconds { get; set; } = 10.0;

    public double CircuitBreakerFailureRatio { get; set; } = 0.5;
    public int CircuitBreakerMinimumThroughput { get; set; } = 10;
    public double CircuitBreakerBreakDurationSeconds { get; set; } = 30.0;
    public double CircuitBreakerSamplingDurationSeconds { get; set; } = 60.0;

    public double TimeoutSeconds { get; set; } = 15.0;

    public bool RetryOnSoftErrors { get; set; } = true;
    public int MaxSoftErrorRetryAttempts { get; set; } = 6;
    public double SoftErrorRetryBaseDelaySeconds { get; set; } = 1.0;
    public double SoftErrorMaxDelaySeconds { get; set; } = 2.0;
}
