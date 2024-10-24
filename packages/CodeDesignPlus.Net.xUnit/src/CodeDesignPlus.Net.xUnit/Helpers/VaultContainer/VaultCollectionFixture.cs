using System.Net;

namespace CodeDesignPlus.Net.xUnit.Helpers.VaultContainer;
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