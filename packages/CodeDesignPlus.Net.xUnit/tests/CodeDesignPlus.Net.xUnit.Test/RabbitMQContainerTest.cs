using CodeDesignPlus.Net.xUnit.Containers.RabbitMQContainer;
using RabbitMQ.Client;

namespace CodeDesignPlus.Net.xUnit.Test;

[Collection(RabbitMQCollectionFixture.Collection)]
public class RabbitMQContainerTest(RabbitMQCollectionFixture rabbitMQCollectionFixture)
{
    private readonly RabbitMQContainer container = rabbitMQCollectionFixture.Container;

    [Fact]
    public async Task CheckConnectionServer()
    {
        var host = "localhost";
        var port = container.Port;

        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = "usr_codedesignplus",
            Password = "Temporal1"
        };

        // Act
        IConnection connection = null!;

        do
        {
            try
            {
                connection = factory.CreateConnection();

                // Assert
                Assert.True(connection.IsOpen, "Connection should be open.");
            }
            catch
            {
                await Task.Delay(1000);
            }

        } while (connection == null || !connection.IsOpen);

        Assert.NotNull(connection);
        Assert.True(container.IsRunning);
        Assert.True(connection.IsOpen);
    }
}
