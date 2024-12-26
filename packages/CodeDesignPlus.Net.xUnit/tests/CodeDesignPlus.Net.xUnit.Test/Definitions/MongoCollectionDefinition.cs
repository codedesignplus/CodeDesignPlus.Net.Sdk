using CodeDesignPlus.Net.xUnit.Containers.MongoContainer;

namespace CodeDesignPlus.Net.xUnit.Test.Definitions;


[CollectionDefinition(MongoCollectionFixture.Collection)]
public class MongoCollectionDefinition : ICollectionFixture<MongoCollectionFixture>
{
}