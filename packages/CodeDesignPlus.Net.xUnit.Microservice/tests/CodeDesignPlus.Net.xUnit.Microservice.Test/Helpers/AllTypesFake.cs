namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Helpers;

/// <summary>
/// A class containing properties of all supported types for testing TypeExtensions.
/// </summary>
public class AllTypesFake
{
    public string? StringProperty { get; set; }
    public int IntProperty { get; set; }
    public long LongProperty { get; set; }
    public Guid GuidProperty { get; set; }
    public DateTime DateTimeProperty { get; set; }
    public DateTimeOffset DateTimeOffsetProperty { get; set; }
    public bool BoolProperty { get; set; }
    public decimal DecimalProperty { get; set; }
    public float FloatProperty { get; set; }
    public double DoubleProperty { get; set; }
    public byte ByteProperty { get; set; }
    public short ShortProperty { get; set; }
    public byte[]? ByteArrayProperty { get; set; }
    public Dictionary<string, object>? DictionaryProperty { get; set; }
    public char CharProperty { get; set; }
    public uint UIntProperty { get; set; }
    public ulong ULongProperty { get; set; }
    public ushort UShortProperty { get; set; }
    public sbyte SByteProperty { get; set; }
    public TimeSpan TimeSpanProperty { get; set; }
    public Uri? UriProperty { get; set; }
    public int? NullableIntProperty { get; set; }
    public long? NullableLongProperty { get; set; }
    public Guid? NullableGuidProperty { get; set; }
    public DateTime? NullableDateTimeProperty { get; set; }
    public DateTimeOffset? NullableDateTimeOffsetProperty { get; set; }
    public bool? NullableBoolProperty { get; set; }
    public decimal? NullableDecimalProperty { get; set; }
    public float? NullableFloatProperty { get; set; }
    public double? NullableDoubleProperty { get; set; }
    public byte? NullableByteProperty { get; set; }
    public short? NullableShortProperty { get; set; }
    public EnumFake EnumProperty { get; set; }
}

/// <summary>
/// An enumeration for testing TypeExtensions.
/// </summary>
public enum EnumFake
{
    Value1,
    Value2,
    Value3
}