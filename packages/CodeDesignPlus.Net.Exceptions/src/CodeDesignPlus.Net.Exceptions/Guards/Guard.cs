namespace CodeDesignPlus.Net.Exceptions;

public static class Guard
{
    public static void IsNull(object value, Layer layer, string error)
    {
        if (value is null)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void IsNotNull(object value, Layer layer, string error)
    {
        if (value is not null)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void IsNullOrEmpty(string value, Layer layer, string error)
    {
        if (string.IsNullOrEmpty(value))
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void IsNotNullOrEmpty(string value, Layer layer, string error)
    {
        if (!string.IsNullOrEmpty(value))
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void IsNullOrWhiteSpace(string value, Layer layer, string error)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void IsNotNullOrWhiteSpace(string value, Layer layer, string error)
    {
        if (!string.IsNullOrWhiteSpace(value))
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void IsTrue(bool value, Layer layer, string error)
    {
        if (value)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void IsFalse(bool value, Layer layer, string error)
    {
        if (!value)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }
    public static void IsGreaterThan<T>(T value, T compare, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(compare) > 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void IsGreaterThanOrEqual<T>(T value, T compare, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(compare) >= 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void IsLessThan<T>(T value, T compare, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(compare) < 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void IsLessThanOrEqual<T>(T value, T compare, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(compare) <= 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void AreEquals<T>(T value, T compare, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(compare) == 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void AreNotEquals<T>(T value, T compare, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(compare) != 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void IsInRange<T>(T value, T min, T max, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void IsNotInRange<T>(T value, T min, T max, Layer layer, string error) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void IsEmpty<T>(IEnumerable<T> value, Layer layer, string error)
    {
        if (value is null || !value.Any())
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void IsNotEmpty<T>(IEnumerable<T> value, Layer layer, string error)
    {
        if (value is not null && value.Any())
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void GuidIsEmpty(Guid value, Layer layer, string error)
    {
        if (value == Guid.Empty)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }

    public static void GuidIsNotEmpty(Guid value, Layer layer, string error)
    {
        if (value != Guid.Empty)
            throw new CodeDesignPlusException(layer, error.GetCode(), error.GetMessage());
    }
}
