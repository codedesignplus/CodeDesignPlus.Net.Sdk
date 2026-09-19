using CodeDesignPlus.Net.ServiceBus.Test.Helpers.Entities;

namespace CodeDesignPlus.Net.ServiceBus.Test.Services;

public class SubscriptionNameResolverTest
{
    private static SubscriptionNameResolver BuildResolver()
        => new(Microsoft.Extensions.Options.Options.Create(ConfigurationUtil.CoreOptions));

    [QueueName<NotificationEntity>("short_action")]
    private sealed class ShortHandler { }

    [QueueName<NotificationEntity>("desactivate_unit_ownership_projection_handler")]
    private sealed class VeryLongHandler { }

    // Mismo appName y misma accion que VeryLongHandler, distinta entidad: el nombre corto coincide y solo el
    // nombre logico los distingue.
    [QueueName<UserEntity>("desactivate_unit_ownership_projection_handler")]
    private sealed class VeryLongHandlerOtherEntity { }

    private sealed class HandlerWithoutAttribute { }

    [Fact]
    public void GetSubscriptionName_NameFits_ReturnsReadableName()
    {
        var resolver = BuildResolver();

        var name = resolver.GetSubscriptionName(typeof(ShortHandler));

        Assert.Equal("test-servicebus.short_action", name);
        Assert.True(name.Length <= SubscriptionNameResolver.MaxLength);
    }

    [Fact]
    public void GetSubscriptionName_NameTooLong_TruncatesAndAppendsDiscriminator()
    {
        var resolver = BuildResolver();

        var name = resolver.GetSubscriptionName(typeof(VeryLongHandler));

        Assert.Equal(SubscriptionNameResolver.MaxLength, name.Length);
        Assert.StartsWith("test-servicebus.desactivate_unit", name);
        Assert.Equal('-', name[^(SubscriptionNameResolver.HashLength + 1)]);
        Assert.All(name[^SubscriptionNameResolver.HashLength..], x => Assert.True(char.IsAsciiHexDigitLower(x)));
    }

    [Fact]
    public void GetSubscriptionName_SameHandler_IsDeterministic()
    {
        var first = BuildResolver().GetSubscriptionName(typeof(VeryLongHandler));
        var second = BuildResolver().GetSubscriptionName(typeof(VeryLongHandler));

        Assert.Equal(first, second);
    }

    [Fact]
    public void GetSubscriptionName_SameActionDifferentEntity_ReturnsDifferentName()
    {
        var resolver = BuildResolver();

        var first = resolver.GetSubscriptionName(typeof(VeryLongHandler));
        var second = resolver.GetSubscriptionName(typeof(VeryLongHandlerOtherEntity));

        // El prefijo recortado es identico; lo unico que los separa es el resumen del nombre logico completo.
        Assert.Equal(first[..^SubscriptionNameResolver.HashLength], second[..^SubscriptionNameResolver.HashLength]);
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void GetSubscriptionName_HandlerWithoutAttribute_ThrowServiceBusPubSubException()
    {
        var resolver = BuildResolver();

        var exception = Assert.Throws<ServiceBusPubSubException>(() => resolver.GetSubscriptionName(typeof(HandlerWithoutAttribute)));

        Assert.Contains(nameof(HandlerWithoutAttribute), exception.Message);
    }

    [Fact]
    public void GetSubscriptionName_TypeIsNull_ThrowArgumentNullException()
    {
        var resolver = BuildResolver();

        Assert.Throws<ArgumentNullException>(() => resolver.GetSubscriptionName(null!));
    }

    [Fact]
    public void Constructor_CoreOptionsIsNull_ThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SubscriptionNameResolver(null!));
    }

    [Theory]
    [InlineData("Ms-Accounting.DocumentPaidHandler", "ms-accounting.documentpaidhandler")]
    [InlineData("ms accounting/handler", "ms-accounting-handler")]
    [InlineData("--ms-accounting--", "ms-accounting")]
    [InlineData("ms_accounting.v1-handler", "ms_accounting.v1-handler")]
    public void Sanitize_InvalidCharacters_ReturnsValidName(string value, string expected)
    {
        Assert.Equal(expected, SubscriptionNameResolver.Sanitize(value));
    }

    [Fact]
    public void Sanitize_ValueIsEmpty_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, SubscriptionNameResolver.Sanitize("   "));
    }

    [Fact]
    public void Discriminator_SameInput_ReturnsSameEightHexCharacters()
    {
        var first = SubscriptionNameResolver.Discriminator("kappali.ms-administration.v1.unitownershipprojectionaggregate.desactivate");
        var second = SubscriptionNameResolver.Discriminator("kappali.ms-administration.v1.unitownershipprojectionaggregate.desactivate");

        Assert.Equal(first, second);
        Assert.Equal(SubscriptionNameResolver.HashLength, first.Length);
        Assert.All(first, x => Assert.True(char.IsAsciiHexDigitLower(x)));
    }

    [Fact]
    public void Discriminator_DifferentInput_ReturnsDifferentValue()
    {
        Assert.NotEqual(SubscriptionNameResolver.Discriminator("a"), SubscriptionNameResolver.Discriminator("b"));
    }
}
