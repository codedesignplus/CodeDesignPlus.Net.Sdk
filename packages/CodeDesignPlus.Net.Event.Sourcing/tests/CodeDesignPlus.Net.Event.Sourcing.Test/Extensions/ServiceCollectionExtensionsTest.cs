using CodeDesignPlus.Net.Event.Sourcing.Extensions;
using CodeDesignPlus.Net.xUnit.Extensions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddEventSourcing_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEventSourcing(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddEventSourcing_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEventSourcing(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddEventSourcing_SectionNotExist_EventSourcingException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<EventSourcingException>(() => serviceCollection.AddEventSourcing(configuration));

        // Assert
        Assert.Equal($"The section {EventSourcingOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddEventSourcing_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            Core = OptionsUtil.CoreOptions,
            EventSourcing = OptionsUtil.Options
        });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddEventSourcing(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<EventSourcingOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);

        Assert.Equal(OptionsUtil.Options.MainName, value.MainName);
        Assert.Equal(OptionsUtil.Options.SnapshotSuffix, value.SnapshotSuffix);
    }


    [Fact]
    public void AddEventSourcing_WithNullServiceCollection_ThrowsArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEventSourcing(Mock.Of<IConfiguration>(), x => { }));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddEventSourcing_WithNullConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEventSourcing(null!, x => { }));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddEventSourcing_WithNullSetupOptions_ThrowsArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        IConfiguration configuration = new ConfigurationBuilder().Build();
        Action<EventSourcingOptions>? setupOptions = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEventSourcing(configuration, setupOptions));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'setupOptions')", exception.Message);
    }

    [Fact]
    public void AddEventSourcing_WithValidParameters_RegistersOptionsCorrectly()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            Core = OptionsUtil.CoreOptions
        });

        var serviceCollection = new ServiceCollection();

        var expectedMainName = "TestMainName";
        var expectedSnapshotSuffix = "TestSnapshotSuffix";

        // Act
        serviceCollection.AddEventSourcing(configuration, options =>
        {
            options.MainName = expectedMainName;
            options.SnapshotSuffix = expectedSnapshotSuffix;
        });

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<EventSourcingOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);
        Assert.Equal(expectedMainName, value.MainName);
        Assert.Equal(expectedSnapshotSuffix, value.SnapshotSuffix);
    }

}
