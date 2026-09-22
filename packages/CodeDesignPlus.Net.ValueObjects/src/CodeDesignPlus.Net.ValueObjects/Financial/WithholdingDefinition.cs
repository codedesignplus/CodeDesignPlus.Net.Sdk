using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Financial;

/// <summary>
/// Says <b>which</b> withholding a charge carries. It deliberately cannot say how much.
/// </summary>
/// <remarks>
/// <para>
/// This type used to carry the rate, the minimum base and the currency, and it could compute the amount on
/// its own. That made every document a second source of truth: a contract signed in March and one signed in
/// April could disagree about the same concept, and nothing compared them. Withholding rates are not a term
/// the parties negotiate — the law sets them — so a copy frozen at signing time is not a record of what was
/// agreed, it is a stale answer waiting to be used.
/// </para>
/// <para>
/// The minimum base was worse, because it was frozen <b>in money</b>. Colombian thresholds are expressed in
/// UVT and the UVT changes every December, so a minimum stored in pesos silently shrinks every January. It
/// never fails; it just withholds on amounts it should have skipped.
/// </para>
/// <para>
/// So the rate and the threshold now live in one place, the withholding catalogue, and are resolved by the
/// service that issues the document, on the document's own date. That is already how the paying side works.
/// <b>Losing the ability to compute is the point</b>: while this object can answer on its own, somebody will
/// ask it, and the two answers come back.
/// </para>
/// </remarks>
public sealed class WithholdingDefinition : IEquatable<WithholdingDefinition>
{
    /// <summary>
    /// The catalogue code. This is the whole operative content of the type.
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// The name shown when the withholding was configured. <b>Display only.</b>
    /// </summary>
    /// <remarks>
    /// A signed contract has to stay readable years later, including after the rule it points at has been
    /// retired from the catalogue. That is all this is for.
    /// </remarks>
    [JsonProperty]
    public string? AgreedNameForDisplay { get; private set; }

    /// <summary>
    /// The rate shown when the withholding was configured, in basis points. <b>Display only — never the one
    /// that is applied.</b>
    /// </summary>
    /// <remarks>
    /// The name is long on purpose. Anything shorter reads like the rate to use, and the whole reason this
    /// type shrank is that somebody would use it.
    /// </remarks>
    [JsonProperty]
    public int? AgreedRateBasisPointsForDisplay { get; private set; }

    /// <summary>
    /// Rebuilds the value object when reading it back.
    /// </summary>
    /// <param name="code">The catalogue code.</param>
    /// <remarks>
    /// Only <paramref name="code"/> is a parameter. The display fields come back through their members, so
    /// adding another one later does not stop stored documents from being read.
    /// </remarks>
    [JsonConstructor]
    private WithholdingDefinition(string code)
    {
        var normalizedCode = code?.Trim().ToUpperInvariant() ?? string.Empty;

        Guard.IsNullOrEmpty(normalizedCode, Exceptions.Layer.None, "000 : Code cannot be null or empty.");

        Code = normalizedCode;
    }

    /// <summary>
    /// Creates a withholding definition.
    /// </summary>
    /// <param name="code">The catalogue code. The only thing that decides anything.</param>
    /// <param name="agreedNameForDisplay">The name to show. Optional, display only.</param>
    /// <param name="agreedRateBasisPointsForDisplay">The rate to show, in basis points. Optional, display only.</param>
    public static WithholdingDefinition Create(string code, string? agreedNameForDisplay = null, int? agreedRateBasisPointsForDisplay = null)
    {
        if (agreedRateBasisPointsForDisplay.HasValue)
            Guard.IsNotInRange(agreedRateBasisPointsForDisplay.Value, 0, 100000, Exceptions.Layer.None, "002 : RateBasisPoints must be between 0 and 100000.");

        return new WithholdingDefinition(code)
        {
            AgreedNameForDisplay = agreedNameForDisplay,
            AgreedRateBasisPointsForDisplay = agreedRateBasisPointsForDisplay
        };
    }

    /// <summary>
    /// Compares two definitions.
    /// </summary>
    /// <param name="a">The first definition.</param>
    /// <param name="b">The second definition.</param>
    public static bool operator ==(WithholdingDefinition? a, WithholdingDefinition? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    /// <summary>
    /// Compares two definitions.
    /// </summary>
    /// <param name="a">The first definition.</param>
    /// <param name="b">The second definition.</param>
    public static bool operator !=(WithholdingDefinition? a, WithholdingDefinition? b) => !(a == b);

    /// <summary>
    /// Two definitions are the same when they point at the same catalogue rule.
    /// </summary>
    /// <param name="other">The definition to compare against.</param>
    /// <remarks>
    /// The display fields are left out on purpose. They are a snapshot of what was shown, so two charges
    /// carrying the same rule captured months apart are still the same withholding — and comparing them would
    /// report a change every time the catalogue is edited.
    /// </remarks>
    public bool Equals(WithholdingDefinition? other)
    {
        if (other is null) return false;

        return Code == other.Code;
    }

    /// <summary>
    /// Compares against any object.
    /// </summary>
    /// <param name="obj">The object to compare against.</param>
    public override bool Equals(object? obj) => obj is WithholdingDefinition other && Equals(other);

    /// <summary>
    /// Hashes the code, which is what identity is built on.
    /// </summary>
    public override int GetHashCode() => Code.GetHashCode();
}
