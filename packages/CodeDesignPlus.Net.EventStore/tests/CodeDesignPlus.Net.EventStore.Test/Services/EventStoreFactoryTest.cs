﻿using Moq;
using E = EventStore.ClientAPI;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.EventStore.Test.Services;

public class EventStoreFactoryTest
{
    private readonly Mock<ILogger<EventStoreFactory>> loggerMock = new();

    [Fact]
    public void Constructor_ValidParameters_Success()
    {
        // Arrange
        var eventStoreConnection = new Mock<IEventStoreConnection>().Object;
        var logger = new Mock<ILogger<EventStoreFactory>>().Object;
        var options = new Mock<IOptions<EventStoreOptions>>().Object;

        // Act
        var factory = new EventStoreFactory(eventStoreConnection, logger, options);

        // Assert
        Assert.NotNull(factory);
    }

    [Fact]
    public void Constructor_NullEventStoreConnection_ThrowsArgumentNullException()
    {
        // Arrange
        var logger = new Mock<ILogger<EventStoreFactory>>().Object;
        var options = new Mock<IOptions<EventStoreOptions>>().Object;

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new EventStoreFactory(null, logger, options));
    }

    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        var eventStoreConnection = new Mock<IEventStoreConnection>().Object;
        var options = new Mock<IOptions<EventStoreOptions>>().Object;

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new EventStoreFactory(eventStoreConnection, null, options));
    }

    [Fact]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        var eventStoreConnection = new Mock<IEventStoreConnection>().Object;
        var logger = new Mock<ILogger<EventStoreFactory>>().Object;

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new EventStoreFactory(eventStoreConnection, logger, null));
    }

    [Fact]
    public async Task CreateAsync_ValidKey_ReturnsConnection()
    {
        // Arrange
        var eventStoreConnection = new Mock<IEventStoreConnection>();
        var eventConnection = new Mock<E.IEventStoreConnection>();
        var options = O.Options.Create(OptionsUtil.EventStoreOptions);
        var factory = new EventStoreFactory(eventStoreConnection.Object, loggerMock.Object, options);
        var key = EventStoreFactoryConst.Core;

        eventStoreConnection.Setup(e => e.InitializeAsync(It.IsAny<Server>())).ReturnsAsync(eventConnection.Object);

        // Act
        var connection = await factory.CreateAsync(key);

        // Assert
        Assert.Same(eventConnection.Object, connection);
    }

    [Fact]
    public async Task CreateAsync_InvalidKey_ThrowsEventStoreException()
    {
        // Arrange
        var eventStoreConnection = new Mock<IEventStoreConnection>();
        var options = O.Options.Create(new EventStoreOptions());
        var factory = new EventStoreFactory(eventStoreConnection.Object, loggerMock.Object, options);
        var key = "invalidKey";

        // Act and Assert
        var exception = await Assert.ThrowsAsync<EventStoreException>(() => factory.CreateAsync(key));

        Assert.Equal("The connection is not registered in the settings.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_EmptyKey_ThrowsArgumentNullException()
    {
        // Arrange
        var eventStoreConnection = new Mock<IEventStoreConnection>();
        var options = O.Options.Create(new EventStoreOptions());
        var factory = new EventStoreFactory(eventStoreConnection.Object, loggerMock.Object, options);
        var key = "";

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => factory.CreateAsync(key));
    }

    [Fact]
    public async Task RemoveConnection_ValidKey_ReturnsTrue()
    {
        // Arrange
        var eventStoreConnection = new Mock<IEventStoreConnection>();
        var options = O.Options.Create(OptionsUtil.EventStoreOptions);
        var factory = new EventStoreFactory(eventStoreConnection.Object, loggerMock.Object, options);
        var key = EventStoreFactoryConst.Core;
        await factory.CreateAsync(key);

        // Act
        var result = factory.RemoveConnection(key);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void RemoveConnection_InvalidKey_ReturnsFalse()
    {
        // Arrange
        var eventStoreConnection = new Mock<IEventStoreConnection>();
        var options = O.Options.Create(OptionsUtil.EventStoreOptions);
        var factory = new EventStoreFactory(eventStoreConnection.Object, loggerMock.Object, options);
        var key = "invalidKey";

        // Act
        var result = factory.RemoveConnection(key);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GetCredentials_keyIsNullOrEmpty(string? key)
    {
        // Arrange
        var eventStoreConnection = new Mock<IEventStoreConnection>();
        var options = O.Options.Create(OptionsUtil.EventStoreOptions);
        var factory = new EventStoreFactory(eventStoreConnection.Object, loggerMock.Object, options);

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => factory.GetCredentials(key));
    }

    [Fact]
    public void GetCredentials_ServerNotContainsKey_EventStoreException()
    {
        // Arrange
        var eventStoreConnection = new Mock<IEventStoreConnection>();
        var options = O.Options.Create(OptionsUtil.EventStoreOptions);
        var factory = new EventStoreFactory(eventStoreConnection.Object, loggerMock.Object, options);
        var key = "invalidKey";

        // Act and Assert
        var exception = Assert.Throws<EventStoreException>(() => factory.GetCredentials(key));

        Assert.Equal("The connection is not registered in the settings.", exception.Message);
    }

    [Fact]
    public void GetCredentials_ValidKey_ReturnsCredentials()

    {
        // Arrange
        var eventStoreConnection = new Mock<IEventStoreConnection>();
        var options = O.Options.Create(OptionsUtil.EventStoreOptions);
        var expected = OptionsUtil.EventStoreOptions.Servers[EventStoreFactoryConst.Core];
        var factory = new EventStoreFactory(eventStoreConnection.Object, loggerMock.Object, options);

        // Act
        var (user, pass) = factory.GetCredentials(EventStoreFactoryConst.Core);

        // Assert
        Assert.NotNull(user);
        Assert.NotNull(pass);
        Assert.Equal(expected.User, user);
        Assert.Equal(expected.Password, pass);
    }
}
