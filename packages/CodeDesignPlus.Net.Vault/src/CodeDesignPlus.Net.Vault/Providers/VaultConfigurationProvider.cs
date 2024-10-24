using System;
using CodeDesignPlus.Net.Vault.Abstractions.Options;
using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;

namespace CodeDesignPlus.Net.Vault.Providers;

public class VaultConfigurationProvider : ConfigurationProvider
{
    public VaultOptions config;
    private IVaultClient client;

    public VaultConfigurationProvider(VaultOptions config)
    {
        this.config = config;

        var vaultClientSettings = new VaultClientSettings(
            this.config.Address,
            new AppRoleAuthMethodInfo(this.config.RoleId, this.config.SecretId)
        );

        this.client = new VaultClient(vaultClientSettings);
    }

    public override void Load()
    {
        LoadAsync().Wait();
    }

    public async Task LoadAsync()
    {
        await GetDatabaseCredentials();
    }

    public async Task GetDatabaseCredentials()
    {
        if (this.config.KeyVault.Enable)
        {
            var data = await client.V1.Secrets.KeyValue.V2.ReadSecretAsync(config.AppName, null, $"{config.Solution}-keyvalue");

            foreach (var item in data.Data.Data)
            {
                base.Data.Add(item.Key, item.Value.ToString());
            }
        }

        if (this.config.Mongo.Enable)
        {
            var credentials = await client.V1.Secrets.Database.GetCredentialsAsync($"{config.AppName}-{config.Mongo.RoleSufix}", mountPoint: $"{config.Solution}-database");

            var connectionString = string.Format(config.Mongo.TemplateConnectionString, credentials.Data.Username, credentials.Data.Password);

            base.Data.Add("Mongo:ConnectionString", connectionString);
        }

        if (this.config.RabbitMQ.Enable)
        {
            var credentials = await client.V1.Secrets.RabbitMQ.GetCredentialsAsync($"{config.AppName}-{config.RabbitMQ.RoleSufix}", mountPoint: $"{config.Solution}-rabbitmq");

            base.Data.Add("RabbitMQ:UserName", credentials.Data.Username);
            base.Data.Add("RabbitMQ:Password", credentials.Data.Password);
        }
    }
}
