using System;
using CodeDesignPlus.Net.xUnit.Containers.RedisContainer;

namespace CodeDesignPlus.Net.Redis.Test.Helpers;

[CollectionDefinition(RedisCollectionFixture.Collection)]
public class RedisCollectionDefinition : ICollectionFixture<RedisCollectionFixture>
{
}