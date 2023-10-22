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
            CompressionType = "snappy"
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    // [Fact]
    // public void KafkaOptions_NameIsRequired_FailedValidation()
    // {
    //     // Arrange
    //     var options = new KafkaOptions();

    //     // Act
    //     var results = options.Validate();

    //     // Assert
    //     Assert.Contains(results, x => x.ErrorMessage == "The Name field is required.");
    // }

    // [Fact]
    // public void KafkaOptions_EmailIsRequired_FailedValidation()
    // {
    //     // Arrange
    //     var options = new KafkaOptions()
    //     {
    //         Enable = true,
    //         Name = Guid.NewGuid().ToString(),
    //         Email = null
    //     };

    //     // Act
    //     var results = options.Validate();

    //     // Assert
    //     Assert.Contains(results, x => x.ErrorMessage == "The Email field is required.");
    // }

    // [Fact]
    // public void KafkaOptions_EmailIsInvalid_FailedValidation()
    // {
    //     // Arrange
    //     var options = new KafkaOptions()
    //     {
    //         Enable = true,
    //         Name = Guid.NewGuid().ToString(),
    //         Email = "asdfasdfsdfgs"
    //     };

    //     // Act
    //     var results = options.Validate();

    //     // Assert
    //     Assert.Contains(results, x => x.ErrorMessage == "The Email field is not a valid e-mail address.");
    // }
}
