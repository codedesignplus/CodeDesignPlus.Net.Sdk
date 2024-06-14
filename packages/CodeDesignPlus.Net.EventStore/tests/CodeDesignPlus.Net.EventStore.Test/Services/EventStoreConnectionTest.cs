using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.xUnit.Helpers;
using Moq;
using O = Microsoft.Extensions.Options;
using ES = EventStore.ClientAPI;
using CodeDesignPlus.Net.xUnit.Helpers.EventStoreContainer;
using System.Reflection;
namespace CodeDesignPlus.Net.EventStore.Test;

public class EventStoreConnectionTest : IClassFixture<EventStoreContainer>
{
    private readonly O.IOptions<CoreOptions> coreOptions = O.Options.Create(new CoreOptions()
    {
        AppName = "AppTest"
    });

    private readonly EventStoreContainer fixture;

    public EventStoreConnectionTest(EventStoreContainer eventStoreContainer)
    {
        this.fixture = eventStoreContainer;
    }

    [Fact]
    public void Constructor_ValidParameters_Success()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStoreConnection>>().Object;

        // Act
        var connection = new EventStoreConnection(this.coreOptions, logger);

        // Assert
        Assert.NotNull(connection);
    }

    [Fact]
    public void Constructor_NullCoreOptions_ThrowsArgumentNullException()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStoreConnection>>().Object;

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new EventStoreConnection(null, logger));
    }

    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new EventStoreConnection(this.coreOptions, null));
    }

    [Fact]
    public async Task InitializeAsync_Success()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStoreConnection>>();
        var server = new Server()
        {
            ConnectionString = new Uri($"tcp://localhost:{this.fixture.Port}")
        };

        var eventStoreConnection = new EventStoreConnection(this.coreOptions, logger.Object);

        // Act
        var connection = await eventStoreConnection.InitializeAsync(server);

        // Assert
        Assert.Equal("AppTest", connection.ConnectionName);
        logger.VerifyLogging("Successfully connected to EventStore.", LogLevel.Information);
    }


    [Fact]
    public void AuthenticationFailed_LogsError()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStoreConnection>>();
        var eventStoreConnection = new EventStoreConnection(this.coreOptions, logger.Object);
        var eventArgs = new ES.ClientAuthenticationFailedEventArgs(null, "Reason");

        var methodInfo = typeof(EventStoreConnection).GetMethod("AuthenticationFailed", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        methodInfo!.Invoke(eventStoreConnection, new object[] { null!, eventArgs });

        // Assert        
        logger.VerifyLogging("Authentication failed in EventStore: Reason", LogLevel.Error);
    }

    [Fact]
    public void ErrorOccurred_LogsError()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStoreConnection>>();
        var eventStoreConnection = new EventStoreConnection(this.coreOptions, logger.Object);
        var exception = new Exception("Test error");
        var eventArgs = new ES.ClientErrorEventArgs(null, exception);

        var methodInfo = typeof(EventStoreConnection).GetMethod("ErrorOccurred", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        methodInfo!.Invoke(eventStoreConnection, new object[] { null!, eventArgs });

        // Assert
        logger.VerifyLogging("Error occurred in EventStore: Test error", LogLevel.Error);
    }

    [Fact]
    public void Closed_LogsInformation()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStoreConnection>>();
        var eventStoreConnection = new EventStoreConnection(this.coreOptions, logger.Object);
        var eventArgs = new ES.ClientClosedEventArgs(null, "Reason");

        var methodInfo = typeof(EventStoreConnection).GetMethod("Closed", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        methodInfo!.Invoke(eventStoreConnection, new object[] { null!, eventArgs });

        // Assert
        logger.VerifyLogging("EventStore connection closed: Reason", LogLevel.Information);
    }

    [Fact]
    public void Reconnecting_LogsInformation()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStoreConnection>>();
        var eventStoreConnection = new EventStoreConnection(this.coreOptions, logger.Object);
        var eventArgs = new ES.ClientReconnectingEventArgs(null);

        var methodInfo = typeof(EventStoreConnection).GetMethod("Reconnecting", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        methodInfo!.Invoke(eventStoreConnection, new object[] { null!, eventArgs });

        // Assert
        logger.VerifyLogging("Reconnecting to EventStore", LogLevel.Information);
    }

    [Fact]
    public void Disconnected_LogsInformation()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStoreConnection>>();
        var eventStoreConnection = new EventStoreConnection(this.coreOptions, logger.Object);
        var eventArgs = new ES.ClientConnectionEventArgs(null, null);

        var methodInfo = typeof(EventStoreConnection).GetMethod("Disconnected", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        methodInfo!.Invoke(eventStoreConnection, new object[] { null!, eventArgs });

        // Assert
        logger.VerifyLogging("EventStore connection disconnected.", LogLevel.Information);
    }

    [Fact]
    public void Connected_LogsInformation()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStoreConnection>>();
        var eventStoreConnection = new EventStoreConnection(this.coreOptions, logger.Object);
        var eventArgs = new ES.ClientConnectionEventArgs(null, null);

        var methodInfo = typeof(EventStoreConnection).GetMethod("Connected", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        methodInfo!.Invoke(eventStoreConnection, new object[] { null!, eventArgs });

        // Assert
        logger.VerifyLogging("EventStore connection established.", LogLevel.Information);
    }

    [Fact]
    public async Task InitializeAsync_ServerNull_ArgumentNullException()
    {
         // Arrange
        var logger = new Mock<ILogger<EventStoreConnection>>().Object;
        var connection = new EventStoreConnection(this.coreOptions, logger);

        // Assert & Act
        await Assert.ThrowsAsync<ArgumentNullException>(() => connection.InitializeAsync(null!));
    }
}