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
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEFCore(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddEFCore_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEFCore(null));

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
        var exception = Assert.Throws<EFCoreException>(() => serviceCollection.AddEFCore(configuration));

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
        serviceCollection.AddEFCore(configuration);

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
        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRepositories<CodeDesignPlusContextInMemory>();

        // Assert
        var repository = serviceCollection.Where(x => typeof(IRepositoryBase).IsAssignableFrom(x.ServiceType)).ToList();

        repository.ForEach(x => Assert.Equal(ServiceLifetime.Transient, x.Lifetime));
    }

    [Fact]
    public void AddEfCore_RegisterConfigurations_EfCoreOption()
    {
        // Arrange
        var section = "EFCore";
        var memory = new Dictionary<string, string>()
        {
            { $"{section}:{nameof(EFCoreOptions.ClaimsIdentity)}:{nameof(ClaimsOption.User)}", "user"},
            { $"{section}:{nameof(EFCoreOptions.ClaimsIdentity)}:{nameof(ClaimsOption.IdUser)}", "sub"},
            { $"{section}:{nameof(EFCoreOptions.ClaimsIdentity)}:{nameof(ClaimsOption.Email)}", "email"},
            { $"{section}:{nameof(EFCoreOptions.ClaimsIdentity)}:{nameof(ClaimsOption.Role)}", "role"},
        };
        var configBuilder = new ConfigurationBuilder().AddInMemoryCollection(memory as IEnumerable<KeyValuePair<string, string?>>);

        /*
         * appsettings.json
         * {
         *      "EFCore": {
         *          "ClaimsIdentity": {
         *              "User": "user",
         *              "IdUser": "sub",
         *              "Email": "email",
         *              "Role": "role"
         *          }
         *      }
         * }
         */

        var configuration = configBuilder.Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);

        // Act 
        services.AddEFCore(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<EFCoreOptions>>();
        var efCoreOption = options?.Value;

        Assert.NotNull(efCoreOption);
        Assert.Equal("user", efCoreOption.ClaimsIdentity.User);
        Assert.Equal("email", efCoreOption.ClaimsIdentity.Email);
        Assert.Equal("sub", efCoreOption.ClaimsIdentity.IdUser);
        Assert.Equal("role", efCoreOption.ClaimsIdentity.Role);
    }
}
