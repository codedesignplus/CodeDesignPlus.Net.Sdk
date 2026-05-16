using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Common;

/// <summary>
/// Represents an immutable date-time range with a start and end point in UTC.
/// Useful for booking windows, lease terms, availability slots, and reporting periods.
/// Both values are stored as UTC to support multi-timezone deployments.
/// </summary>
public sealed class DateRange : IEquatable<DateRange>
{
    /// <summary>
    /// Gets the start of the range (UTC).
    /// </summary>
    public DateTimeOffset Start { get; private set; }

    /// <summary>
    /// Gets the end of the range (UTC).
    /// </summary>
    public DateTimeOffset End { get; private set; }

    /// <summary>
    /// Gets the duration of the range.
    /// </summary>
    public TimeSpan Duration => End - Start;

    [JsonConstructor]
    private DateRange(DateTimeOffset start, DateTimeOffset end)
    {
        Guard.IsTrue(start == default, Exceptions.Layer.None, "000 : Start cannot be the default value.");
        Guard.IsTrue(end == default, Exceptions.Layer.None, "001 : End cannot be the default value.");
        Guard.IsTrue(end <= start, Exceptions.Layer.None, "002 : End must be after Start.");

        Start = start.ToUniversalTime();
        End = end.ToUniversalTime();
    }

    /// <summary>
    /// Creates a new <see cref="DateRange"/> from start and end UTC values.
    /// </summary>
    /// <param name="start">The start of the range (UTC).</param>
    /// <param name="end">The end of the range (UTC).</param>
    /// <returns>A new <see cref="DateRange"/> instance.</returns>
    public static DateRange Create(DateTimeOffset start, DateTimeOffset end)
        => new(start, end);

    /// <summary>
    /// Creates a <see cref="DateRange"/> starting now (UTC) with a given duration.
    /// </summary>
    /// <param name="duration">The duration of the range.</param>
    /// <returns>A new <see cref="DateRange"/> instance.</returns>
    public static DateRange FromNow(TimeSpan duration)
    {
        var now = DateTimeOffset.UtcNow;
        return new DateRange(now, now.Add(duration));
    }

    /// <summary>
    /// Returns true if the given point falls within this range (inclusive Start, exclusive End).
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns>True if the point is within the range; otherwise, false.</returns>
    public bool Contains(DateTimeOffset point) => point >= Start && point < End;

    /// <summary>
    /// Returns true if this range overlaps with another. Touching boundaries are NOT considered overlapping.
    /// </summary>
    /// <param name="other">The other range to check against.</param>
    /// <returns>True if the ranges overlap; otherwise, false.</returns>
    public bool Overlaps(DateRange other)
    {
        Guard.IsNull(other, Exceptions.Layer.None, "003 : Other range cannot be null.");
        return Start < other.End && End > other.Start;
    }

    /// <summary>
    /// Returns true if this range fully contains the other range.
    /// </summary>
    /// <param name="other">The other range to check against.</param>
    /// <returns>True if this range contains the other; otherwise, false.</returns>
    public bool Contains(DateRange other)
    {
        Guard.IsNull(other, Exceptions.Layer.None, "004 : Other range cannot be null.");
        return Start <= other.Start && End >= other.End;
    }

    /// <summary>
    /// Returns true if two <see cref="DateRange"/> instances are equal.
    /// </summary>
    /// <param name="a">The first range.</param>
    /// <param name="b">The second range.</param>
    /// <returns>True if the ranges are equal; otherwise, false.</returns>
    public static bool operator ==(DateRange? a, DateRange? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    /// <summary>
    /// Returns true if two <see cref="DateRange"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first range.</param>
    /// <param name="b">The second range.</param>
    /// <returns>True if the ranges are not equal; otherwise, false.</returns>
    public static bool operator !=(DateRange? a, DateRange? b) => !(a == b);

    /// <summary>
    /// Returns true if this instance is equal to another <see cref="DateRange"/>.
    /// </summary>
    /// <param name="other">The other range to compare to.</param>
    /// <returns>True if the ranges are equal; otherwise, false.</returns>
    public bool Equals(DateRange? other)
    {
        if (other is null) return false;
        return Start == other.Start && End == other.End;
    }

    /// <summary>
    /// Returns true if this instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare to.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is DateRange other && Equals(other);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => HashCode.Combine(Start, End);
}
