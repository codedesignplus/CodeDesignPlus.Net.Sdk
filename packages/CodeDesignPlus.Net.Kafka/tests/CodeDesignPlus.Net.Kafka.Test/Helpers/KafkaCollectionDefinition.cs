using CodeDesignPlus.Net.xUnit.Containers.KafkaContainer;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers;

[CollectionDefinition(KafkaCollectionFixture.Collection)]
public class KafkaCollectionDefinition : ICollectionFixture<KafkaCollectionFixture>
{
}