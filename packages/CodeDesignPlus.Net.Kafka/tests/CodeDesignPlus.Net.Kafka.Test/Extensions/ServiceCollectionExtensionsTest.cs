using CodeDesignPlus.Net.Kafka.Extensions;
using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.Kafka.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddKafka_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddKafka(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddKafka_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddKafka(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddKafka_SectionNotExist_KafkaException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<Net.Kafka.Exceptions.KafkaException>(() => serviceCollection.AddKafka(configuration));

        // Assert
        Assert.Equal($"The section {KafkaOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddKafka_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { Core = OptionUtils.CoreOptions, Kafka = OptionUtils.KafkaOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddKafka(configuration);

        // Assert
        var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IKafkaPubSub));

        Assert.NotNull(libraryService);
        Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
        Assert.Equal(typeof(KafkaPubSub), libraryService.ImplementationType);
    }

    [Fact]
    public void AddKafka_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { Core = OptionUtils.CoreOptions, Kafka = OptionUtils.KafkaOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddKafka(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<KafkaOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);
    }
}
