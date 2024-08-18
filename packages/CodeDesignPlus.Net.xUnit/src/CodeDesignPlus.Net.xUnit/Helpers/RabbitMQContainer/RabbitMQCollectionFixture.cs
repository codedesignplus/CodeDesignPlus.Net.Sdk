using System;

namespace CodeDesignPlus.Net.xUnit.Helpers.RabbitMQContainer;

public sealed class RabbitMQCollectionFixture : IDisposable
{
    public const string Collection = "RabbitMQ Collection";
    public  RabbitMQContainer Container { get; }

    public RabbitMQCollectionFixture()
    {
        this.Container = new RabbitMQContainer();

        Thread.Sleep(10000);
    }

    public void Dispose()
    {
        this.Container.StopInstance();
    }
}
