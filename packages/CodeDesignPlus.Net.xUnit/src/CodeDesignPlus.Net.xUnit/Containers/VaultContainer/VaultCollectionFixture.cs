namespace CodeDesignPlus.Net.xUnit.Containers.VaultContainer;

public sealed class VaultCollectionFixture : IDisposable
{
    public const string Collection = "Vault Collection";

    public VaultContainer Container { get; }

    public VaultCollectionFixture()
    {
        this.Container = new VaultContainer();
    }

    public void Dispose()
    {
        this.Container.StopInstance();
    }
}