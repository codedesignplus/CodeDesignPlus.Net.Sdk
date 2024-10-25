using CodeDesignPlus.Net.Vault.Abstractions.Options;
using CodeDesignPlus.Net.Vault.Services;
using VaultSharp;

namespace CodeDesignPlus.Net.Vault.Test.Extensions;

[Collection(VaultCollectionFixture.Collection)]
public class VaultExtensionsTest(VaultCollectionFixture fixture)
{
    [Fact]
    public void AddVault_ServiceCollectionIsNull_ThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection serviceCollection = null!;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddVault(new ConfigurationBuilder().Build()));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddVault_ConfigurationIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        IConfiguration configuration = null!;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddVault(configuration));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddVault_SectionNotExist_ThrowVaultException()
    {
        // Arrange
        var configurationBuilder = new ConfigurationBuilder();

        var configuration = configurationBuilder.Build();

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<VaultException>(() => serviceCollection.AddVault(configuration));

        // Assert
        Assert.Equal("The section Vault is required.", exception.Message);
    }

    [Fact]
    public void AddVault_RegisterServicesAndOptions_Susscess()
    {
        // Arrange
        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Vault:Address", "http://localhost:8200" },
                { "Vault:RoleId", "role-id" },
                { "Vault:SecretId", "secret-id" },
                { "Vault:Solution", "unit-test" },
                { "Vault:AppName", "my-app" }
            });

        var configuration = configurationBuilder.Build();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddVault(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var factory = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IVaultClient));
        var vaultTransit = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IVaultTransit));
        var vaultOptions = serviceProvider.GetRequiredService<IOptions<VaultOptions>>().Value;

        Assert.NotNull(factory);

        Assert.NotNull(vaultTransit);
        Assert.Equal(typeof(VaultTransit), vaultTransit.ImplementationType);

        Assert.NotNull(vaultOptions);
        Assert.Equal("http://localhost:8200", vaultOptions.Address);
        Assert.Equal("role-id", vaultOptions.RoleId);
        Assert.Equal("secret-id", vaultOptions.SecretId);
        Assert.Equal("unit-test", vaultOptions.Solution);
        Assert.Equal("my-app", vaultOptions.AppName);
    }

    [Fact]
    public void AddVault_ConfigurationBuilder_OvverideAppSettings()
    {
        // Wait for the vault container to be ready (RabbitMQ)
        Thread.Sleep(20000);

        var credentials = VaultContainer.GetCredentials();

        var server = new TestServer(new WebHostBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                builder.AddVault(options =>
                {
                    options.Address = $"http://localhost:{fixture.Container.Port}";
                    options.RoleId = credentials.RoleId;
                    options.SecretId = credentials.SecretId;
                    options.Solution = "unit-test";
                    options.KeyVault.Enable = true;
                    options.Mongo.Enable = true;
                    options.RabbitMQ.Enable = true;
                    options.AppName = "my-app";
                });
            }).ConfigureServices(services =>
            {

            }).Configure(app =>
            {
                var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();

                var securityOptions = configuration.GetSection("Security").Get<SecurityOptions>();
                var mongoOptions = configuration.GetSection("Mongo").Get<MongoOptions>();
                var rabbitMQOptions = configuration.GetSection("RabbitMQ").Get<RabbitMQOptions>();

                Assert.NotNull(securityOptions);
                Assert.NotEmpty(securityOptions.ValidAudiences);
                Assert.NotNull(securityOptions.ClientId);

                Assert.NotNull(mongoOptions);
                Assert.NotNull(mongoOptions.ConnectionString);
                Assert.NotEqual("mongodb://localhost:27017", mongoOptions.ConnectionString);

                Assert.NotNull(rabbitMQOptions);
                Assert.NotNull(rabbitMQOptions.UserName);
                Assert.NotNull(rabbitMQOptions.Password);
            })
        );

        Assert.NotNull(server);
    }

}
