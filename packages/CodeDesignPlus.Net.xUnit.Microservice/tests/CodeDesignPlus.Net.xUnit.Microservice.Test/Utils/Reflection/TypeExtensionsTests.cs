using CodeDesignPlus.Net.xUnit.Microservice.Test.Helpers;

namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Utils.Reflection;

/// <summary>
/// Tests for the TypeExtensions class.
/// </summary>
public class TypeExtensionsTests
{
    [Fact]
    public void GetDefaultValue_ShouldReturnCorrectDefaultValues()
    {
        // Arrange & Act
        var stringDefault = typeof(string).GetDefaultValue();
        var intDefault = typeof(int).GetDefaultValue();
        var guidDefault = typeof(Guid).GetDefaultValue();
        var dateTimeDefault = typeof(DateTime).GetDefaultValue();
        var nullableIntDefault = typeof(int?).GetDefaultValue();

        // Assert
        Assert.Null(stringDefault);
        Assert.Equal(0, intDefault);
        Assert.Equal(Guid.Empty, guidDefault);
        Assert.Equal(default(DateTime), dateTimeDefault);
        Assert.Null(nullableIntDefault);
    }

    [Fact]
    public void GetParameterValues_ShouldReturnCorrectValues()
    {
        // Arrange
        var parameters = new[]
        {
                new ParameterInfoMock(typeof(string), "param1"),
                new ParameterInfoMock(typeof(int), "param2"),
                new ParameterInfoMock(typeof(Guid), "param3"),
                new ParameterInfoMock(typeof(DateTime), "param4"),
                new ParameterInfoMock(typeof(bool), "param5")
            };

        // Act
        var values = parameters.GetParameterValues();

        // Assert
        Assert.Equal("Test", values[parameters[0]]);
        Assert.Equal(1, values[parameters[1]]);
        Assert.IsType<Guid>(values[parameters[2]]);
        Assert.IsType<DateTime>(values[parameters[3]]);
        Assert.True((bool)values[parameters[4]]);
    }

    [Fact]
    public void SetValueProperties_ShouldSetCorrectValues()
    {
        // Arrange
        var testClass = new AllTypesFake();
        var type = typeof(AllTypesFake);

        // Act
        type.SetValueProperties(testClass);

        // Assert
        Assert.Equal("Test", testClass.StringProperty);
        Assert.Equal(1, testClass.IntProperty);
        Assert.Equal(1L, testClass.LongProperty);
        Assert.IsType<Guid>(testClass.GuidProperty);
        Assert.IsType<DateTime>(testClass.DateTimeProperty);
        Assert.IsType<DateTimeOffset>(testClass.DateTimeOffsetProperty);
        Assert.True(testClass.BoolProperty);
        Assert.Equal(1.0M, testClass.DecimalProperty);
        Assert.Equal(1.0F, testClass.FloatProperty);
        Assert.Equal(1.0D, testClass.DoubleProperty);
        Assert.Equal((byte)1, testClass.ByteProperty);
        Assert.Equal((short)1, testClass.ShortProperty);
        Assert.Equal([1, 2, 3], testClass.ByteArrayProperty);
        Assert.Equal([], testClass.DictionaryProperty);
        Assert.Equal('A', testClass.CharProperty);
        Assert.Equal(1U, testClass.UIntProperty);
        Assert.Equal(1UL, testClass.ULongProperty);
        Assert.Equal((ushort)1, testClass.UShortProperty);
        Assert.Equal((sbyte)1, testClass.SByteProperty);
        Assert.Equal(TimeSpan.Zero, testClass.TimeSpanProperty);
        Assert.Equal(new Uri("https://codedesignplus.com"), testClass.UriProperty);
        Assert.Equal(1, testClass.NullableIntProperty);
        Assert.Equal(1L, testClass.NullableLongProperty);
        Assert.IsType<Guid>(testClass.NullableGuidProperty);
        Assert.IsType<DateTime>(testClass.NullableDateTimeProperty);
        Assert.IsType<DateTimeOffset>(testClass.NullableDateTimeOffsetProperty);
        Assert.True(testClass.NullableBoolProperty);
        Assert.Equal(1.0M, testClass.NullableDecimalProperty);
        Assert.Equal(1.0F, testClass.NullableFloatProperty);
        Assert.Equal(1.0D, testClass.NullableDoubleProperty);
        Assert.Equal((byte)1, testClass.NullableByteProperty);
        Assert.Equal((short)1, testClass.NullableShortProperty);
        Assert.Equal(EnumFake.Value1, testClass.EnumProperty);
    }

    private class ParameterInfoMock : ParameterInfo
    {
        public ParameterInfoMock(Type parameterType, string name)
        {
            ClassImpl = parameterType;
            NameImpl = name;
        }
    }
}