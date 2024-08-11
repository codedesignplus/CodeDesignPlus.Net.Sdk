using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.Exceptions.Test.Guards;

public class GuardTest
{
    public static TheoryData<int[]?> Data => new()
    {
        null,
        Array.Empty<int>()
    };


    [Fact]
    public void IsNull_ThrowsException_WhenValueIsNull()
    {
        // Arrange
        object value = null!;
        var layer = Layer.Application;
        var error = "D-001 : Throw exception when value is null.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsNull(value, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-001", exception.Code);
        Assert.Equal("Throw exception when value is null.", exception.Message);
    }

    [Fact]
    public void IsNull_NotException_WhenValueIsNotNull()
    {
        // Arrange
        object value = new();
        var layer = Layer.Application;
        var error = "D-002 : Throw exception when value is not null.";

        // Act 
        var exception = Record.Exception(() => Guard.IsNull(value, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void IsNotNull_ThrowsException_WhenValueIsNotNull()
    {
        // Arrange
        object value = new();
        var layer = Layer.Application;
        var error = "D-002 : Throw exception when value is not null.";

        // Act 
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsNotNull(value, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-002", exception.Code);
        Assert.Equal("Throw exception when value is not null.", exception.Message);
    }

    [Fact]
    public void IsNotNull_NotException_WhenValueIsNull()
    {
        // Arrange
        object value = null!;
        var layer = Layer.Application;
        var error = "D-001 : Throw exception when value is null.";

        // Act
        var exception = Record.Exception(() => Guard.IsNotNull(value, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void IsNullOrEmpty_ThrowsException_WhenValueIsNullOrEmpty()
    {
        // Arrange
        string value = string.Empty;
        var layer = Layer.Application;
        var error = "D-003 : Throw exception when value is null or empty.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsNullOrEmpty(value, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-003", exception.Code);
        Assert.Equal("Throw exception when value is null or empty.", exception.Message);
    }

    [Fact]
    public void IsNullOrEmpty_NotException_WhenValueIsNotNullOrEmpty()
    {
        // Arrange
        string value = "test";
        var layer = Layer.Application;
        var error = "D-004 : Throw exception when value is not null or empty.";

        // Act
        var exception = Record.Exception(() => Guard.IsNullOrEmpty(value, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void IsNotNullOrEmpty_ThrowsException_WhenValueIsNotNullOrEmpty()
    {
        // Arrange
        string value = "test";
        var layer = Layer.Application;
        var error = "D-004 : Throw exception when value is not null or empty.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsNotNullOrEmpty(value, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-004", exception.Code);
        Assert.Equal("Throw exception when value is not null or empty.", exception.Message);
    }

    [Fact]
    public void IsNotNullOrEmpty_NotException_WhenValueIsNullOrEmpty()
    {
        // Arrange
        string value = string.Empty;
        var layer = Layer.Application;
        var error = "D-003 : Throw exception when value is null or empty.";

        // Act
        var exception = Record.Exception(() => Guard.IsNotNullOrEmpty(value, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void IsNullOrWhiteSpace_ThrowsException_WhenValueIsNullOrWhiteSpace()
    {
        // Arrange
        string value = " ";
        var layer = Layer.Application;
        var error = "D-005 : Throw exception when value is null or white space.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsNullOrWhiteSpace(value, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-005", exception.Code);
        Assert.Equal("Throw exception when value is null or white space.", exception.Message);
    }

    [Fact]
    public void IsNullOrWhiteSpace_NotException_WhenValueIsNotNullOrWhiteSpace()
    {
        // Arrange
        string value = "test";
        var layer = Layer.Application;
        var error = "D-006 : Throw exception when value is not null or white space.";

        // Act
        var exception = Record.Exception(() => Guard.IsNullOrWhiteSpace(value, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void IsNotNullOrWhiteSpace_ThrowsException_WhenValueIsNotNullOrWhiteSpace()
    {
        // Arrange
        string value = "test";
        var layer = Layer.Application;
        var error = "D-006 : Throw exception when value is not null or white space.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsNotNullOrWhiteSpace(value, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-006", exception.Code);
        Assert.Equal("Throw exception when value is not null or white space.", exception.Message);
    }

    [Fact]
    public void IsNotNullOrWhiteSpace_NotException_WhenValueIsNullOrWhiteSpace()
    {
        // Arrange
        string value = " ";
        var layer = Layer.Application;
        var error = "D-005 : Throw exception when value is null or white space.";

        // Act
        var exception = Record.Exception(() => Guard.IsNotNullOrWhiteSpace(value, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void IsTrue_ThrowsException_WhenValueIsTrue()
    {
        // Arrange
        bool value = true;
        var layer = Layer.Application;
        var error = "D-007 : Throw exception when value is true.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsTrue(value, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-007", exception.Code);
        Assert.Equal("Throw exception when value is true.", exception.Message);
    }

    [Fact]
    public void IsTrue_NotException_WhenValueIsFalse()
    {
        // Arrange
        bool value = false;
        var layer = Layer.Application;
        var error = "D-008 : Throw exception when value is false.";

        // Act
        var exception = Record.Exception(() => Guard.IsTrue(value, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void IsFalse_ThrowsException_WhenValueIsFalse()
    {
        // Arrange
        bool value = false;
        var layer = Layer.Application;
        var error = "D-008 : Throw exception when value is false.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsFalse(value, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-008", exception.Code);
        Assert.Equal("Throw exception when value is false.", exception.Message);
    }

    [Fact]
    public void IsFalse_NotException_WhenValueIsTrue()
    {
        // Arrange
        bool value = true;
        var layer = Layer.Application;
        var error = "D-007 : Throw exception when value is true.";

        // Act
        var exception = Record.Exception(() => Guard.IsFalse(value, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void IsGreaterThan_ThrowsException_WhenValueIsGreaterThanCompare()
    {
        // Arrange
        int value = 2;
        int compare = 1;
        var layer = Layer.Application;
        var error = "D-009 : Throw exception when value is greater than compare.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsGreaterThan(value, compare, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-009", exception.Code);
        Assert.Equal("Throw exception when value is greater than compare.", exception.Message);
    }

    [Fact]
    public void IsGreaterThan_NotException_WhenValueIsLessThanCompare()
    {
        // Arrange
        int value = 1;
        int compare = 2;
        var layer = Layer.Application;
        var error = "D-009 : Throw exception when value is greater than compare.";

        // Act
        var exception = Record.Exception(() => Guard.IsGreaterThan(value, compare, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void IsGreaterThanOrEqual_ThrowsException_WhenValueIsGreaterThanOrEqualCompare()
    {
        // Arrange
        int value = 2;
        int compare = 2;
        var layer = Layer.Application;
        var error = "D-010 : Throw exception when value is greater or equal to compare";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsGreaterThanOrEqual(value, compare, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-010", exception.Code);
        Assert.Equal("Throw exception when value is greater or equal to compare", exception.Message);
    }

    [Fact]
    public void IsGreaterThanOrEqual_NotException_WhenValueIsLessThanCompare()
    {
        // Arrange
        int value = 1;
        int compare = 2;
        var layer = Layer.Application;
        var error = "D-010 : Throw exception when value is greater or equal to compare";

        // Act
        var exception = Record.Exception(() => Guard.IsGreaterThanOrEqual(value, compare, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void IsLessThan_ThrowsException_WhenValueIsLessThanCompare()
    {
        // Arrange
        int value = 1;
        int compare = 2;
        var layer = Layer.Application;
        var error = "D-011 : Throw exception when value is less than compare.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsLessThan(value, compare, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-011", exception.Code);
        Assert.Equal("Throw exception when value is less than compare.", exception.Message);
    }

    [Fact]
    public void IsLessThan_NotException_WhenValueIsGreaterThanCompare()
    {
        // Arrange
        int value = 2;
        int compare = 1;
        var layer = Layer.Application;
        var error = "D-011 : Throw exception when value is less than compare.";

        // Act
        var exception = Record.Exception(() => Guard.IsLessThan(value, compare, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void IsLessThanOrEqual_ThrowsException_WhenValueIsLessThanOrEqualCompare()
    {
        // Arrange
        int value = 2;
        int compare = 2;
        var layer = Layer.Application;
        var error = "D-012 : Throw exception when value is less or equal to compare.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsLessThanOrEqual(value, compare, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-012", exception.Code);
        Assert.Equal("Throw exception when value is less or equal to compare.", exception.Message);
    }

    [Fact]
    public void IsLessThanOrEqual_NotException_WhenValueIsGreaterThanCompare()
    {
        // Arrange
        int value = 2;
        int compare = 1;
        var layer = Layer.Application;
        var error = "D-012 : Throw exception when value is less or equal to compare.";

        // Act
        var exception = Record.Exception(() => Guard.IsLessThanOrEqual(value, compare, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void AreEquals_ThrowsException_WhenValueAreEqualsCompare()
    {
        // Arrange
        int value = 2;
        int compare = 2;
        var layer = Layer.Application;
        var error = "D-013 : Throw exception when values are equals.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.AreEquals(value, compare, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-013", exception.Code);
        Assert.Equal("Throw exception when values are equals.", exception.Message);
    }

    [Fact]
    public void AreEquals_NotException_WhenValueAreNotEqualsCompare()
    {
        // Arrange
        int value = 2;
        int compare = 1;
        var layer = Layer.Application;
        var error = "D-013 : Throw exception when values are equals.";

        // Act
        var exception = Record.Exception(() => Guard.AreEquals(value, compare, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void AreNotEquals_ThrowsException_WhenValueAreNotEqualsCompare()
    {
        // Arrange
        int value = 2;
        int compare = 1;
        var layer = Layer.Application;
        var error = "D-014 : Throw exception when values are not equals";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.AreNotEquals(value, compare, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-014", exception.Code);
        Assert.Equal("Throw exception when values are not equals", exception.Message);
    }

    [Fact]
    public void AreNotEquals_NotException_WhenValueAreEqualsCompare()
    {
        // Arrange
        int value = 2;
        int compare = 2;
        var layer = Layer.Application;
        var error = "D-014 : Throw exception when values are not equals";

        // Act
        var exception = Record.Exception(() => Guard.AreNotEquals(value, compare, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void IsInRange_ThrowsException_WhenValueIsInRange()
    {
        // Arrange
        int value = 2;
        int min = 1;
        int max = 3;
        var layer = Layer.Application;
        var error = "D-015 : Throw exception when value is in range of min and max.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsInRange(value, min, max, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-015", exception.Code);
        Assert.Equal("Throw exception when value is in range of min and max.", exception.Message);
    }

    [Fact]
    public void IsInRange_NotException_WhenValueIsNotInRange()
    {
        // Arrange
        int value = 2;
        int min = 3;
        int max = 6;
        var layer = Layer.Application;
        var error = "D-015 : Throw exception when value is in range of min and max.";

        // Act
        var exception = Record.Exception(() => Guard.IsInRange(value, min, max, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void IsNotInRange_ThrowsException_WhenValueIsNotInRange()
    {
        // Arrange
        int value = 2;
        int min = 3;
        int max = 6;
        var layer = Layer.Application;
        var error = "D-016 : Throw exception when value is out of range of min and max.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsNotInRange(value, min, max, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-016", exception.Code);
        Assert.Equal("Throw exception when value is out of range of min and max.", exception.Message);
    }

    [Fact]
    public void IsNotInRange_NotException_WhenValueIsInRange()
    {
        // Arrange
        int value = 2;
        int min = 1;
        int max = 3;
        var layer = Layer.Application;
        var error = "D-016 : Throw exception when value is out of range of min and max.";

        // Act
        var exception = Record.Exception(() => Guard.IsNotInRange(value, min, max, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void IsEmpty_ThrowsException_WhenValueIsEmpty(int[]? value)
    {
        // Arrange
        //var value = Array.Empty<int>();
        var layer = Layer.Application;
        var error = "D-017 : Throw exception when value is empty.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsEmpty(value, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-017", exception.Code);
        Assert.Equal("Throw exception when value is empty.", exception.Message);
    }

    [Fact]
    public void IsEmpty_NotException_WhenValueIsNotEmpty()
    {
        // Arrange
        var value = new List<int>() { 1, 2, 3 };
        var layer = Layer.Application;
        var error = "D-017 : Throw exception when value is empty.";

        // Act
        var exception = Record.Exception(() => Guard.IsEmpty(value, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void IsNotEmpty_ThrowsException_WhenValueIsNotEmpty()
    {
        // Arrange
        var value = new List<int>() { 1, 2, 3 };
        var layer = Layer.Application;
        var error = "D-018 : Thorw exception when value is not empty.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.IsNotEmpty(value, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-018", exception.Code);
        Assert.Equal("Thorw exception when value is not empty.", exception.Message);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void IsNotEmpty_NotException_WhenValueIsEmpty(int[]? value)
    {
        // Arrange
        var layer = Layer.Application;
        var error = "D-018 : Thorw exception when value is not empty.";

        // Act
        var exception = Record.Exception(() => Guard.IsNotEmpty(value, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void GuidIsEmpty_ThrowsException_WhenValueIsEmpty()
    {
        // Arrange
        var value = Guid.Empty;
        var layer = Layer.Application;
        var error = "D-019 : Throw exception when value is empty.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.GuidIsEmpty(value, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-019", exception.Code);
        Assert.Equal("Throw exception when value is empty.", exception.Message);
    }

    [Fact]
    public void GuidIsEmpty_NotException_WhenValueIsNotEmpty()
    {
        // Arrange
        var value = Guid.NewGuid();
        var layer = Layer.Application;
        var error = "D-019 : Throw exception when value is empty.";

        // Act
        var exception = Record.Exception(() => Guard.GuidIsEmpty(value, layer, error));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void GuidIsNotEmpty_ThrowsException_WhenValueIsNotEmpty()
    {
        // Arrange
        var value = Guid.NewGuid();
        var layer = Layer.Application;
        var error = "D-020 : Throw exception when value is not empty.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Guard.GuidIsNotEmpty(value, layer, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-020", exception.Code);
        Assert.Equal("Throw exception when value is not empty.", exception.Message);
    }

    [Fact]
    public void GuidIsNotEmpty_NotException_WhenValueIsEmpty()
    {
        // Arrange
        var value = Guid.Empty;
        var layer = Layer.Application;
        var error = "D-020 : Throw exception when value is not empty.";

        // Act
        var exception = Record.Exception(() => Guard.GuidIsNotEmpty(value, layer, error));

        // Assert
        Assert.Null(exception);
    }
}
