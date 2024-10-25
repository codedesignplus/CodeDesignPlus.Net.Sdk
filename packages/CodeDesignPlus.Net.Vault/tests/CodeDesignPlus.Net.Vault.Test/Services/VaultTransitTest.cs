namespace CodeDesignPlus.Net.Vault.Test.Services;

[Collection(VaultCollectionFixture.Collection)]
public class VaultTransitTest(VaultCollectionFixture fixture)
{

    [Fact]
    public async Task Encrypt_Decrypt_Sucessfully()
    {
        await Task.Delay(20000);

        var credentials = VaultContainer.GetCredentials();

        await Task.Delay(1000);

        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>() {
            { "Vault:RoleId", credentials.RoleId },
            { "Vault:SecretId", credentials.SecretId },
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
        await Task.Delay(20000);

        var credentials = VaultContainer.GetCredentials();

        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>() {
            { "Vault:RoleId", credentials.RoleId },
            { "Vault:SecretId", credentials.SecretId },
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

}
