using CodeDesignPlus.Net.xUnit.Helpers;
using CodeDesignPlus.Net.xUnit.Helpers.RedisContainer;
using Moq;
using StackExchange.Redis;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace CodeDesignPlus.Net.Redis.Test.Services;

public class RedisServiceTest(RedisContainer fixture) : IClassFixture<RedisContainer>
{
    private readonly RedisContainer fixture = fixture;

    [Fact]
    public void Constructor_LoggerIsNull_ArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisService(null));
    }

    [Fact]
    public void Initialize_Connection_Success()
    {
        // Act
        var redisService = this.fixture.RedisServer;

        // Assert
        Assert.True(redisService.IsConnected);
        Assert.NotNull(redisService.Database);
        Assert.NotNull(redisService.Subscriber);
    }

    [Fact]
    public void Initialize_CertificateWithoutPassword_Success()
    {
        // Act
        var redisService = this.fixture.RedisServerWithoutPfxPassword;

        // Assert
        Assert.True(redisService.IsConnected);
        Assert.NotNull(redisService.Database);
        Assert.NotNull(redisService.Subscriber);
    }

    [Fact]
    public void InternalError_WriteLogger()
    {
        // Arrange 
        var redisService = this.fixture.RedisServer;

        var endpoint = redisService.Connection.GetEndPoints().FirstOrDefault();

        var arguments = new InternalErrorEventArgs(this, endpoint!, ConnectionType.Subscription, new Exception(), nameof(RedisServiceTest));

        var data = new
        {
            arguments.ConnectionType,
            EndPoint = arguments.EndPoint!.ToString(),
            arguments.Origin
        };

        // Act
        this.InvokeHandler(redisService, arguments, nameof(IConnectionMultiplexer.InternalError));

        // Assert
        Assert.True(redisService.IsConnected);
        Assert.NotNull(redisService.Database);
        Assert.NotNull(redisService.Subscriber);

        this.fixture.Logger.VerifyLogging(string.Format("Internal Error - Data: {0}", JsonSerializer.Serialize(data)), LogLevel.Critical);
    }

    [Fact]
    public void HashSlotMoved_WriteLogger()
    {
        // Arrange
        var redisService = this.fixture.RedisServer;

        var oldEndpoint = redisService.Connection.GetEndPoints().ElementAt(0);
        var newEndpoint = redisService.Connection.GetEndPoints().ElementAt(0);

        var arguments = new HashSlotMovedEventArgs(this, 0, oldEndpoint, newEndpoint);

        var data = new
        {
            arguments.HashSlot,
            OldEndPoint = arguments.OldEndPoint?.ToString(),
            NewEndPoint = arguments.NewEndPoint.ToString()
        };

        // Act
        this.InvokeHandler(redisService, arguments, nameof(IConnectionMultiplexer.HashSlotMoved));

        // Assert
        Assert.True(redisService.IsConnected);
        Assert.NotNull(redisService.Database);
        Assert.NotNull(redisService.Subscriber);

        this.fixture.Logger.VerifyLogging(string.Format("Hash Slot Moved - Data: {0}", JsonSerializer.Serialize(data)), LogLevel.Warning);
    }

    [Fact]
    public void ErrorMessage_WriteLogger()
    {
        // Arrange
        var redisService = this.fixture.RedisServer;

        var endpoint = redisService.Connection.GetEndPoints().ElementAt(0);

        var arguments = new RedisErrorEventArgs(this, endpoint, "Internal Error Message");

        var data = new
        {
            EndPoint = arguments.EndPoint?.ToString(),
            arguments.Message
        };

        // Act
        this.InvokeHandler(redisService, arguments, nameof(IConnectionMultiplexer.ErrorMessage));

        // Assert
        Assert.True(redisService.IsConnected);
        Assert.NotNull(redisService.Database);
        Assert.NotNull(redisService.Subscriber);

        this.fixture.Logger.VerifyLogging(string.Format("Error Message - Data: {0}", JsonSerializer.Serialize(data)), LogLevel.Error);
    }

    [Fact]
    public void ConnectionRestored_WriteLogger()
    {
        // Arrange
        var redisService = this.fixture.RedisServer;

        var endpoint = redisService.Connection.GetEndPoints().ElementAt(0);

        var arguments = new ConnectionFailedEventArgs(this, endpoint, ConnectionType.Subscription, ConnectionFailureType.SocketClosed, new Exception(), nameof(RedisServiceTest));

        var data = new
        {
            arguments.ConnectionType,
            EndPoint = arguments.EndPoint?.ToString(),
            arguments.FailureType,
            physicalNameConnection = arguments.ToString()
        };

        // Act
        this.InvokeHandler(redisService, arguments, nameof(IConnectionMultiplexer.ConnectionRestored));

        // Assert
        Assert.True(redisService.IsConnected);
        Assert.NotNull(redisService.Database);
        Assert.NotNull(redisService.Subscriber);

        this.fixture.Logger.VerifyLogging(string.Format("Connection Restored - Data: {0}", JsonSerializer.Serialize(data)), LogLevel.Information);
    }

    [Fact]
    public void ConnectionFailed_WriteLogger()
    {
        // Arrange
        var redisService = this.fixture.RedisServer;

        var endpoint = redisService.Connection.GetEndPoints().ElementAt(0);

        var arguments = new ConnectionFailedEventArgs(this, endpoint, ConnectionType.Subscription, ConnectionFailureType.SocketClosed, new Exception(), nameof(RedisServiceTest));

        var data = new
        {
            arguments.ConnectionType,
            EndPoint = arguments.EndPoint?.ToString(),
            arguments.FailureType,
            physicalNameConnection = arguments.ToString()
        };

        // Act
        this.InvokeHandler(redisService, arguments, nameof(IConnectionMultiplexer.ConnectionFailed));

        // Assert
        Assert.True(redisService.IsConnected);
        Assert.NotNull(redisService.Database);
        Assert.NotNull(redisService.Subscriber);

        this.fixture.Logger.VerifyLogging(string.Format("Connection Failed - Data: {0}", JsonSerializer.Serialize(data)), LogLevel.Information);
    }

    [Fact]
    public void ConfigurationChangedBroadcast_WriteLogger()
    {
        // Arrange
        var redisService = this.fixture.RedisServer;

        var endpoint = redisService.Connection.GetEndPoints().ElementAt(0);

        var arguments = new EndPointEventArgs(this, endpoint);

        var data = new
        {
            EndPoint = arguments.EndPoint?.ToString()
        };

        // Act
        this.InvokeHandler(redisService, arguments, nameof(IConnectionMultiplexer.ConfigurationChangedBroadcast));

        // Assert
        Assert.True(redisService.IsConnected);
        Assert.NotNull(redisService.Database);
        Assert.NotNull(redisService.Subscriber);

        this.fixture.Logger.VerifyLogging(string.Format("Configuration Changed Broadcast - Data: {0}", JsonSerializer.Serialize(data)), LogLevel.Information);
    }

    [Fact]
    public void ConfigurationChanged_WriteLogger()
    {
        // Arrange
        var redisService = this.fixture.RedisServer;

        var endpoint = redisService.Connection.GetEndPoints()[0];

        var arguments = new EndPointEventArgs(this, endpoint);

        var data = new
        {
            EndPoint = arguments.EndPoint?.ToString()
        };

        // Act
        this.InvokeHandler(redisService, arguments, nameof(IConnectionMultiplexer.ConfigurationChanged));

        // Assert
        Assert.True(redisService.IsConnected);
        Assert.NotNull(redisService.Database);
        Assert.NotNull(redisService.Subscriber);

        this.fixture.Logger.VerifyLogging(string.Format("Configuration Changed - Data: {0}", JsonSerializer.Serialize(data)), LogLevel.Information, Times.AtLeastOnce());
    }

    [Fact]
    public void CertificateValidation_WithNoErrors_ReturnsTrue()
    {
        // Arrange
        var service = new RedisService(Mock.Of<ILogger<RedisService>>());
        var method = typeof(RedisService).GetMethod("CertificateValidation", BindingFlags.NonPublic | BindingFlags.Static);
        var chain = new X509Chain();
        var sslPolicyErrors = SslPolicyErrors.None;
        var passwordCertificate = "fakePassword";
        var certificate = "fakeCertificate";

        // Act
        var result = (bool)method!.Invoke(service, new object[] { chain, sslPolicyErrors, passwordCertificate, certificate })!;

        // Assert
        Assert.True(result);
    }

    private void InvokeHandler<TEventArgs>(IRedisService redisService, TEventArgs arguments, string member)
    {
        var typeConnection = redisService.Connection.GetType();

        var field = typeConnection.GetField(member, BindingFlags.Instance | BindingFlags.NonPublic);

        var eventHandler = (EventHandler<TEventArgs>)field?.GetValue(redisService.Connection)!;

        eventHandler?.Invoke(this, arguments);
    }

}
