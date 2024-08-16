using System;
using CodeDesignPlus.Net.xUnit.Helpers.KafkaContainer;

namespace CodeDesignPlus.Net.xUnit.Test.Definitions;

[CollectionDefinition(KafkaCollectionFixture.Collection)]
public class KafkaCollectionDefinition : ICollectionFixture<KafkaCollectionFixture>
{
}