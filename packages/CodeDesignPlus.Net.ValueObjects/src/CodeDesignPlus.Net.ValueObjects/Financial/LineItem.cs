using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Financial;

/// <summary>
/// Represents a line item in a financial document (invoice, receipt, etc.).
/// All monetary amounts are stored in minor units (e.g., cents) to avoid floating-point precision loss.
/// <see cref="Subtotal"/>, <see cref="TaxAmount"/>, and <see cref="Total"/> are computed from the other fields.
/// </summary>
public sealed class LineItem : IEquatable<LineItem>
{
    /// <summary>
    /// Gets the product or service identifier.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Gets the description shown on the document (e.g., "Arriendo Octubre 2025").
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets the number of units.
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Gets the unit price in minor units (e.g., 15000000 = $150,000 COP).
    /// </summary>
    public long UnitPrice { get; private set; }

    /// <summary>
    /// Gets the ISO 4217 currency code (e.g., "COP", "USD").
    /// </summary>
    public string Currency { get; private set; }

    /// <summary>
    /// Gets the list of taxes applicable to this line item.
    /// </summary>
    public IReadOnlyList<TaxDefinition> Taxes { get; private set; }

    /// <summary>
    /// Gets the subtotal before taxes (<see cref="Quantity"/> × <see cref="UnitPrice"/>) in minor units.
    /// </summary>
    public long Subtotal => Quantity * UnitPrice;

    /// <summary>
    /// Gets the total tax amount for this line item in minor units.
    /// Only exclusive taxes (IsInclusive = false) are summed.
    /// </summary>
    public long TaxAmount
    {
        get
        {
            if (Taxes is null || Taxes.Count == 0) return 0L;

            var exclusiveTaxBase = Taxes.Any(t => !t.IsInclusive) ? Subtotal : 0L;

            return Taxes
                .Where(t => !t.IsInclusive)
                .Sum(t => t.CalculateTaxAmount(exclusiveTaxBase));
        }
    }

    /// <summary>
    /// Gets the total amount including taxes (<see cref="Subtotal"/> + <see cref="TaxAmount"/>) in minor units.
    /// </summary>
    public long Total => Subtotal + TaxAmount;

    [JsonConstructor]
    private LineItem(Guid productId, string description, int quantity, long unitPrice, string currency, IReadOnlyList<TaxDefinition> taxes)
    {
        var normalizedCurrency = currency?.Trim().ToUpperInvariant() ?? string.Empty;

        Guard.GuidIsEmpty(productId, Exceptions.Layer.None, "000 : ProductId cannot be empty.");
        Guard.IsNullOrEmpty(description, Exceptions.Layer.None, "001 : Description cannot be null or empty.");
        Guard.IsLessThan(quantity, 1, Exceptions.Layer.None, "002 : Quantity must be at least 1.");
        Guard.IsLessThan(unitPrice, 0L, Exceptions.Layer.None, "003 : UnitPrice cannot be negative.");
        Guard.IsNullOrEmpty(normalizedCurrency, Exceptions.Layer.None, "004 : Currency cannot be null or empty.");
        Guard.IsFalse(normalizedCurrency.Length == 3, Exceptions.Layer.None, "005 : Currency must be exactly 3 characters (ISO 4217).");

        ProductId = productId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Currency = normalizedCurrency;
        Taxes = taxes ?? [];
    }

    /// <summary>
    /// Creates a line item with a list of applicable taxes.
    /// </summary>
    /// <param name="productId">The product or service identifier.</param>
    /// <param name="description">The description shown on the document.</param>
    /// <param name="quantity">The number of units. Must be at least 1.</param>
    /// <param name="unitPrice">The unit price in minor units.</param>
    /// <param name="currency">The ISO 4217 currency code.</param>
    /// <param name="taxes">The list of taxes applicable to this line item.</param>
    /// <returns>A new <see cref="LineItem"/> instance.</returns>
    public static LineItem Create(Guid productId, string description, int quantity, long unitPrice, string currency, IReadOnlyList<TaxDefinition> taxes)
        => new(productId, description, quantity, unitPrice, currency, taxes);

    /// <summary>
    /// Creates a line item without taxes.
    /// </summary>
    /// <param name="productId">The product or service identifier.</param>
    /// <param name="description">The description shown on the document.</param>
    /// <param name="quantity">The number of units. Must be at least 1.</param>
    /// <param name="unitPrice">The unit price in minor units.</param>
    /// <param name="currency">The ISO 4217 currency code.</param>
    /// <returns>A new <see cref="LineItem"/> instance with an empty tax list.</returns>
    public static LineItem Create(Guid productId, string description, int quantity, long unitPrice, string currency)
        => new(productId, description, quantity, unitPrice, currency, []);

    /// <summary>
    /// Returns true if two <see cref="LineItem"/> instances are equal.
    /// </summary>
    /// <param name="a">The first instance.</param>
    /// <param name="b">The second instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(LineItem? a, LineItem? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    /// <summary>
    /// Returns true if two <see cref="LineItem"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first instance.</param>
    /// <param name="b">The second instance.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(LineItem? a, LineItem? b) => !(a == b);

    /// <summary>
    /// Returns true if this instance is equal to another <see cref="LineItem"/>.
    /// </summary>
    /// <param name="other">The other instance to compare to.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public bool Equals(LineItem? other)
    {
        if (other is null) return false;
        return ProductId == other.ProductId &&
               Description == other.Description &&
               Quantity == other.Quantity &&
               UnitPrice == other.UnitPrice &&
               Currency == other.Currency &&
               Taxes.SequenceEqual(other.Taxes);
    }

    /// <summary>
    /// Returns true if this instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare to.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is LineItem other && Equals(other);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(ProductId);
        hash.Add(Description);
        hash.Add(Quantity);
        hash.Add(UnitPrice);
        hash.Add(Currency);
        foreach (var tax in Taxes) hash.Add(tax);
        return hash.ToHashCode();
    }
}
