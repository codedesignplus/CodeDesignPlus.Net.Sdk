using CodeDesignPlus.Net.Redis.Cache.Abstractions.Options;
using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.Redis.Cache.Test.Options;

public class RedisCacheOptionsTest
{
    [Fact]
    public void RedisCacheOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new RedisCacheOptions()
        {
            Enable = true,
            Expiration = TimeSpan.FromMinutes(5),
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void RedisCacheOptions_ExpirationIsNegative_FailedValidation()
    {
        // Arrange
        var options = new RedisCacheOptions()
        {
            Enable = true,
            Expiration = TimeSpan.FromMinutes(-5),
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The Expiration field is required.");
    }

    [Fact]
    public void RedisCacheOptions_ExpirationIsInvalidWhenEnableIsFalse_Success()
    {
        // Arrange
        var options = new RedisCacheOptions()
        {
            Enable = false,
            Expiration = TimeSpan.FromMinutes(-5),
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }
}
