using CodeDesignPlus.Net.xUnit.Helpers.VaultContainer;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.AppRole;

namespace CodeDesignPlus.Net.xUnit.Test;

[Collection(VaultCollectionFixture.Collection)]
public class VaultContainerTest(VaultCollectionFixture fixture)
{

    [Fact]
    public async Task CheckConnectionService()
    {
        var credentials = VaultContainer.GetCredentials();

        IAuthMethodInfo authMethod = new AppRoleAuthMethodInfo(credentials.RoleId, credentials.SecretId);
        var vaultClientSettings = new VaultClientSettings($"http://localhost:{fixture.Container.Port}", authMethod);

        IVaultClient vaultClient = new VaultClient(vaultClientSettings);

        var kv1Secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("puc/appid", mountPoint: "accounting");

        // key = foo, value = world2

        Assert.True(kv1Secret.Data.Data.ContainsKey("foo"));
        Assert.Equal("world2", kv1Secret.Data.Data["foo"]);
    }

    
}
