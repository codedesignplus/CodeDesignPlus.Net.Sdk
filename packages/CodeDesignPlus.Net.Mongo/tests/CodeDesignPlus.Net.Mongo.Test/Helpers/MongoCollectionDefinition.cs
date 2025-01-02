using System;
using CodeDesignPlus.Net.xUnit.Containers.MongoContainer;

namespace CodeDesignPlus.Net.Mongo.Test.Helpers;

[CollectionDefinition(MongoCollectionFixture.Collection)]
public class MongoCollectionDefinition : ICollectionFixture<MongoCollectionFixture>
{
}