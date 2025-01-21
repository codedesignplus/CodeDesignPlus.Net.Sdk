using CodeDesignPlus.Net.xUnit.Containers.RedisContainer;

namespace CodeDesignPlus.Net.Redis.Cache.Test.Helpers;

[CollectionDefinition(RedisCollectionFixture.Collection)]
public class RedisCollectionDefinition : ICollectionFixture<RedisCollectionFixture>
{
}