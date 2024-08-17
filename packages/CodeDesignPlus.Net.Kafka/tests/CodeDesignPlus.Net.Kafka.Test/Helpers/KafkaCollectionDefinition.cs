using CodeDesignPlus.Net.xUnit.Helpers.KafkaContainer;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers;

[CollectionDefinition(KafkaCollectionFixture.Collection)]
public class KafkaCollectionDefinition : ICollectionFixture<KafkaCollectionFixture>
{
}