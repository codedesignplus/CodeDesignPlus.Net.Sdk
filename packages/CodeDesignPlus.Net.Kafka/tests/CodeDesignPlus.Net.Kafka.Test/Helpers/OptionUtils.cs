using CodeDesignPlus.Net.Core.Abstractions.Options;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers;

public static class OptionUtils
{
    public static readonly CoreOptions CoreOptions = new()
    {
        AppName = "xunit-kafka",
        Description = "The xunit test for the kafka library",
        Version = "v1",
        Business = "CodeDesignPlus",
        Contact = new()
        {
            Name = "CodeDesignPlus",
            Email = "CodeDesignPlus@outlook.com"
        }
    };

    public static readonly KafkaOptions KafkaOptions = new()
    {
        Enable = true,
        BootstrapServers = "localhost:29092",
        Acks = "all",
        BatchSize = 4096,
        LingerMs = 5,
        CompressionType = "snappy",
        NameMicroservice = "Microservice.Test"
    };
}
