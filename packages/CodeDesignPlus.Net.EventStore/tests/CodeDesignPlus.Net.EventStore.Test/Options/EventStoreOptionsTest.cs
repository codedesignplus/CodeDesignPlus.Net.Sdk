using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.EventStore.Test.Options;

public class EventStoreOptionsTest
{
    [Fact]
    public void EventStoreOptions_DefaultValues_Valid()
    {
        // Arrange
        var options = new EventStoreOptions();

        // Act
        var results = options.Validate();

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, x => x.ErrorMessage == "The collection of EventStore servers (nodes) to which the application can connect is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void EventStoreOptions_ValuesInvalid_Invalid(string? value)
    {
        // Arrange
        var options = new EventStoreOptions
        {
            Servers = new Dictionary<string, Server>
            {
                {
                    "server1",
                    new Server
                    {
                        ConnectionString = null!,
                        User = value,
                        Password = value
                    }
                }
            }
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, x => x.ErrorMessage == "The User field is required." && x.MemberNames.Contains("User"));
        Assert.Contains(results, x => x.ErrorMessage == "The Password field is required." && x.MemberNames.Contains("Password"));
        Assert.Contains(results, x => x.ErrorMessage == "The ConnectionString field is required." && x.MemberNames.Contains("ConnectionString"));
    }

    [Fact]
    public void EventStoreOptions_ValuesValid_Success()
    {
        // Arrange
        var options = new EventStoreOptions
        {            
            FrequencySnapshot = 15,
            MainName = "aggregate-custom",
            SnapshotSuffix = "snapshot-custom",
            Servers = new Dictionary<string, Server>
            {
                {
                    "server1",
                    new Server
                    {
                        ConnectionString = new Uri("tcp://localhost:1113"),
                        User = "admin",
                        Password = "changeit"
                    }
                }
            }
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void EventStoreOptions_FrequencySnapshot_Invalid()
    {
        // Arrange
        var options = new EventStoreOptions
        {
            FrequencySnapshot = 600,
            MainName = "aggregate-custom",
            SnapshotSuffix = "snapshot-custom",
            Servers = new Dictionary<string, Server>
            {
                {
                    "server1",
                    new Server
                    {
                        ConnectionString = new Uri("tcp://localhost:1113"),
                        User = "admin",
                        Password = "changeit"
                    }
                }
            }
        };

        // Act
        var results = options.Validate();

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, x => x.ErrorMessage == "The field FrequencySnapshot must be between 1 and 500." && x.MemberNames.Contains("FrequencySnapshot"));
    }
}
