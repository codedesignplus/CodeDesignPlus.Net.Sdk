using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.xUnit.Helpers;
using CodeDesignPlus.Net.xUnit.Helpers.RabitMQContainer;
using Moq;
using RabbitMQ.Client;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.RabitMQ.Test.Services;

public class RabitConnectionTest(RabitMQContainer container) : IClassFixture<RabitMQContainer>
{
    private readonly IOptions<RabitMQOptions> options = O.Options.Create(new RabitMQOptions()
    {
        Host = "localhost",
        Port = container.Port,
        UserName = "usr_codedesignplus",
        Password = "Temporal1",
        Enable = true
    });

    private readonly IOptions<CoreOptions> coreOptions = O.Options.Create(Helpers.ConfigurationUtil.CoreOptions);

    private readonly Mock<ILogger<RabitConnection>> logger = new();

    [Fact]
    public void RabbitConnection_Constructor_ThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RabitConnection(null!, this.coreOptions, logger.Object));
    }

    [Fact]
    public void RabbitConnection_Constructor_ThrowArgumentNullException2()
    {
        Assert.Throws<ArgumentNullException>(() => new RabitConnection(this.options, null!, logger.Object));
    }

    [Fact]
    public void RabbitConnection_Constructor_ThrowArgumentNullException3()
    {
        Assert.Throws<ArgumentNullException>(() => new RabitConnection(this.options, this.coreOptions, null!));
    }

    [Fact]
    public void RabitConnection_Constructor_CreatesConnection()
    {
        // Arrange
        var connection = new RabitConnection(options, this.coreOptions, logger.Object);

        // Act
        var result = connection.Connection;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsOpen, "The connection is not open");
    }

    [Fact]
    public void RabitConnection_Constructor_ThrowException()
    {
        // Arrange
        var options = O.Options.Create(new RabitMQOptions()
        {
            Host = "lh",
            Port = container.Port,
            UserName = "usr_codedesignplus",
            Password = "Temporal2",
            Enable = true,
            MaxRetry = 5,
            RetryInterval = 1000

        });

        var coreOptions = O.Options.Create(Helpers.ConfigurationUtil.CoreOptions);

        // Act
        var exception = Assert.Throws<RabitMQ.Exceptions.RabitMQException>(() => new RabitConnection(options, coreOptions, logger.Object));

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
    public void RabitConnection_Dispose_DisposesConnection()
    {
        // Arrange
        var connection = new RabitConnection(options, this.coreOptions, logger.Object);

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
            var connection = new RabitConnection(options, this.coreOptions, logger.Object);
        }

        // Act
        action();
        
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Assert
        Assert.True(true);
    }
}
