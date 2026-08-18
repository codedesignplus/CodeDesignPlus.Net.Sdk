using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Options;

public class QueueArgumentsTest
{
    [Fact]
    public void GetArguments_ReturnsCorrectArguments()
    {
        // Arrange
        var queueArguments = new QueueArguments
        {
            QueueType = "classic",
            MessageTtl = 1000,
            Expires = 3600,
            MaxLength = 100,
            MaxLengthBytes = 1024,
            MaxPriority = 5,
            QueueMode = "default",
            QueueMasterLocator = "min-masters",
            Overflow = "drop-head",
            OverflowRejectPublish = 10,
            ExtraArguments = new Dictionary<string, object>
                {
                    { "arg1", "value1" },
                    { "arg2", 123 }
                }
        };

        var expectedArguments = new Dictionary<string, object>
            {
                { "x-queue-type", "classic" },
                { "x-message-ttl", 1000 },
                { "x-expires", 3600 },
                { "x-max-length", 100 },
                { "x-max-length-bytes", 1024 },
                { "x-max-priority", 5 },
                { "x-queue-mode", "default" },
                { "x-queue-master-locator", "min-masters" },
                { "x-overflow", "drop-head" },
                { "x-overflow-reject-publish", 10 },
                { "x-arguments", new Dictionary<string, object>
                    {
                        { "arg1", "value1" },
                        { "arg2", 123 }
                    }
                }
            };

        // Act
        var result = queueArguments.GetArguments();

        // Assert
        Assert.Equal(expectedArguments["x-queue-type"], result["x-queue-type"]);
        Assert.Equal(expectedArguments["x-message-ttl"], result["x-message-ttl"]);
        Assert.Equal(expectedArguments["x-expires"], result["x-expires"]);
        Assert.Equal(expectedArguments["x-max-length"], result["x-max-length"]);
        Assert.Equal(expectedArguments["x-max-length-bytes"], result["x-max-length-bytes"]);
        Assert.Equal(expectedArguments["x-max-priority"], result["x-max-priority"]);
        Assert.Equal(expectedArguments["x-queue-mode"], result["x-queue-mode"]);
        Assert.Equal(expectedArguments["x-queue-master-locator"], result["x-queue-master-locator"]);
        Assert.Equal(expectedArguments["x-overflow"], result["x-overflow"]);
        Assert.Equal(expectedArguments["x-overflow-reject-publish"], result["x-overflow-reject-publish"]);

        foreach (var item in (Dictionary<string, object>)expectedArguments["x-arguments"])
        {
            Assert.Equal(item.Value, ((Dictionary<string, object>)result["x-arguments"])[item.Key]);
        }

        // El espejado de colas clasicas desaparecio en RabbitMQ 4, asi que x-ha-policy ya no se declara nunca.
        Assert.False(result.ContainsKey("x-ha-policy"));
    }

    [Fact]
    public void Validate_ValidQueueArguments_ReturnsNoValidationErrors()
    {
        // Arrange
        var queueArguments = new QueueArguments
        {
            QueueType = "classic",
            MessageTtl = 1000,
            Expires = 3600,
            MaxLength = 100,
            MaxLengthBytes = 1024,
            MaxPriority = 5,
            QueueMode = "default",
            QueueMasterLocator = "min-masters",
            Overflow = "drop-head",
            OverflowRejectPublish = 10,
            ExtraArguments = new Dictionary<string, object>
                {
                    { "arg1", "value1" },
                    { "arg2", 123 },
                    { "arg3", DateTime.Now }
                }
        };

        // Act
        var results = queueArguments.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void GetArguments_PropertiesWithDefaultValues_ReturnsCorrectArguments()
    {
        // Arrange
        var queueArguments = new QueueArguments();

        var expectedArguments = new Dictionary<string, object>
            {
                { "x-queue-type", "quorum" },
                { "x-message-ttl", 172800000 }
            };

        // Act
        var result = queueArguments.Validate();
        var arguments = queueArguments.GetArguments();

        // Assert
        Assert.Empty(result);
        Assert.Equal(expectedArguments, arguments);
    }

    [Fact]
    public void GetArguments_DefaultQueueType_IsQuorum()
    {
        // Arrange & Act
        var queueArguments = new QueueArguments();
        var arguments = queueArguments.GetArguments();

        // Assert
        // Sin x-queue-type la cola nace clasica, no publica x-delivery-count y el tope de reintentos del
        // consumidor deja de contar: el mensaje se reencola sin fin.
        Assert.True(queueArguments.IsQuorum);
        Assert.Equal("quorum", arguments["x-queue-type"]);
    }

    [Fact]
    public void GetArguments_QueueTypeQuorum_OmitsClassicOnlyArguments()
    {
        // Arrange
        var queueArguments = new QueueArguments
        {
            MaxPriority = 5,
            QueueMode = "lazy",
            QueueMasterLocator = "min-masters"
        };

        // Act
        var arguments = queueArguments.GetArguments();

        // Assert
        // El broker rechaza la declaracion y cierra el canal si una cola quorum recibe cualquiera de los tres.
        Assert.False(arguments.ContainsKey("x-max-priority"));
        Assert.False(arguments.ContainsKey("x-queue-mode"));
        Assert.False(arguments.ContainsKey("x-queue-master-locator"));
        Assert.False(arguments.ContainsKey("x-ha-policy"));
    }

    [Fact]
    public void GetArguments_DeliveryLimitAssigned_IncludeDeliveryLimit()
    {
        // Arrange
        var queueArguments = new QueueArguments { DeliveryLimit = 4 };

        // Act
        var arguments = queueArguments.GetArguments();

        // Assert
        Assert.Equal(4, arguments["x-delivery-limit"]);
    }

    [Fact]
    public void Validate_QuorumWithClassicOnlyArguments_ReturnErrors()
    {
        // Arrange
        var queueArguments = new QueueArguments
        {
            MaxPriority = 5,
            QueueMode = "lazy",
            QueueMasterLocator = "min-masters"
        };

        // Act
        var results = queueArguments.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The field MaxPriority is not supported by quorum queues.");
        Assert.Contains(results, x => x.ErrorMessage == "The field QueueMode is not supported by quorum queues.");
        Assert.Contains(results, x => x.ErrorMessage == "The field QueueMasterLocator is not supported by quorum queues.");
    }

    [Fact]
    public void Validate_DeliveryLimitBelowOne_ReturnError()
    {
        // Arrange
        var queueArguments = new QueueArguments { DeliveryLimit = 0 };

        // Act
        var results = queueArguments.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The field DeliveryLimit must be greater than or equal to one.");
    }

    [Fact]
    public void Validate_QueueTypeInvalid_ReturnError()
    {
        // Arrange
        var queueArguments = new QueueArguments { QueueType = "stream" };

        // Act
        var results = queueArguments.Validate();

        // Assert
        Assert.Contains(results, x => x.ErrorMessage == "The field QueueType must match the regular expression '^(classic|quorum)?$'.");
    }

    [Fact]
    public void Validate_AllValuesInvalid_ReturnAllErrors()
    {
        // Arrange
        var queueArguments = new QueueArguments
        {
            QueueType = "classic",
            MessageTtl = -1,
            Expires = -1,
            MaxLength = -1,
            MaxLengthBytes = -1,
            MaxPriority = -1,
            QueueMode = "invalid",
            QueueMasterLocator = "invalid",
            Overflow = "invalid",
            OverflowRejectPublish = -1,
            ExtraArguments = new Dictionary<string, object>
                {
                    { "arg1", null! }
                }
        };

        // Act
        var results = queueArguments.Validate();

        // Assert
        Assert.NotEmpty(results);

        Assert.Contains(results, x => x.ErrorMessage == "The field MessageTtl must be greater than or equal to zero.");
        Assert.Contains(results, x => x.ErrorMessage == "The field Expires must be greater than or equal to zero.");
        Assert.Contains(results, x => x.ErrorMessage == "The field MaxLength must be greater than or equal to zero.");
        Assert.Contains(results, x => x.ErrorMessage == "The field MaxLengthBytes must be greater than or equal to zero.");
        Assert.Contains(results, x => x.ErrorMessage == "The field MaxPriority must be between 0 and 255.");
        Assert.Contains(results, x => x.ErrorMessage == "The field OverflowRejectPublish must be greater than or equal to zero.");

        Assert.Contains(results, x => x.ErrorMessage == "The field QueueMode must match the regular expression '^(default|lazy)?$'.");
        Assert.Contains(results, x => x.ErrorMessage == "The field QueueMasterLocator must match the regular expression '^(min-masters)?$'.");
        Assert.Contains(results, x => x.ErrorMessage == "The field Overflow must match the regular expression '^(drop-head|reject-publish)?$'.");

        Assert.Contains(results, x => x.ErrorMessage == "The field arg1 of the ExtraArguments cannot be null.");
    }
}
