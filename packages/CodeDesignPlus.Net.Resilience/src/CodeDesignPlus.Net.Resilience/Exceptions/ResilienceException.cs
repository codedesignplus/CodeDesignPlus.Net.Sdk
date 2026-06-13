namespace CodeDesignPlus.Net.Resilience.Exceptions;

public class ResilienceException : Exception
{
    public ResilienceException(string message) : base(message) { }

    public ResilienceException(string message, Exception innerException) : base(message, innerException) { }
}
