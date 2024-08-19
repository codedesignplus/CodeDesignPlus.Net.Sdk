namespace CodeDesignPlus.Net.Exceptions.Guards;

/// <summary>
/// Provides guard clauses for application layer validations.
/// </summary>
public static class ApplicationGuard
{
    /// <summary>
    /// Throws an exception if the specified value is null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="error">The error message to include in the exception if the value is null.</param>
    public static void IsNull(object value, string error) => Guard.IsNull(value, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified value is not null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="error">The error message to include in the exception if the value is not null.</param>
    public static void IsNotNull(object value, string error) => Guard.IsNotNull(value, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified string is null or empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="error">The error message to include in the exception if the string is null or empty.</param>
    public static void IsNullOrEmpty(string value, string error) => Guard.IsNullOrEmpty(value, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified string is not null or empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="error">The error message to include in the exception if the string is not null or empty.</param>
    public static void IsNotNullOrEmpty(string value, string error) => Guard.IsNotNullOrEmpty(value, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified string is null or whitespace.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="error">The error message to include in the exception if the string is null or whitespace.</param>
    public static void IsNullOrWhiteSpace(string value, string error) => Guard.IsNullOrWhiteSpace(value, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified string is not null or whitespace.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="error">The error message to include in the exception if the string is not null or whitespace.</param>
    public static void IsNotNullOrWhiteSpace(string value, string error) => Guard.IsNotNullOrWhiteSpace(value, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified value is true.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="error">The error message to include in the exception if the value is true.</param>
    public static void IsTrue(bool value, string error) => Guard.IsTrue(value, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified value is false.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="error">The error message to include in the exception if the value is false.</param>
    public static void IsFalse(bool value, string error) => Guard.IsFalse(value, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified value is not greater than the comparison value.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="compare">The comparison value.</param>
    /// <param name="error">The error message to include in the exception if the value is not greater than the comparison value.</param>
    public static void IsGreaterThan<T>(T value, T compare, string error) where T : IComparable<T> => Guard.IsGreaterThan(value, compare, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified value is not greater than or equal to the comparison value.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="compare">The comparison value.</param>
    /// <param name="error">The error message to include in the exception if the value is not greater than or equal to the comparison value.</param>
    public static void IsGreaterThanOrEqual<T>(T value, T compare, string error) where T : IComparable<T> => Guard.IsGreaterThanOrEqual(value, compare, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified value is not less than the comparison value.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="compare">The comparison value.</param>
    /// <param name="error">The error message to include in the exception if the value is not less than the comparison value.</param>
    public static void IsLessThan<T>(T value, T compare, string error) where T : IComparable<T> => Guard.IsLessThan(value, compare, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified value is not less than or equal to the comparison value.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="compare">The comparison value.</param>
    /// <param name="error">The error message to include in the exception if the value is not less than or equal to the comparison value.</param>
    public static void IsLessThanOrEqual<T>(T value, T compare, string error) where T : IComparable<T> => Guard.IsLessThanOrEqual(value, compare, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified values are not equal.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="compare">The comparison value.</param>
    /// <param name="error">The error message to include in the exception if the values are not equal.</param>
    public static void AreEquals<T>(T value, T compare, string error) where T : IComparable<T> => Guard.AreEquals(value, compare, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified values are equal.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="compare">The comparison value.</param>
    /// <param name="error">The error message to include in the exception if the values are equal.</param>
    public static void AreNotEquals<T>(T value, T compare, string error) where T : IComparable<T> => Guard.AreNotEquals(value, compare, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified value is not within the specified range.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum value of the range.</param>
    /// <param name="max">The maximum value of the range.</param>
    /// <param name="error">The error message to include in the exception if the value is not within the range.</param>
    public static void IsInRange<T>(T value, T min, T max, string error) where T : IComparable<T> => Guard.IsInRange(value, min, max, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified value is within the specified range.
    /// </summary>
    /// <typeparam name="T">The type of the values to compare.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum value of the range.</param>
    /// <param name="max">The maximum value of the range.</param>
    /// <param name="error">The error message to include in the exception if the value is within the range.</param>
    public static void IsNotInRange<T>(T value, T min, T max, string error) where T : IComparable<T> => Guard.IsNotInRange(value, min, max, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified collection is empty.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <param name="value">The collection to check.</param>
    /// <param name="error">The error message to include in the exception if the collection is empty.</param>
    public static void IsEmpty<T>(IEnumerable<T> value, string error) => Guard.IsEmpty(value, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified collection is not empty.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <param name="value">The collection to check.</param>
    /// <param name="error">The error message to include in the exception if the collection is not empty.</param>
    public static void IsNotEmpty<T>(IEnumerable<T> value, string error) => Guard.IsNotEmpty(value, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified GUID is empty.
    /// </summary>
    /// <param name="value">The GUID to check.</param>
    /// <param name="error">The error message to include in the exception if the GUID is empty.</param>
    public static void GuidIsEmpty(Guid value, string error) => Guard.GuidIsEmpty(value, Layer.Application, error);

    /// <summary>
    /// Throws an exception if the specified GUID is not empty.
    /// </summary>
    /// <param name="value">The GUID to check.</param>
    /// <param name="error">The error message to include in the exception if the GUID is not empty.</param>
    public static void GuidIsNotEmpty(Guid value, string error) => Guard.GuidIsNotEmpty(value, Layer.Application, error);
}