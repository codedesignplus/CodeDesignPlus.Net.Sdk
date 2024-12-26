using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.Kafka.Test.Options;

public class KafkaOptionsTest
{
    [Fact]
    public void KafkaOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new KafkaOptions()
        {
            BootstrapServers = "localhost:9092",
            Acks = Confluent.Kafka.Acks.All ,
            BatchSize = 4096,
            LingerMs = 5,
            CompressionType = Confluent.Kafka.CompressionType.Gzip ,
            NameMicroservice = "MicroserviceTest"
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }
}
