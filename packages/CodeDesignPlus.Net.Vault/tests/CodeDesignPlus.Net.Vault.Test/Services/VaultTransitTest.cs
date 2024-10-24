using System;
using CodeDesignPlus.Net.xUnit.Helpers.VaultContainer;
using Microsoft.AspNetCore.Hosting;
using CodeDesignPlus.Net.Vault.Extensions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeDesignPlus.Net.Vault.Test.Services;


[Collection(VaultCollectionFixture.Collection)]
public class VaultTransitTest(VaultCollectionFixture fixture)
{

    [Fact]
    public async Task Temp()
    {
        await Task.Delay(16000);

        var credentials = VaultContainer.GetCredentials();

        await Task.Delay(5000);

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

        var (key, ciphertext) = await transit.EncryptAsync("Custom Value");

        var decrypt = await transit.DecryptAsync(key, ciphertext);
            

    }

}
