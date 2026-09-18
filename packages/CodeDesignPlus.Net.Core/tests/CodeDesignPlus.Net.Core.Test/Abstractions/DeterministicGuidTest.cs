namespace CodeDesignPlus.Net.Core.Test.Abstractions;

/// <summary>
/// Covers the name-based identifier generator.
/// </summary>
/// <remarks>
/// The interoperability test is the one that matters: the value has to match what any other RFC 4122
/// implementation produces for the same namespace and name. Without the byte-order conversion the algorithm
/// still looks correct and still returns a stable identifier, but a different one, so nothing outside .NET
/// could ever reproduce it.
/// </remarks>
public class DeterministicGuidTest
{
    private static readonly Guid Namespace = Guid.Parse("6f1b0a3c-9d47-4a2e-b8f5-1c7e2d4a6b90");

    [Fact]
    public void Create_SameNamespaceAndName_ReturnsSameGuid()
    {
        // Arrange
        const string name = "tenant|source|2026-09";

        // Act
        var first = DeterministicGuid.Create(Namespace, name);
        var second = DeterministicGuid.Create(Namespace, name);

        // Assert
        Assert.Equal(first, second);
    }

    [Fact]
    public void Create_DifferentName_ReturnsDifferentGuid()
    {
        // Arrange & Act
        var september = DeterministicGuid.Create(Namespace, "tenant|source|2026-09");
        var october = DeterministicGuid.Create(Namespace, "tenant|source|2026-10");

        // Assert
        Assert.NotEqual(september, october);
    }

    [Fact]
    public void Create_DifferentNamespace_ReturnsDifferentGuid()
    {
        // Arrange
        var other = Guid.Parse("2b8e4d17-5a63-4c90-9e28-7f0a1c3b5d64");

        // Act
        var first = DeterministicGuid.Create(Namespace, "same-name");
        var second = DeterministicGuid.Create(other, "same-name");

        // Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Create_AnyName_ReturnsVersionFiveGuid()
    {
        // Arrange & Act
        var result = DeterministicGuid.Create(Namespace, "any-name").ToString();

        // Assert
        Assert.Equal('5', result[14]);
    }

    [Fact]
    public void Create_AnyName_ReturnsRfc4122Variant()
    {
        // Arrange & Act
        var result = DeterministicGuid.Create(Namespace, "any-name").ToString();

        // Assert
        Assert.Contains(result[19], "89ab");
    }

    [Fact]
    public void Create_KnownVector_MatchesRfc4122()
    {
        // Arrange
        // The DNS namespace and "www.example.org" are the reference vector of the RFC, and every
        // implementation outside .NET returns this same value.
        var dns = Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8");

        // Act
        var result = DeterministicGuid.Create(dns, "www.example.org");

        // Assert
        Assert.Equal(Guid.Parse("74738ff5-5367-5958-9aee-98fffdcd1876"), result);
    }

    [Fact]
    public void Create_NullName_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => DeterministicGuid.Create(Namespace, (string)null!));
    }

    [Fact]
    public void Create_EmptyName_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => DeterministicGuid.Create(Namespace, string.Empty));
    }

    [Fact]
    public void Create_Parts_JoinsWithSeparator()
    {
        // Arrange & Act
        var fromParts = DeterministicGuid.Create(Namespace, "tenant", "source", "2026-09");
        var fromName = DeterministicGuid.Create(Namespace, "tenant|source|2026-09");

        // Assert
        Assert.Equal(fromName, fromParts);
    }

    [Fact]
    public void Create_PartsThatWouldConcatenateAlike_ReturnsDifferentGuid()
    {
        // Arrange
        // Without a separator both keys collapse into "ab|c" versus "a|bc" written as "abc".
        // Act
        var first = DeterministicGuid.Create(Namespace, "ab", "c");
        var second = DeterministicGuid.Create(Namespace, "a", "bc");

        // Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Create_NoParts_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => DeterministicGuid.Create(Namespace, []));
    }
}
