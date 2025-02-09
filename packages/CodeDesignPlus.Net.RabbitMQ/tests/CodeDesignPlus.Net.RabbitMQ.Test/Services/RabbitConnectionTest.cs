﻿using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.xUnit.Extensions;
using CodeDesignPlus.Net.xUnit.Containers.RabbitMQContainer;
using Moq;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Services;

[Collection(RabbitMQCollectionFixture.Collection)]
public class RabbitConnectionTest(RabbitMQCollectionFixture fixture)
{
    private readonly IOptions<RabbitMQOptions> options = O.Options.Create(new RabbitMQOptions()
    {
        Host = "localhost",
        Port = fixture.Container.Port,
        UserName = "usr_codedesignplus",
        Password = "Temporal1",
        Enable = true,
        MaxRetry = 10,
        RetryInterval = 1000
    });

    private readonly IOptions<CoreOptions> coreOptions = O.Options.Create(Helpers.ConfigurationUtil.CoreOptions);

    private readonly Mock<ILogger<RabbitConnection>> logger = new();

    [Fact]
    public async Task RabitConnection_ConnectAsync_CreateConnection()
    {
        // Arrange
        var connection = new RabbitConnection(logger.Object);

        // Act
        await connection.ConnectAsync(options.Value, coreOptions.Value.AppName);

        var result = connection.Connection;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsOpen, "The connection is not open");
        logger.VerifyLogging("RabbitMQ Connection established successfully.", LogLevel.Information, Times.Once());
    }

    [Fact]
    public void RabitConnection_ConnectAsync_ThrowException()
    {
        // Arrange
        var options = O.Options.Create(new RabbitMQOptions()
        {
            Host = "lh",
            Port = fixture.Container.Port,
            UserName = "usr_codedesignplus",
            Password = "Temporal2",
            Enable = true,
            MaxRetry = 5,
            RetryInterval = 1000

        });

        var coreOptions = O.Options.Create(Helpers.ConfigurationUtil.CoreOptions);

        var connection = new RabbitConnection(logger.Object);

        // Act
        var exception = Assert.Throws<RabbitMQ.Exceptions.RabbitMQException>(() => connection.ConnectAsync(options.Value, coreOptions.Value.AppName).GetAwaiter().GetResult());

        // Assert
        Assert.Equal("Failed to connect to the RabbitMQ server", exception.Message);
        Assert.NotEmpty(exception.Errors);
        logger.VerifyLogging("Error connecting. Attempt 1 of 5.", LogLevel.Error, Times.Once());
        logger.VerifyLogging("Error connecting. Attempt 2 of 5.", LogLevel.Error, Times.Once());
        logger.VerifyLogging("Error connecting. Attempt 3 of 5.", LogLevel.Error, Times.Once());
        logger.VerifyLogging("Error connecting. Attempt 4 of 5.", LogLevel.Error, Times.Once());
        logger.VerifyLogging("Error connecting. Attempt 5 of 5.", LogLevel.Error, Times.Once());
    }

    [Fact]
    public async Task RabitConnection_Dispose_DisposesConnection()
    {
        // Arrange
        var connection = new RabbitConnection(logger.Object);
        await connection.ConnectAsync(options.Value, coreOptions.Value.AppName);

        // Act
        connection.Dispose();

        // Assert
        Assert.False(connection.Connection.IsOpen, "The connection is open");
    }

    [Fact]
    public void RabitConnection_Destructor_DisposesConnection()
    {
        // Arrange
        void action()
        {
            var connection = new RabbitConnection(logger.Object);

            connection.ConnectAsync(options.Value, coreOptions.Value.AppName).GetAwaiter().GetResult();
            
        }

        // Act
        action();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Assert
        Assert.True(true);
    }
}
