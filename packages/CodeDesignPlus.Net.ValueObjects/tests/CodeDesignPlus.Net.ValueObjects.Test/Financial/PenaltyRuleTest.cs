using CodeDesignPlus.Net.ValueObjects.Financial;

namespace CodeDesignPlus.Net.ValueObjects.Test.Financial;

/// <summary>
/// Covers the monthly late-payment rate of <see cref="PenaltyRule"/>.
/// </summary>
/// <remarks>
/// Late-payment interest is quoted per month, and the legal daily equivalent (around 0.06 %) does not fit in whole
/// basis points: a daily rule could only hold 0.06 % or 0.07 %. The monthly rule keeps two decimals and prorates the
/// rate over 30 days, so 30 days late on a debt charge exactly the monthly rate (pendings/192).
/// </remarks>
public class PenaltyRuleTest
{
    /// <summary>
    /// Thirty days late charge exactly the monthly rate: 1.80 % of 1,000,000 is 18,000.
    /// </summary>
    [Fact]
    public void Calculate_MonthlyRate_ThirtyDaysChargeTheMonthlyRate()
    {
        var rule = PenaltyRule.CreateMonthlyRate(180, "COP");

        Assert.Equal(1_800_000L, rule.Calculate(100_000_000L, 30));
    }

    /// <summary>
    /// The monthly rate accrues day by day: fifteen days charge half of it.
    /// </summary>
    [Fact]
    public void Calculate_MonthlyRate_AccruesPerDay()
    {
        var rule = PenaltyRule.CreateMonthlyRate(180, "COP");

        Assert.Equal(900_000L, rule.Calculate(100_000_000L, 15));
    }

    /// <summary>
    /// Grace days and the cap apply to the monthly rate as they do to the daily one.
    /// </summary>
    [Fact]
    public void Calculate_MonthlyRate_HonoursGraceDaysAndCap()
    {
        var withGrace = PenaltyRule.CreateMonthlyRate(180, "COP", graceDays: 5);
        var capped = PenaltyRule.CreateMonthlyRate(180, "COP", maxPenaltyAmount: 500_000L);

        Assert.Equal(1_500_000L, withGrace.Calculate(100_000_000L, 30));
        Assert.Equal(0L, withGrace.Calculate(100_000_000L, 5));
        Assert.Equal(500_000L, capped.Calculate(100_000_000L, 30));
    }

    /// <summary>
    /// The input a client sends with a monthly percentage becomes a monthly rule in basis points.
    /// </summary>
    [Fact]
    public void ToValueObject_MonthlyRate_KeepsTheTypeAndTwoDecimals()
    {
        var input = new PenaltyRuleInput("MONTHLY_RATE", 1.85m, 0m, "COP", 0, 0m);

        var rule = input.ToValueObject(2);

        Assert.Equal("MONTHLY_RATE", rule.Type);
        Assert.Equal(185, rule.RateBasisPoints);
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
}
