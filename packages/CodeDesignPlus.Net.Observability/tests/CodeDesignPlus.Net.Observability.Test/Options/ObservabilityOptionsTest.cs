using CodeDesignPlus.Net.xUnit.Helpers;

namespace CodeDesignPlus.Net.Observability.Test.Options;

public class ObservabilityOptionsTest
{
    [Fact]
    public void ObservabilityOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new ObservabilityOptions()
        {
            Enable = true,
            ServerOtel = new Uri("http://localhost:4317"),
            Metrics = new Metrics()
            {
                Enable = true,
                AspNetCore = true
            },
            Trace = new Trace()
            {
                Enable = true,
                AspNetCore = true,
                CodeDesignPlusSdk = true,
                Redis = true,
                Kafka = true,
                SqlClient = true,
                GrpcClient = true
            }
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void ObservabilityOptions_ServerOtelIsRequired_FailedValidation()
    {
        // Arrange
        var options = new ObservabilityOptions()
        {
            Enable = true,
            ServerOtel = null
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The ServerOtel field is required.");
    }

    [Fact]
    public void ObservabilityOptions_IsNotEnable_SuccessValidation()
    {
        // Arrange
        var options = new ObservabilityOptions()
        {
            Enable = false
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }
}
