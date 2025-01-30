using CodeDesignPlus.Net.xUnit.Containers.VaultContainer;

namespace CodeDesignPlus.Net.Vault.Test.Helpers;

[CollectionDefinition(VaultCollectionFixture.Collection)]
public class VaultContainerDefinition : ICollectionFixture<VaultCollectionFixture>
{
}