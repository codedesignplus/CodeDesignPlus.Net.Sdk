using CodeDesignPlus.Net.ValueObjects.Financial;

namespace CodeDesignPlus.Net.ValueObjects.Test.Financial;

/// <summary>
/// Covers the annual effective late-payment rate of <see cref="PenaltyRule"/>.
/// </summary>
/// <remarks>
/// Late-payment interest is certified and approved as an annual effective rate. Its daily equivalent is
/// <c>(1 + rate)^(1/365) - 1</c>, and the interest is simple: balance × days × daily rate, rounded once. The monthly rate
/// divided by 30 that replaced it could make a few days of interest exceed the usury cap (pendings/353).
/// </remarks>
public class PenaltyRuleTest
{
    /// <summary>
    /// The daily rate is the equivalent of the annual effective one, not the annual rate divided by 365.
    /// </summary>
    [Fact]
    public void GetDailyRate_AnnualEffectiveRate_IsTheEquivalentDailyRate()
    {
        var rule = PenaltyRule.CreateAnnualEffectiveRate(2500, "COP");

        Assert.Equal(0.000611539109223m, Math.Round(rule.GetDailyRate(), 15));
    }

    /// <summary>
    /// Thirty days late on 1,000,000.00 at 25 % effective per year: 100,000,000 × 30 × 0.000611539… = 1,834,617.
    /// </summary>
    [Fact]
    public void Calculate_AnnualEffectiveRate_AccruesSimpleInterestPerDay()
    {
        var rule = PenaltyRule.CreateAnnualEffectiveRate(2500, "COP");

        Assert.Equal(1_834_617L, rule.Calculate(100_000_000L, 30));
    }

    /// <summary>
    /// Simple interest does not capitalize: a full year charges less than the annual effective rate.
    /// </summary>
    [Fact]
    public void Calculate_AnnualEffectiveRate_AFullYearDoesNotCapitalize()
    {
        var rule = PenaltyRule.CreateAnnualEffectiveRate(2500, "COP");

        Assert.Equal(22_321_177L, rule.Calculate(100_000_000L, 365));
    }

    /// <summary>
    /// Grace days and the cap apply to the annual effective rate as they do to the daily one.
    /// </summary>
    [Fact]
    public void Calculate_AnnualEffectiveRate_HonoursGraceDaysAndCap()
    {
        var withGrace = PenaltyRule.CreateAnnualEffectiveRate(2500, "COP", graceDays: 5);
        var capped = PenaltyRule.CreateAnnualEffectiveRate(2500, "COP", maxPenaltyAmount: 500_000L);

        Assert.Equal(1_528_848L, withGrace.Calculate(100_000_000L, 30));
        Assert.Equal(0L, withGrace.Calculate(100_000_000L, 5));
        Assert.Equal(500_000L, capped.Calculate(100_000_000L, 30));
    }

    /// <summary>
    /// The input a client sends with an annual effective percentage becomes an annual effective rule in basis points.
    /// </summary>
    [Fact]
    public void ToValueObject_AnnualEffectiveRate_KeepsTheTypeAndTwoDecimals()
    {
        var input = new PenaltyRuleInput("ANNUAL_EFFECTIVE_RATE", 25.62m, 0m, "COP", 0, 0m);

        var rule = input.ToValueObject(2);

        Assert.Equal("ANNUAL_EFFECTIVE_RATE", rule.Type);
        Assert.Equal(2562, rule.RateBasisPoints);
    }

    /// <summary>
    /// The monthly rate is gone: a client that still sends it is rejected instead of being charged a different rate.
    /// </summary>
    [Fact]
    public void ToValueObject_MonthlyRate_IsRejected()
    {
        var input = new PenaltyRuleInput("MONTHLY_RATE", 1.8m, 0m, "COP", 0, 0m);

        Assert.ThrowsAny<Exception>(() => input.ToValueObject(2));
    }

    /// <summary>
    /// The daily rule keeps its meaning: a rate per day.
    /// </summary>
    [Fact]
    public void Calculate_DailyRate_StaysPerDay()
    {
        var rule = PenaltyRule.CreateDailyRate(6, "COP");

        Assert.Equal(1_800_000L, rule.Calculate(100_000_000L, 30));
    }

    /// <summary>
    /// A fixed amount and a percentage of the original do not accrue per day.
    /// </summary>
    [Fact]
    public void GetDailyRate_FixedAndPercentage_AreZero()
    {
        Assert.Equal(0m, PenaltyRule.CreateFixed(1_000L, "COP").GetDailyRate());
        Assert.Equal(0m, PenaltyRule.CreatePercentage(1000, "COP").GetDailyRate());
    }
}
