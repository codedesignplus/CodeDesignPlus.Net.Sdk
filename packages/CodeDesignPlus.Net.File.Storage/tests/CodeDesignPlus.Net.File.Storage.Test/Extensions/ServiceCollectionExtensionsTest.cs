using CodeDesignPlus.Net.File.Storage.Abstractions.Factories;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using CodeDesignPlus.Net.File.Storage.Extensions;
using CodeDesignPlus.Net.File.Storage.Factories;
using CodeDesignPlus.Net.File.Storage.Providers;
using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.File.Storage.Test.Extensions;

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
        var fileService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IFileStorageService));
        var azureBlobProvider = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IAzureBlobProvider));
        var azureFileProvider = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IAzureFileProvider));
        var localProvider = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(ILocalProvider));

        var azureBlobFactory = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IAzureBlobFactory));
        var azureFileFactory = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IAzureFileFactory));

        var providers = serviceCollection.Where(x => x.ServiceType == typeof(IProvider));

        Assert.NotNull(fileService);
        Assert.Equal(ServiceLifetime.Singleton, fileService.Lifetime);
        Assert.Equal(typeof(FileStorageService), fileService.ImplementationType);

        Assert.NotNull(azureBlobProvider);
        Assert.Equal(ServiceLifetime.Singleton, azureBlobProvider.Lifetime);
        Assert.Equal(typeof(AzureBlobProvider), azureBlobProvider.ImplementationType);

        Assert.NotNull(azureFileProvider);
        Assert.Equal(ServiceLifetime.Singleton, azureFileProvider.Lifetime);
        Assert.Equal(typeof(AzureFileProvider), azureFileProvider.ImplementationType);

        Assert.NotNull(localProvider);
        Assert.Equal(ServiceLifetime.Singleton, localProvider.Lifetime);
        Assert.Equal(typeof(LocalProvider), localProvider.ImplementationType);

        Assert.NotNull(azureBlobFactory);
        Assert.Equal(ServiceLifetime.Singleton, azureBlobFactory.Lifetime);
        Assert.Equal(typeof(AzureBlobFactory), azureBlobFactory.ImplementationType);

        Assert.NotNull(azureFileFactory);
        Assert.Equal(ServiceLifetime.Singleton, azureFileFactory.Lifetime);
        Assert.Equal(typeof(AzureFileFactory), azureFileFactory.ImplementationType);

        Assert.NotNull(providers);
        Assert.Contains(providers, x => x.ImplementationType == typeof(AzureBlobProvider));
        Assert.Contains(providers, x => x.ImplementationType == typeof(AzureFileProvider));
        Assert.Contains(providers, x => x.ImplementationType == typeof(LocalProvider));
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
