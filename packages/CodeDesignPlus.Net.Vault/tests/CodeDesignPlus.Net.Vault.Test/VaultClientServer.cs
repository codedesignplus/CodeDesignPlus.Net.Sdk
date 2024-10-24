using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using CodeDesignPlus.Net.Vault.Extensions;
using CodeDesignPlus.Net.xUnit.Helpers.VaultContainer;

namespace CodeDesignPlus.Net.Vault.Test;

[Collection(VaultCollectionFixture.Collection)]
public class VaultClientServer(VaultCollectionFixture fixture)
{

    [Fact]
    public void Temp()
    {
        var credentials = VaultContainer.GetCredentials();
        
        Thread.Sleep(16000);

        var server = new TestServer(new WebHostBuilder()
            .ConfigureAppConfiguration((context, builder) =>
            {
                var c = context.Configuration;

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

                var sectionSecurity = configuration.GetSection("Security");
                var securityOptions = sectionSecurity.Get<SecurityOptions>();
            })
        );
    }

}

public class SecurityOptions
{
    public string? ClientId { get; set; }
    public string[] ValidAudiences { get; set; } = [];
}
