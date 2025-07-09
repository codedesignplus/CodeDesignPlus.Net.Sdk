namespace CodeDesignPlus.Net.Exceptions.Guards;

/// <summary>
/// Provides guard clauses for various validation checks.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Throws an exception if the specified value is null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsNull([NotNull] object value, Layer layer, string error)
    {
        if (value is null)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified value is not null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsNotNull(object value, Layer layer, string error)
    {
        if (value is not null)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified string is null or empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsNullOrEmpty(string value, Layer layer, string error)
    {
        if (string.IsNullOrEmpty(value))
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified string is not null or empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsNotNullOrEmpty(string value, Layer layer, string error)
    {
        if (!string.IsNullOrEmpty(value))
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified string is null or whitespace.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsNullOrWhiteSpace(string value, Layer layer, string error)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified string is not null or whitespace.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsNotNullOrWhiteSpace(string value, Layer layer, string error)
    {
        if (!string.IsNullOrWhiteSpace(value))
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified value is true.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsTrue(bool value, Layer layer, string error)
    {
        if (value)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified value is false.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsFalse(bool value, Layer layer, string error)
    {
        if (!value)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified value is greater than the comparison value.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="compare">The comparison value.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsGreaterThan<T>(T value, T compare, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(compare) > 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified value is greater than or equal to the comparison value.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="compare">The comparison value.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsGreaterThanOrEqual<T>(T value, T compare, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(compare) >= 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified value is less than the comparison value.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="compare">The comparison value.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsLessThan<T>(T value, T compare, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(compare) < 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified value is less than or equal to the comparison value.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="compare">The comparison value.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsLessThanOrEqual<T>(T value, T compare, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(compare) <= 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified values are equal.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="compare">The comparison value.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void AreEquals<T>(T value, T compare, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(compare) == 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified values are not equal.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="compare">The comparison value.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void AreNotEquals<T>(T value, T compare, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(compare) != 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified value is within the specified range.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum value of the range.</param>
    /// <param name="max">The maximum value of the range.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsInRange<T>(T value, T min, T max, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified value is not within the specified range.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum value of the range.</param>
    /// <param name="max">The maximum value of the range.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsNotInRange<T>(T value, T min, T max, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified value is null.
    /// </summary>
    /// <typeparam name="T">The type of the value to check.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsEmpty<T>(IEnumerable<T> value, Layer layer, string error)
    {
        if (value is null || !value.Any())
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified value is not null.
    /// </summary>
    /// <typeparam name="T">The type of the value to check.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void IsNotEmpty<T>(IEnumerable<T> value, Layer layer, string error)
    {
        if (value is not null && value.Any())
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified GUID is empty.
    /// </summary>
    /// <param name="value">The GUID to check.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void GuidIsEmpty(Guid value, Layer layer, string error)
    {
        if (value == Guid.Empty)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    /// <summary>
    /// Throws an exception if the specified GUID is not empty.
    /// </summary>
    /// <param name="value">The GUID to check.</param>
    /// <param name="layer">The layer where the exception occurred.</param>
    /// <param name="error">The error message to include in the exception.</param>
    public static void GuidIsNotEmpty(Guid value, Layer layer, string error)
    {
        if (value != Guid.Empty)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }
}
