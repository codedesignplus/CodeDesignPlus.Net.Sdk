using CodeDesignPlus.Net.xUnit.Helpers.RabbitMQContainer;
using RabbitMQ.Client;

namespace CodeDesignPlus.Net.xUnit.Test;


public class RabbitMQContainerTest(RabbitMQContainer container) : IClassFixture<RabbitMQContainer>
{

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
        Exception connectionException = null!;

        try
        {
            await Task.Delay(10000);
            using var connection = factory.CreateConnection();

            // Assert
            Assert.True(connection.IsOpen, "Connection should be open.");
        }
        catch (Exception ex)
        {
            connectionException = ex;
        }

        Assert.Null(connectionException);
    }
}
