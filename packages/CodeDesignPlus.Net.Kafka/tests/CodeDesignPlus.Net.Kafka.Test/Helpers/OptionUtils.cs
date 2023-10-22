namespace CodeDesignPlus.Net.Kafka.Test.Helpers;

public static class OptionUtils
{
    public static readonly KafkaOptions KafkaOptions = new()
    {
        Enable = true,
        BootstrapServers = "localhost:9092",
        Acks = "all",
        BatchSize = 4096,
        LingerMs = 5,
        CompressionType = "snappy",
        NameMicroservice = "Microservice.Test"
    };
}
