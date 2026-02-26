using System;
using Xunit;
using CodeDesignPlus.Net.ValueObjects.Financial;

namespace CodeDesignPlus.Net.ValueObjects.Test.Location;

public class MoneyTest
{
    [Fact]
    public void Constructor_NormalizesCurrency()
    {
        var money = new Money(10.5m, " usd ");
        Assert.Equal("USD", money.Currency);
        Assert.Equal(10.5m, money.Amount);
    }

    [Fact]
    public void Constructor_ThrowsIfCurrencyNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Money(10m, null));
    }

    [Fact]
    public void FromDecimal_CreatesCorrectInstance()
    {
        var money = Money.FromDecimal(20.25m, "EUR");
        Assert.Equal(20.25m, money.Amount);
        Assert.Equal("EUR", money.Currency);
    }

    [Fact]
    public void FromLong_CreatesCorrectInstance()
    {
        var money = Money.FromLong(12345, "USD", 2);
        Assert.Equal(123.45m, money.Amount);
        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void FromLong_ThrowsIfDecimalPlacesNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Money.FromLong(100, "USD", -1));
    }

    [Fact]
    public void Zero_NoCurrency_ReturnsXXX()
    {
        var money = Money.Zero();
        Assert.Equal(0m, money.Amount);
        Assert.Equal("XXX", money.Currency);
    }

    [Fact]
    public void Zero_WithCurrency_ReturnsZeroAmount()
    {
        var money = Money.Zero("COP");
        Assert.Equal(0m, money.Amount);
        Assert.Equal("COP", money.Currency);
    }

    [Fact]
    public void ToLong_ConvertsCorrectly()
    {
        var money = new Money(12.345m, "USD");
        var result = money.ToLong(2);
        Assert.Equal(1235, result); // 12.345 * 100 = 1234.5 => 1235 (AwayFromZero)
    }

    [Fact]
    public void ToLong_ThrowsIfDecimalPlacesNegative()
    {
        var money = new Money(1m, "USD");
        Assert.Throws<ArgumentOutOfRangeException>(() => money.ToLong(-1));
    }

    [Fact]
    public void ToDecimal_ReturnsAmount()
    {
        var money = new Money(99.99m, "USD");
        Assert.Equal(99.99m, money.ToDecimal());
    }

    [Fact]
    public void Operator_Add_SameCurrency()
    {
        var a = new Money(10m, "USD");
        var b = new Money(5m, "USD");
        var result = a + b;
        Assert.Equal(15m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Operator_Add_DifferentCurrency_Throws()
    {
        var a = new Money(10m, "USD");
        var b = new Money(5m, "EUR");
        Assert.Throws<InvalidOperationException>(() => { var _ = a + b; });
    }

    [Fact]
    public void Operator_Subtract_SameCurrency()
    {
        var a = new Money(10m, "USD");
        var b = new Money(3m, "USD");
        var result = a - b;
        Assert.Equal(7m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Operator_Subtract_DifferentCurrency_Throws()
    {
        var a = new Money(10m, "USD");
        var b = new Money(3m, "EUR");
        Assert.Throws<InvalidOperationException>(() => { var _ = a - b; });
    }

    [Fact]
    public void Operator_Multiply_Decimal()
    {
        var money = new Money(10m, "USD");
        var result = money * 2.5m;
        Assert.Equal(25m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Operator_Multiply_Long()
    {
        var money = new Money(10m, "USD");
        var result = money * 3L;
        Assert.Equal(30m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Operator_Multiply_Decimal_Left()
    {
        var money = new Money(10m, "USD");
        var result = 2.5m * money;
        Assert.Equal(25m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Operator_Multiply_Long_Left()
    {
        var money = new Money(10m, "USD");
        var result = 3L * money;
        Assert.Equal(30m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Operator_Equality()
    {
        var a = new Money(10m, "USD");
        var b = new Money(10m, "USD");
        Assert.True(a == b);
        Assert.False(a != b);
    }

    [Fact]
    public void Operator_Inequality()
    {
        var a = new Money(10m, "USD");
        var b = new Money(20m, "USD");
        var c = new Money(10m, "EUR");
        Assert.True(a != b);
        Assert.True(a != c);
    }

    [Fact]
    public void Equals_ObjectAndMoney()
    {
        var a = new Money(10m, "USD");
        object b = new Money(10m, "USD");
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void GetHashCode_SameForEqualInstances()
    {
        var a = new Money(10m, "USD");
        var b = new Money(10m, "USD");
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Min_ReturnsSmallerAmount()
    {
        var a = new Money(5m, "USD");
        var b = new Money(10m, "USD");
        var min = Money.Min(a, b);
        Assert.Equal(5m, min.Amount);
    }

    [Fact]
    public void Min_DifferentCurrency_Throws()
    {
        var a = new Money(5m, "USD");
        var b = new Money(10m, "EUR");
        Assert.Throws<InvalidOperationException>(() => Money.Min(a, b));
    }

    [Fact]
    public void Max_ReturnsLargerAmount()
    {
        var a = new Money(5m, "USD");
        var b = new Money(10m, "USD");
        var max = Money.Max(a, b);
        Assert.Equal(10m, max.Amount);
    }

    [Fact]
    public void Max_DifferentCurrency_Throws()
    {
        var a = new Money(5m, "USD");
        var b = new Money(10m, "EUR");
        Assert.Throws<InvalidOperationException>(() => Money.Max(a, b));
    }
}
