using CodeDesignPlus.Net.xUnit.Helpers.VaultContainer;

namespace CodeDesignPlus.Net.Vault.Test.Helpers;

[CollectionDefinition(VaultCollectionFixture.Collection)]
public class VaultContainerDefinition : ICollectionFixture<VaultCollectionFixture>
{
}