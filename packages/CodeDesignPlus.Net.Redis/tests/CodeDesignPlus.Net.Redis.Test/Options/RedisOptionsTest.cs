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
    public void Validate_SslWithoutCertificate_Success()
    {
        // Arrange
        var instance = new Instance
        {
            ConnectionString = "cache.redis.azure.net:10000,password=secret,ssl=True,abortConnect=False"
        };

        var options = new RedisOptions();
        options.Instances.Add("Core", instance);

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_PasswordCertificateWithoutCertificate_Failed()
    {
        // Arrange
        var instance = new Instance
        {
            ConnectionString = "cache.redis.azure.net:10000,password=secret,ssl=True",
            PasswordCertificate = "password123"
        };

        var options = new RedisOptions();
        options.Instances.Add("Core", instance);

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage!.Equals("The Certificate is required when the PasswordCertificate is set.") && x.MemberNames.Contains(nameof(Instance.Certificate)));
    }

    [Fact]
    public void Validate_ConnectionStringHasUnknownKeyword_Failed()
    {
        // Arrange
        var instance = new Instance
        {
            ConnectionString = "localhost:6379,opcionDesconocida=1"
        };

        var options = new RedisOptions();
        options.Instances.Add("Core", instance);

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.MemberNames.Contains(nameof(Instance.ConnectionString)));
    }

    [Fact]
    public void Validate_ConnectionStringHasNoEndpoint_Failed()
    {
        // Arrange
        var instance = new Instance
        {
            ConnectionString = "ssl=true"
        };

        var options = new RedisOptions();
        options.Instances.Add("Core", instance);

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage!.Equals("The ConnectionString must contain at least one endpoint.") && x.MemberNames.Contains(nameof(Instance.ConnectionString)));
    }

    [Fact]
    public void Validate_ConnectionStringIsEmpty_Failed()
    {
        // Arrange
        var instance = new Instance
        {
            ConnectionString = string.Empty
        };

        var options = new RedisOptions();
        options.Instances.Add("Core", instance);

        // Act
        var results = options.Validate();

        // Assert
        Assert.Contains(results, x => x.MemberNames.Contains(nameof(Instance.ConnectionString)));
    }

    [Fact]
    public void Validate_MutualTlsWithCertificate_Success()
    {
        // Arrange
        var instance = new Instance
        {
            ConnectionString = "redis.interno:6380,ssl=true",
            Certificate = "cliente.pfx",
            PasswordCertificate = "password123"
        };

        var options = new RedisOptions();
        options.Instances.Add("Core", instance);

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

}