namespace CodeDesignPlus.Net.Resilience.Abstractions;

public class ResilienceSoftErrorException(string message, string responseBody) : Exception(message)
{
    public string ResponseBody { get; } = responseBody;
}
