using System;
using CodeDesignPlus.Net.xUnit.Helpers.ObservabilityContainer;

namespace CodeDesignPlus.Net.Logger.Test.Helpers;

[CollectionDefinition(ObservabilityCollectionFixture.Collection)]
public class ObservabilityCollectionDefinition: ICollectionFixture<ObservabilityCollectionFixture>
{

}
