using CodeDesignPlus.Net.xUnit.Containers.VaultContainer;
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
        IAuthMethodInfo authMethod = new AppRoleAuthMethodInfo(fixture.Container.Credentials.RoleId, fixture.Container.Credentials.SecretId);
        var vaultClientSettings = new VaultClientSettings($"http://localhost:{fixture.Container.Port}", authMethod);

        IVaultClient vaultClient = new VaultClient(vaultClientSettings);

        var kv1Secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("my-app", mountPoint: "unit-test-keyvalue");

        Assert.True(kv1Secret.Data.Data.ContainsKey("Security:ClientId"));
        Assert.Equal("a74cb192-598c-4757-95ae-b315793bbbca", kv1Secret.Data.Data["Security:ClientId"]);
    }
}
