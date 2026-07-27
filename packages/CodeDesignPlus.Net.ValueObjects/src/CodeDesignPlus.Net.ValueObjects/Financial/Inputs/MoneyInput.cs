namespace CodeDesignPlus.Net.ValueObjects.Financial;

/// <summary>
/// WRITE contract for a monetary amount received from a client (REST body, gRPC request).
/// <para>
/// <b><see cref="Amount"/> is expressed in MAJOR units</b> (pesos, dollars) exactly as the user typed it.
/// The command handler is responsible for converting it to minor units before it reaches the aggregate:
/// resolve <c>decimalDigits</c> via <c>ICurrencyGrpc.GetCurrencyAsync(code)</c> and call
/// <see cref="ToMinorUnits"/> or <see cref="ToMoney"/>.
/// </para>
/// <para>
/// Never use this type in a query response — read DTOs expose <see cref="long"/> in minor units
/// and the client converts for display.
/// </para>
/// </summary>
/// <param name="Amount">The amount in major units (e.g., 4500000 for $4.500.000 COP).</param>
/// <param name="Currency">The 3-letter currency code, according to ISO 4217 (e.g., "COP", "USD").</param>
public sealed record MoneyInput(decimal Amount, string Currency)
{
    /// <summary>
    /// Converts this input into a <see cref="Money"/> value object in minor units.
    /// </summary>
    /// <param name="decimalPlaces">The number of decimal places for the currency, from <c>Currency.DecimalDigits</c>.</param>
    /// <returns>A <see cref="Money"/> instance holding the amount in minor units.</returns>
    public Money ToMoney(short decimalPlaces)
    {
        return Money.FromDecimal(Amount, Currency, decimalPlaces);
    }

    /// <summary>
    /// Converts this input into a raw minor unit amount, for aggregates that store a plain <see cref="long"/>.
    /// </summary>
    /// <param name="decimalPlaces">The number of decimal places for the currency, from <c>Currency.DecimalDigits</c>.</param>
    /// <returns>The amount in minor units (e.g., 450000000 for $4.500.000 COP).</returns>
    public long ToMinorUnits(short decimalPlaces)
    {
        return Money.FromDecimal(Amount, Currency, decimalPlaces).Amount;
    }
}
