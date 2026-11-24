namespace CodeDesignPlus.Net.Core.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddCore_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddCore(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddCore_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddCore(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddCore_SectionNotExist_CoreException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<CoreException>(() => serviceCollection.AddCore(configuration));

        // Assert
        Assert.Equal($"The section {CoreOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddCore_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddCore(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<CoreOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);

        Assert.Equal(ConfigurationUtil.CoreOptions.AppName, value.AppName);
    }

    [Fact]
    public void AddStartups_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddCore(configuration);

        // Assert
        Assert.True(StartupFake.Initialized);
    }

    [Fact]
    public void AddCore_CalledThreeTimes_RunsEachStartupOnce()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddCore(configuration);
        serviceCollection.AddCore(configuration);
        serviceCollection.AddCore(configuration);

        // Assert
        Assert.Equal(1, CountingStartupFake.GetExecutions(serviceCollection));
    }

    [Fact]
    public void AddCore_CalledThreeTimes_BoundListHasNoDuplicates()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            Core = ConfigurationUtil.CoreOptions,
            ListFake = new ListOptionsFake { Items = ["R-99-PN", "O-13"] }
        });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddCore(configuration);
        serviceCollection.AddCore(configuration);
        serviceCollection.AddCore(configuration);

        // Assert
        var options = serviceCollection.BuildServiceProvider().GetRequiredService<IOptions<ListOptionsFake>>();

        Assert.Equal(["R-99-PN", "O-13"], options.Value.Items);
    }

    [Fact]
    public void AddCore_DistinctServiceCollections_RunsStartupsOnEach()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var first = new ServiceCollection();
        var second = new ServiceCollection();

        // Act
        first.AddCore(configuration);
        second.AddCore(configuration);

        // Assert
        Assert.Equal([1, 1], new[] { CountingStartupFake.GetExecutions(first), CountingStartupFake.GetExecutions(second) });
    }
}
