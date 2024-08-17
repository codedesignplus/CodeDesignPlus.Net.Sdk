namespace CodeDesignPlus.Net.xUnit.Helpers.KafkaContainer;

public sealed class KafkaCollectionFixture : IDisposable
{
    public const string Collection = "Kafka Collection";
    public  KafkaContainer Container { get; }

    public KafkaCollectionFixture()
    {
        this.Container = new KafkaContainer();

        Thread.Sleep(10000);
    }

    public void Dispose()
    {
        this.Container.StopInstance();
    }
}
