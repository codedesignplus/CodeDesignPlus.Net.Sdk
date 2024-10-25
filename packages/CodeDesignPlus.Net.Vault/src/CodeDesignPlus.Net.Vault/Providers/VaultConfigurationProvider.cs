using CodeDesignPlus.Net.Vault.Abstractions.Options;
using CodeDesignPlus.Net.Vault.Services;
using Microsoft.Extensions.Configuration;
using VaultSharp;

namespace CodeDesignPlus.Net.Vault.Providers;


/// <summary>
/// Provides configuration settings from a Vault service.
/// </summary>
public class VaultConfigurationProvider : ConfigurationProvider
{
    /// <summary>
    /// The configuration options for the Vault.
    /// </summary>
    public readonly VaultOptions config;

    /// <summary>
    /// The client used to interact with the Vault service.
    /// </summary>
    private readonly IVaultClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="VaultConfigurationProvider"/> class.
    /// </summary>
    /// <param name="config">The configuration options for the Vault.</param>
    /// <exception cref="ArgumentNullException">config is null.</exception>
    public VaultConfigurationProvider(VaultOptions config)
    {
        ArgumentNullException.ThrowIfNull(config);

        this.config = config;
        this.client = VaultClientFactory.Create(this.config);
    }

    /// <summary>
    /// Loads the configuration settings from the Vault service.
    /// </summary>
    public override void Load()
    {
        LoadAsync().Wait();
    }

    /// <summary>
    /// Asynchronously loads the configuration settings from the Vault service.
    /// </summary>
    /// <returns>A task that represents the asynchronous load operation.</returns>
    public async Task LoadAsync()
    {
        await GetDatabaseCredentials();
    }

    /// <summary>
    /// Retrieves database credentials from the Vault service and adds them to the configuration data.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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




