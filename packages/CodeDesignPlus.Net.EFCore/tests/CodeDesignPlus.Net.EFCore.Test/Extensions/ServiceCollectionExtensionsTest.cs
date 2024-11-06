using CodeDesignPlus.InMemory;
using CodeDesignPlus.Net.EFCore.Extensions;

namespace CodeDesignPlus.Net.EFCore.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddEFCore_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEFCore<CodeDesignPlusContextInMemory>(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddEFCore_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEFCore<CodeDesignPlusContextInMemory>(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddEFCore_SectionNotExist_EFCoreException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<EFCoreException>(() => serviceCollection.AddEFCore<CodeDesignPlusContextInMemory>(configuration));

        // Assert
        Assert.Equal($"The section {EFCoreOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddEFCore_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddEFCore<CodeDesignPlusContextInMemory>(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<EFCoreOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);
    }

    [Fact]
    public void AddRepositories_ScanAndRegisterWithTransient_AddRepositories()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();
        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddEFCore<CodeDesignPlusContextInMemory>(configuration);

        // Assert
        var repository = serviceCollection.Where(x => typeof(IRepositoryBase).IsAssignableFrom(x.ServiceType)).ToList();

        repository.ForEach(x => Assert.Equal(ServiceLifetime.Transient, x.Lifetime));
    }
}
