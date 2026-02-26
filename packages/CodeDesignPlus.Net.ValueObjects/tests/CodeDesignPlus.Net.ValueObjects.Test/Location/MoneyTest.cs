using System;
using Xunit;
using CodeDesignPlus.Net.ValueObjects.Financial;

namespace CodeDesignPlus.Net.ValueObjects.Test.Location
{
    public class MoneyTest
    {
        [Fact]
        public void FromDecimal_CreatesCorrectMinorUnit()
        {
            var money = Money.FromDecimal(10.55m, "usd", 2);
            Assert.Equal(1055, money.Amount);
            Assert.Equal("USD", money.Currency);
        }

        [Fact]
        public void FromLong_CreatesCorrectMoney()
        {
            var money = Money.FromLong(1234, "eur");
            Assert.Equal(1234, money.Amount);
            Assert.Equal("EUR", money.Currency);
        }

        [Fact]
        public void Zero_DefaultCurrency_IsXXX()
        {
            var money = Money.Zero();
            Assert.Equal(0, money.Amount);
            Assert.Equal("XXX", money.Currency);
        }

        [Fact]
        public void Zero_WithCurrency_SetsCurrency()
        {
            var money = Money.Zero("jpy");
            Assert.Equal(0, money.Amount);
            Assert.Equal("JPY", money.Currency);
        }

        [Fact]
        public void ToLong_ReturnsAmount()
        {
            var money = Money.FromLong(500, "usd");
            Assert.Equal(500, money.ToLong());
        }

        [Fact]
        public void ToDecimal_ReturnsMainUnit()
        {
            var money = Money.FromLong(1234, "usd");
            Assert.Equal(12.34m, money.ToDecimal(2));
        }

        [Fact]
        public void Addition_SameCurrency_Works()
        {
            var a = Money.FromLong(100, "usd");
            var b = Money.FromLong(50, "usd");
            var sum = a + b;
            Assert.Equal(150, sum.Amount);
            Assert.Equal("USD", sum.Currency);
        }

        [Fact]
        public void Subtraction_SameCurrency_Works()
        {
            var a = Money.FromLong(100, "usd");
            var b = Money.FromLong(40, "usd");
            var diff = a - b;
            Assert.Equal(60, diff.Amount);
            Assert.Equal("USD", diff.Currency);
        }

        [Fact]
        public void Multiplication_ByDecimal_Works()
        {
            var money = Money.FromLong(100, "usd");
            var result = money * 1.5m;
            Assert.Equal(150, result.Amount);
            Assert.Equal("USD", result.Currency);
        }

        [Fact]
        public void Multiplication_ByLong_Works()
        {
            var money = Money.FromLong(100, "usd");
            var result = money * 3;
            Assert.Equal(300, result.Amount);
            Assert.Equal("USD", result.Currency);
        }

        [Fact]
        public void Multiplication_Commutative_Works()
        {
            var money = Money.FromLong(100, "usd");
            var result1 = money * 2;
            var result2 = 2 * money;
            Assert.Equal(result1.Amount, result2.Amount);
            Assert.Equal(result1.Currency, result2.Currency);

            var result3 = money * 2.5m;
            var result4 = 2.5m * money;
            Assert.Equal(result3.Amount, result4.Amount);
            Assert.Equal(result3.Currency, result4.Currency);
        }

        [Fact]
        public void Equality_Operator_Works()
        {
            var a = Money.FromLong(100, "usd");
            var b = Money.FromLong(100, "usd");
            var c = Money.FromLong(200, "usd");
            Assert.True(a == b);
            Assert.False(a == c);
            Assert.True(a.Equals(b));
            Assert.False(a.Equals(c));
        }

        [Fact]
        public void Inequality_Operator_Works()
        {
            var a = Money.FromLong(100, "usd");
            var b = Money.FromLong(200, "usd");
            Assert.True(a != b);
        }

        [Fact]
        public void Min_ReturnsSmallerAmount()
        {
            var a = Money.FromLong(100, "usd");
            var b = Money.FromLong(200, "usd");
            var min = Money.Min(a, b);
            Assert.Equal(100, min.Amount);
        }

        [Fact]
        public void Max_ReturnsGreaterAmount()
        {
            var a = Money.FromLong(100, "usd");
            var b = Money.FromLong(200, "usd");
            var max = Money.Max(a, b);
            Assert.Equal(200, max.Amount);
        }

        [Fact]
        public void GetHashCode_ConsistentForEqualObjects()
        {
            var a = Money.FromLong(100, "usd");
            var b = Money.FromLong(100, "usd");
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void Currency_IsNormalized()
        {
            var money = Money.FromLong(100, " UsD ");
            Assert.Equal("USD", money.Currency);
        }
    }
}