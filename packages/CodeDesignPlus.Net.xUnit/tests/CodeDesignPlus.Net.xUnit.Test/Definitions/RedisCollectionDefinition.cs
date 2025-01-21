using CodeDesignPlus.Net.xUnit.Containers.RedisContainer;

namespace CodeDesignPlus.Net.xUnit.Test.Definitions;


[CollectionDefinition(RedisCollectionFixture.Collection)]
public class RedisCollectionDefinition : ICollectionFixture<RedisCollectionFixture>
{
}