using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Security.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddSecurity_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddSecurity(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddSecurity_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddSecurity(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddSecurity_SectionNotExist_SecurityException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<SecurityException>(() => serviceCollection.AddSecurity(configuration));

        // Assert
        Assert.Equal($"The section {SecurityOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddSecurity_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddSecurity(configuration);

        // Assert
        var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(ISecurityService));

        Assert.NotNull(libraryService);
        Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
        Assert.Equal(typeof(SecurityService), libraryService.ImplementationType);
    }

    [Fact]
    public void AddSecurity_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddSecurity(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<SecurityOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);

        Assert.Equal(ConfigurationUtil.SecurityOptions.Name, value.Name);
        Assert.Equal(ConfigurationUtil.SecurityOptions.Email, value.Email);
        Assert.Equal(ConfigurationUtil.SecurityOptions.Enable, value.Enable);
    }


}
