using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.Redis.Test.Options;

/// <summary>
/// Unit test to <see cref="RedisOptions"/>
/// </summary>
public class RedisOptionsTest
{
    [Fact]
    public void Validate_InstancesListIsEmpty_Failed()
    {
        // Arrange
        var options = new RedisOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage!.Equals("The Instances list must not be empty.") && x.MemberNames.Contains(nameof(RedisOptions.Instances)));
    }

    [Fact]
    public void Validate_SslIsOnButCertificateMissing_Failed()
    {
        // Arrange
        var instanceWithMissingCertificate = new Instance
        {
            ConnectionString = "ssl=true",
        };

        var options = new RedisOptions();
        options.Instances.Add("test", instanceWithMissingCertificate);

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage!.Equals("The Certificate is required.") && x.MemberNames.Contains(nameof(Instance.Certificate)));
    }

}