using CodeDesignPlus.Net.xUnit.Containers.KafkaContainer;

namespace CodeDesignPlus.Net.xUnit.Test.Definitions;

[CollectionDefinition(KafkaCollectionFixture.Collection)]
public class KafkaCollectionDefinition : ICollectionFixture<KafkaCollectionFixture>
{
}