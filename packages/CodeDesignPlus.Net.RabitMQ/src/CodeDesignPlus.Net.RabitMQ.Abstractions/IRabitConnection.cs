using RabbitMQ.Client;

namespace CodeDesignPlus.Net.RabitMQ.Abstractions
{
    public interface IRabitConnection
    {
        IConnection Connection { get; }
    }
}
