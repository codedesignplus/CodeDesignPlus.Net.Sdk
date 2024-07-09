﻿using CodeDesignPlus.Net.xUnit.Helpers.RabitMQContainer;
using RabbitMQ.Client;

namespace CodeDesignPlus.Net.xUnit.Test;


public class RabitMQContainerTest(RabitMQContainer container) : IClassFixture<RabitMQContainer>
{

    [Fact]
    public void CheckConnectionServer()
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
