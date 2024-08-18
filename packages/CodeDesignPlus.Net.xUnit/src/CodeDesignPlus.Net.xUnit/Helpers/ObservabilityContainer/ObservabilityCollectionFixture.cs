namespace CodeDesignPlus.Net.xUnit.Helpers.ObservabilityContainer;

public sealed class ObservabilityCollectionFixture : IDisposable
{
    public const string Collection = "Observability Collection";
    public  ObservabilityContainer Container { get; }

    public ObservabilityCollectionFixture()
    {
        this.Container = new ObservabilityContainer();
    }

    public void Dispose()
    {
        this.Container.StopInstance();
    }
}
