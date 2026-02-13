namespace CodeDesignPlus.Net.Security.Abstractions.Models;

/// <summary>
/// Represents the currency information.
/// </summary>
public class Currency
{
    /// <summary>
    /// Gets or sets the identifier of the currency.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the name of the currency.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the code of the currency. (ISO 3166-1 numeric)
    /// </summary>
    public ushort Code { get; set; }
    /// <summary>
    /// Gets or sets the symbol of the currency.
    /// </summary>
    public string Symbol { get; set; }
}
