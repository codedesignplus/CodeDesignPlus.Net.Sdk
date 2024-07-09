using RabbitMQ.Client;

namespace CodeDesignPlus.Net.RabitMQ.Abstractions
{
    public interface IRabitConnection: IDisposable
    {
        IConnection Connection { get; }
    }
}
