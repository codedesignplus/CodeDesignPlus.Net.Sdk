using CodeDesignPlus.Net.Vault.Services;
using VaultSharp.Core;
using VaultSharp.V1.SecretsEngines.Transit;

namespace CodeDesignPlus.Net.Vault.Test.Services;

/// <summary>
/// Covers the fixed-key overloads of <see cref="IVaultTransit"/> against a real Vault: one key per kind of secret,
/// derived per context, instead of one key per call.
/// </summary>
/// <remarks>
/// Kappali's ms-payments encrypts the provider keys of every tenant and rotates them. With a new key per call that
/// leaves tens of thousands of keys in Vault, many of them orphaned (pendings/660).
/// </remarks>
[Collection(VaultCollectionFixture.Collection)]
public class VaultTransitFixedKeyTest(VaultCollectionFixture fixture)
{
    private const string MountPoint = "unit-test-transit";

    private ServiceProvider BuildServiceProvider()
    {
        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>() {
            { "Vault:RoleId", fixture.Container.Credentials.RoleId },
            { "Vault:SecretId", fixture.Container.Credentials.SecretId },
            { "Vault:Address", $"http://localhost:{fixture.Container.Port}" }
        });

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddVault(configurationBuilder.Build());

        return serviceCollection.BuildServiceProvider();
    }

    private static string NewKeyName() => $"fixed-{Guid.NewGuid():N}";

    [Fact]
    public async Task EncryptAsync_FixedKey_DecryptsWithTheSameContext()
    {
        // Arrange
        var transit = BuildServiceProvider().GetRequiredService<IVaultTransit>();
        var keyName = NewKeyName();
        var context = Guid.NewGuid().ToString();

        // Act
        var ciphertext = await transit.EncryptAsync(keyName, "prv_test_secret", context);

        // Assert
        Assert.Equal("prv_test_secret", await transit.DecryptAsync(keyName, ciphertext, context));
    }

    /// <summary>
    /// The context isolates the owners: a secret encrypted for one tenant cannot be decrypted as another's.
    /// </summary>
    [Fact]
    public async Task DecryptAsync_FixedKeyWithAnotherContext_Fails()
    {
        // Arrange
        var transit = BuildServiceProvider().GetRequiredService<IVaultTransit>();
        var keyName = NewKeyName();

        var ciphertext = await transit.EncryptAsync(keyName, "prv_test_secret", Guid.NewGuid().ToString());

        // Act & Assert
        await Assert.ThrowsAsync<VaultApiException>(() => transit.DecryptAsync(keyName, ciphertext, Guid.NewGuid().ToString()));
    }

    [Fact]
    public async Task EncryptAsync_FixedKey_CreatesTheKeyDerived()
    {
        // Arrange
        var provider = BuildServiceProvider();
        var transit = provider.GetRequiredService<IVaultTransit>();
        var keyName = NewKeyName();

        // Act
        await transit.EncryptAsync(keyName, "prv_test_secret", Guid.NewGuid().ToString());

        // Assert
        var key = await provider.GetRequiredService<VaultClientProvider>().Client.V1.Secrets.Transit.ReadEncryptionKeyAsync(keyName, MountPoint);

        Assert.True(key.Data.Derived);
    }

    /// <summary>
    /// Encrypting for many owners, and again for the same one, reuses the key: Vault does not gain one per call.
    /// </summary>
    [Fact]
    public async Task EncryptAsync_FixedKeyForManyContexts_KeepsOneKey()
    {
        // Arrange
        var provider = BuildServiceProvider();
        var transit = provider.GetRequiredService<IVaultTransit>();
        var keyName = NewKeyName();
        var vaultTransit = provider.GetRequiredService<VaultClientProvider>().Client.V1.Secrets.Transit;

        await transit.EncryptAsync(keyName, "first", Guid.NewGuid().ToString());

        var before = (await vaultTransit.ReadAllEncryptionKeysAsync(MountPoint)).Data.Keys.Count();

        // Act
        foreach (var _ in Enumerable.Range(0, 5))
        {
            await transit.EncryptAsync(keyName, "rotated", Guid.NewGuid().ToString());
        }

        // Assert
        var after = (await vaultTransit.ReadAllEncryptionKeysAsync(MountPoint)).Data.Keys.Count();

        Assert.Equal(before, after);
    }

    /// <summary>
    /// Two replicas using a new key at the same time end with one derived key, and both can decrypt.
    /// </summary>
    [Fact]
    public async Task EncryptAsync_NewKeyFromConcurrentInstances_AllDecrypt()
    {
        // Arrange
        var keyName = NewKeyName();
        var context = Guid.NewGuid().ToString();
        var transits = Enumerable.Range(0, 8).Select(_ => BuildServiceProvider().GetRequiredService<IVaultTransit>()).ToList();

        // Act
        var ciphertexts = await Task.WhenAll(transits.Select((transit, index) => transit.EncryptAsync(keyName, $"secret-{index}", context)));

        // Assert
        var plaintexts = await Task.WhenAll(ciphertexts.Select(ciphertext => transits[0].DecryptAsync(keyName, ciphertext, context)));

        Assert.Equal(Enumerable.Range(0, 8).Select(index => $"secret-{index}"), plaintexts);
    }

    /// <summary>
    /// A key that already exists without derivation would ignore the context; it is refused instead of used.
    /// </summary>
    [Fact]
    public async Task EncryptAsync_KeyExistsWithoutDerivation_ThrowsVaultException()
    {
        // Arrange
        var provider = BuildServiceProvider();
        var keyName = NewKeyName();

        await provider.GetRequiredService<VaultClientProvider>().Client.V1.Secrets.Transit.CreateEncryptionKeyAsync(keyName, new CreateKeyRequestOptions { Type = TransitKeyType.aes256_gcm96 }, MountPoint);

        var transit = provider.GetRequiredService<IVaultTransit>();

        // Act & Assert
        await Assert.ThrowsAsync<VaultException>(() => transit.EncryptAsync(keyName, "prv_test_secret", Guid.NewGuid().ToString()));
    }

    [Fact]
    public async Task EncryptAsync_FixedKeyList_DecryptsInOrder()
    {
        // Arrange
        var transit = BuildServiceProvider().GetRequiredService<IVaultTransit>();
        var keyName = NewKeyName();
        var context = Guid.NewGuid().ToString();

        // Act
        var ciphertexts = await transit.EncryptAsync(keyName, ["prv_test_secret", "test_events_secret", "test_integrity_secret"], context);

        // Assert
        Assert.Equal(["prv_test_secret", "test_events_secret", "test_integrity_secret"], await transit.DecryptAsync(keyName, ciphertexts, context));
    }

    [Fact]
    public async Task EncryptAsync_FixedKeyEmptyList_ReturnsEmptyWithoutCreatingTheKey()
    {
        // Arrange
        var provider = BuildServiceProvider();
        var transit = provider.GetRequiredService<IVaultTransit>();
        var keyName = NewKeyName();

        // Act
        var ciphertexts = await transit.EncryptAsync(keyName, [], Guid.NewGuid().ToString());

        // Assert
        var keys = (await provider.GetRequiredService<VaultClientProvider>().Client.V1.Secrets.Transit.ReadAllEncryptionKeysAsync(MountPoint)).Data.Keys;

        Assert.Equal((0, false), (ciphertexts.Count, keys.Contains(keyName)));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task EncryptAsync_FixedKeyWithoutContext_ThrowsArgumentException(string context)
    {
        // Arrange
        var transit = BuildServiceProvider().GetRequiredService<IVaultTransit>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => transit.EncryptAsync(NewKeyName(), "prv_test_secret", context));
    }
}
