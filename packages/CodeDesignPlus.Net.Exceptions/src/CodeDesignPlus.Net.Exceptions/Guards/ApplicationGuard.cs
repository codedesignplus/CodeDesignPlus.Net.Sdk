namespace CodeDesignPlus.Net.Exceptions;

public static class ApplicationGuard
{
    public static void IsNull(object value, string error) => Guard.IsNull(value, Layer.Application, error);

    public static void IsNotNull(object value, string error) => Guard.IsNotNull(value, Layer.Application, error);

    public static void IsNullOrEmpty(string value, string error) => Guard.IsNullOrEmpty(value, Layer.Application, error);

    public static void IsNotNullOrEmpty(string value, string error) => Guard.IsNotNullOrEmpty(value, Layer.Application, error);

    public static void IsNullOrWhiteSpace(string value, string error) => Guard.IsNullOrWhiteSpace(value, Layer.Application, error);

    public static void IsNotNullOrWhiteSpace(string value, string error) => Guard.IsNotNullOrWhiteSpace(value, Layer.Application, error);

    public static void IsTrue(bool value, string error) => Guard.IsTrue(value, Layer.Application, error);

    public static void IsFalse(bool value, string error) => Guard.IsFalse(value, Layer.Application, error);

    public static void IsGreaterThan<T>(T value, T compare, string error) where T : IComparable<T> => Guard.IsGreaterThan(value, compare, Layer.Application, error);

    public static void IsGreaterThanOrEqual<T>(T value, T compare, string error) where T : IComparable<T> => Guard.IsGreaterThanOrEqual(value, compare, Layer.Application, error);

    public static void IsLessThan<T>(T value, T compare, string error) where T : IComparable<T> => Guard.IsLessThan(value, compare, Layer.Application, error);

    public static void IsLessThanOrEqual<T>(T value, T compare, string error) where T : IComparable<T> => Guard.IsLessThanOrEqual(value, compare, Layer.Application, error);

    public static void AreEquals<T>(T value, T compare, string error) where T : IComparable<T> => Guard.AreEquals(value, compare, Layer.Application, error);

    public static void AreNotEquals<T>(T value, T compare, string error) where T : IComparable<T> => Guard.AreNotEquals(value, compare, Layer.Application, error);

    public static void IsInRange<T>(T value, T min, T max, string error) where T : IComparable<T> => Guard.IsInRange(value, min, max, Layer.Application, error);

    public static void IsNotInRange<T>(T value, T min, T max, string error) where T : IComparable<T> => Guard.IsNotInRange(value, min, max, Layer.Application, error);

    public static void IsEmpty<T>(IEnumerable<T> value, string error) => Guard.IsEmpty(value, Layer.Application, error);

    public static void IsNotEmpty<T>(IEnumerable<T> value, string error) => Guard.IsNotEmpty(value, Layer.Application, error);
    
    public static void GuidIsEmpty(Guid value, string error) => Guard.GuidIsEmpty(value, Layer.Application, error);

    public static void GuidIsNotEmpty(Guid value, string error) => Guard.GuidIsNotEmpty(value, Layer.Application, error);
}
