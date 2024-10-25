namespace CodeDesignPlus.Net.Vault.Test.Services;

[Collection(VaultCollectionFixture.Collection)]
public class VaultTransitTest(VaultCollectionFixture fixture)
{

    [Fact]
    public async Task Encrypt_Decrypt_Sucessfully()
    {
        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>() {
            { "Vault:RoleId", fixture.Container.Credentials.RoleId },
            { "Vault:SecretId", fixture.Container.Credentials.SecretId },
            { "Vault:Address", $"http://localhost:{fixture.Container.Port}" }
        });

        var configuration = configurationBuilder.Build();

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddVault(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();


        var transit = serviceProvider.GetRequiredService<IVaultTransit>();

        var context = Guid.NewGuid().ToString();

        var (key, ciphertext) = await transit.EncryptAsync("Custom Value", context);

        var decrypt = await transit.DecryptAsync(key, ciphertext, context);

        Assert.Equal("Custom Value", decrypt);
    }

    [Fact]
    public async Task Encrypt_Decrypt_List_Sucessfully()
    {
        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>() {
            { "Vault:RoleId", fixture.Container.Credentials.RoleId },
            { "Vault:SecretId", fixture.Container.Credentials.SecretId },
            { "Vault:Address", $"http://localhost:{fixture.Container.Port}" }
        });

        var configuration = configurationBuilder.Build();

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddVault(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var transit = serviceProvider.GetRequiredService<IVaultTransit>();

        var context = Guid.NewGuid().ToString();

        var (key, items) = await transit.EncryptAsync(["Custom Value 1", "Custom Value 2"], context);

        var decrypt = await transit.DecryptAsync(key, items, context);

        Assert.Contains("Custom Value 1", decrypt);
        Assert.Contains("Custom Value 2", decrypt);
    }

    [Fact]
    public async Task EncryptAsync_PlainTextIsEmpty_ReturnTuplaEmpty()
    {
        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>() {
            { "Vault:RoleId", fixture.Container.Credentials.RoleId },
            { "Vault:SecretId", fixture.Container.Credentials.SecretId },
            { "Vault:Address", $"http://localhost:{fixture.Container.Port}" }
        });

        var configuration = configurationBuilder.Build();

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddVault(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var transit = serviceProvider.GetRequiredService<IVaultTransit>();

        var context = Guid.NewGuid().ToString();

        var (key, ciphertext) = await transit.EncryptAsync([], context);

        Assert.Equal(Guid.Empty.ToString(), key);
        Assert.Empty(ciphertext);
    }

    
    [Fact]
    public async Task DencryptAsync_CipherIsEmpty_ReturnEmpty() {
        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>() {
            { "Vault:RoleId", fixture.Container.Credentials.RoleId },
            { "Vault:SecretId", fixture.Container.Credentials.SecretId },
            { "Vault:Address", $"http://localhost:{fixture.Container.Port}" }
        });

        var configuration = configurationBuilder.Build();

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddVault(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var transit = serviceProvider.GetRequiredService<IVaultTransit>();

        var context = Guid.NewGuid().ToString();

        var decrypt = await transit.DecryptAsync(Guid.Empty.ToString(), [], context);

        Assert.Empty(decrypt);
    }

}
