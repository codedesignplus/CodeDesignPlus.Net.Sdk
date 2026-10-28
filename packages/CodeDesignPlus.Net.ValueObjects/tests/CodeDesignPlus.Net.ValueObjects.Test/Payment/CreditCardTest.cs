using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.ValueObjects.Payment;

namespace CodeDesignPlus.Net.ValueObjects.Test.Payment;

/// <summary>
/// Covers <see cref="CreditCard"/>, a snapshot of a tokenized card that holds nothing PCI DSS forbids keeping.
/// </summary>
/// <remarks>
/// The card used to carry the security code because the gateway needed it to authorize, and that made it travel
/// from the browser through every service. The card is now entered on the provider's hosted page, so the code
/// never reaches Kappali (pendings/085 and 660).
/// </remarks>
public class CreditCardTest
{
    [Fact]
    public void Create_ValidParameters_ReturnsCardWithoutSecurityCode()
    {
        // Act
        var card = CreditCard.Create("tok_test_123", "4242", "2030/12", "Test User");

        // Assert
        Assert.Equal(("tok_test_123", "4242", "2030/12", "TEST USER", (int?)1), (card.Token, card.Last4Digits, card.ExpirationDate, card.CardHolderName, card.InstallmentsNumber));
    }

    [Fact]
    public void CreditCard_Members_HaveNoSecurityCode()
    {
        // Act
        var property = typeof(CreditCard).GetProperty("SecurityCode");

        // Assert
        Assert.Null(property);
    }

    [Fact]
    public void Deserialize_JsonWithSecurityCode_IgnoresIt()
    {
        // Arrange
        var json = """{"Token":"tok_test_123","Last4Digits":"4242","ExpirationDate":"2030/12","CardHolderName":"Test User","SecurityCode":"123","InstallmentsNumber":3}""";

        // Act
        var card = Newtonsoft.Json.JsonConvert.DeserializeObject<CreditCard>(json)!;

        // Assert
        Assert.Equal(CreditCard.Create("tok_test_123", "4242", "2030/12", "Test User", 3), card);
    }

    [Fact]
    public void Create_InvalidLast4Digits_ThrowsCodeDesignPlusException()
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => CreditCard.Create("tok_test_123", "42", "2030/12", "Test User"));

        Assert.Equal(Errors.CreditCardLast4DigitsMustBeExactly.Code, exception.Code);
    }
}
