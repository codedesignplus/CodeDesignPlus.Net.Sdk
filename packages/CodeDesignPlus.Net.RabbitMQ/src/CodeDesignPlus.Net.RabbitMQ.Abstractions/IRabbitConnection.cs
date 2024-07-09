using RabbitMQ.Client;

namespace CodeDesignPlus.Net.RabbitMQ.Abstractions
{
    public interface IRabbitConnection : IDisposable
    {
        IConnection Connection { get; }
    }
}
