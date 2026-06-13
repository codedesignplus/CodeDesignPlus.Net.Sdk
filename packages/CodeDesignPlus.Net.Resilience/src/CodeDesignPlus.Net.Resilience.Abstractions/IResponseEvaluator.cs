namespace CodeDesignPlus.Net.Resilience.Abstractions;

public interface IResponseEvaluator
{
    bool IsRetryableError(string responseBody);
}
