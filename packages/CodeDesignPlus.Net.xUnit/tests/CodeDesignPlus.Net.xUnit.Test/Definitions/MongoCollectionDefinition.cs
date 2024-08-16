using CodeDesignPlus.Net.xUnit.Helpers.MongoContainer;

namespace CodeDesignPlus.Net.xUnit.Test.Definitions;


[CollectionDefinition(MongoCollectionFixture.Collection)]
public class MongoCollectionDefinition : ICollectionFixture<MongoCollectionFixture>
{
}