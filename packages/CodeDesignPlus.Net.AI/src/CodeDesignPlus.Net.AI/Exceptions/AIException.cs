namespace CodeDesignPlus.Net.AI.Exceptions;

public class AIException : Exception
{
    public AIException(string message) : base(message) { }
    public AIException(string message, Exception innerException) : base(message, innerException) { }
}
