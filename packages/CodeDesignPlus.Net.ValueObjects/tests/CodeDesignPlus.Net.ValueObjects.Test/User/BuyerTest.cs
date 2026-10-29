using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.ValueObjects.User;

namespace CodeDesignPlus.Net.ValueObjects.Test.User;

/// <summary>
/// Covers <see cref="Buyer"/>: phone and email are optional, and validated as always when they come.
/// </summary>
/// <remarks>
/// A visitor paying at the front desk is known only by name and document (Kappali pendings/665); the provider's hosted
/// page asks for what is missing. Requiring them made the payment fail before it reached the provider.
/// </remarks>
public class BuyerTest
{
    private static readonly Guid BuyerId = Guid.Parse("cc000000-0000-4000-8000-000000000003");

    [Fact]
    public void CreateWithoutShipping_NoPhoneNorEmail_KeepsThemEmpty()
    {
        // Act
        var buyer = Buyer.CreateWithoutShipping(BuyerId, "Ana Restrepo", null!, "  ", TypeDocument.Create("CC", "Cédula de ciudadanía"), "1020304050");

        // Assert
        Assert.Equal((string.Empty, string.Empty, "1020304050"), (buyer.Phone, buyer.Email, buyer.Document));
    }

    [Fact]
    public void CreateMinimal_NoPhoneNorEmail_IsAccepted()
    {
        // Act
        var buyer = Buyer.CreateMinimal(BuyerId, "Ana Restrepo", "", "");

        // Assert
        Assert.Equal((string.Empty, string.Empty), (buyer.Phone, buyer.Email));
    }

    [Fact]
    public void CreateMinimal_GivenPhoneAndEmail_AreNormalizedAsBefore()
    {
        // Act
        var buyer = Buyer.CreateMinimal(BuyerId, "Ana Restrepo", " +573001112233 ", "Ana@Example.com");

        // Assert
        Assert.Equal(("+573001112233", "ana@example.com"), (buyer.Phone, buyer.Email));
    }

    [Fact]
    public void CreateMinimal_InvalidEmail_IsStillRejected()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Buyer.CreateMinimal(BuyerId, "Ana Restrepo", "", "ana-at-example"));

        // Assert
        Assert.Equal(Errors.EmailContainsInvalidCharacters.Code, exception.Code);
    }

    [Fact]
    public void CreateMinimal_InvalidPhone_IsStillRejected()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Buyer.CreateMinimal(BuyerId, "Ana Restrepo", "300-ABC", ""));

        // Assert
        Assert.Equal(Errors.PhoneContainsInvalidCharacters.Code, exception.Code);
    }
}
