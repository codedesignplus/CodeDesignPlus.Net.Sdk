using CodeDesignPlus.Net.xUnit.Helpers;

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
            Acks = "all",
            BatchSize = 4096,
            LingerMs = 5,
            CompressionType = "snappy",
            NameMicroservice = "MicroserviceTest"
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }
}
