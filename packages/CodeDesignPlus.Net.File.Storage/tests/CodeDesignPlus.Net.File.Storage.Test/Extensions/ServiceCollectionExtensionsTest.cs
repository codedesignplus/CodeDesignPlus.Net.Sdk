using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.xUnit.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.File.Storage.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddFileStorage_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddFileStorage(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddFileStorage_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddFileStorage(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddFileStorage_SectionNotExist_FileStorageException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<FileStorageException>(() => serviceCollection.AddFileStorage(configuration));

        // Assert
        Assert.Equal($"The section {FileStorageOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddFileStorage_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { FileStorage = OptionsUtil.FileStorageOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddFileStorage(configuration);

        // Assert
        var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IFileStorageService<>));

        Assert.NotNull(libraryService);
        Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
        Assert.Equal(typeof(FileStorageService<>), libraryService.ImplementationType);
    }

    [Fact]
    public void AddFileStorage_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { FileStorage = OptionsUtil.FileStorageOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddFileStorage(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<FileStorageOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);
    }


}
