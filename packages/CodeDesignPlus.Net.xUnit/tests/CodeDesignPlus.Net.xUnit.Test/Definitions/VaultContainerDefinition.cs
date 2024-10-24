using System;
using CodeDesignPlus.Net.xUnit.Helpers.VaultContainer;

namespace CodeDesignPlus.Net.xUnit.Test.Definitions;

[CollectionDefinition(VaultCollectionFixture.Collection)]
public class VaultContainerDefinition : ICollectionFixture<VaultCollectionFixture>
{
}