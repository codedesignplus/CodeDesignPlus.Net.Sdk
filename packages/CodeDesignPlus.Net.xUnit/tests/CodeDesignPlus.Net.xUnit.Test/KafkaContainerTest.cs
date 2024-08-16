using CodeDesignPlus.Net.xUnit.Helpers.KafkaContainer;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace CodeDesignPlus.Net.xUnit.Test;


[Collection(KafkaCollectionFixture.Collection)]
public class KafkaContainerTest(KafkaCollectionFixture kafkaCollectionFixture)
{
    [Fact]
    public async Task Initialize_Publish_Consumer()
    {
        // Arrange
        var producerConfig = new ProducerConfig { BootstrapServers = kafkaCollectionFixture.Container.BrokerList };
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaCollectionFixture.Container.BrokerList,
            GroupId = "test-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var topic = "test-topic";
        var testMessage = "test-message";

        // Act
        string? consumedMessage = null;

        // Produce the message
        using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
        {
            await producer.ProduceAsync(topic, new Message<Null, string> { Value = testMessage });
            producer.Flush(TimeSpan.FromSeconds(10));
        }

        using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
        {
            consumer.Subscribe(topic);
            var consumeResult = consumer.Consume();
            if (consumeResult?.Message?.Value != null)
            {
                consumedMessage = consumeResult.Message.Value;
            }
        }

        // Assert
        Assert.Equal(testMessage, consumedMessage);
    }
}
