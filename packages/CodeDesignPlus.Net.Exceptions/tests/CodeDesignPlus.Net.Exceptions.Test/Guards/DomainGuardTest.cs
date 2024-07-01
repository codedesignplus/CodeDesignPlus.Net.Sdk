using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.Exceptions.Test.Guards;

public class DomainGuardTest
{
    [Fact]
    public void IsNull_ThrowsException_WhenValueIsNull()
    {
        // Arrange
        object value = null!;
        var layer = Layer.Domain;
        var error = "D-001 : Throw exception when value is null.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsNull(value, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-001", exception.Code);
        Assert.Equal("Throw exception when value is null.", exception.Message);
    }

    [Fact]
    public void IsNotNull_ThrowsException_WhenValueIsNotNull()
    {
        // Arrange
        object value = new();
        var layer = Layer.Domain;
        var error = "D-002 : Throw exception when value is not null.";

        // Act 
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsNotNull(value, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-002", exception.Code);
        Assert.Equal("Throw exception when value is not null.", exception.Message);
    }

    [Fact]
    public void IsNullOrEmpty_ThrowsException_WhenValueIsNullOrEmpty()
    {
        // Arrange
        string value = string.Empty;
        var layer = Layer.Domain;
        var error = "D-003 : Throw exception when value is null or empty.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsNullOrEmpty(value, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-003", exception.Code);
        Assert.Equal("Throw exception when value is null or empty.", exception.Message);
    }

    [Fact]
    public void IsNotNullOrEmpty_ThrowsException_WhenValueIsNotNullOrEmpty()
    {
        // Arrange
        string value = "test";
        var layer = Layer.Domain;
        var error = "D-004 : Throw exception when value is not null or empty.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsNotNullOrEmpty(value, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-004", exception.Code);
        Assert.Equal("Throw exception when value is not null or empty.", exception.Message);
    }

    [Fact]
    public void IsNullOrWhiteSpace_ThrowsException_WhenValueIsNullOrWhiteSpace()
    {
        // Arrange
        string value = " ";
        var layer = Layer.Domain;
        var error = "D-005 : Throw exception when value is null or white space.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsNullOrWhiteSpace(value, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-005", exception.Code);
        Assert.Equal("Throw exception when value is null or white space.", exception.Message);
    }

    [Fact]
    public void IsNotNullOrWhiteSpace_ThrowsException_WhenValueIsNotNullOrWhiteSpace()
    {
        // Arrange
        string value = "test";
        var layer = Layer.Domain;
        var error = "D-006 : Throw exception when value is not null or white space.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsNotNullOrWhiteSpace(value, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-006", exception.Code);
        Assert.Equal("Throw exception when value is not null or white space.", exception.Message);
    }

    [Fact]
    public void IsTrue_ThrowsException_WhenValueIsTrue()
    {
        // Arrange
        bool value = true;
        var layer = Layer.Domain;
        var error = "D-007 : Throw exception when value is true.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsTrue(value, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-007", exception.Code);
        Assert.Equal("Throw exception when value is true.", exception.Message);
    }

    [Fact]
    public void IsFalse_ThrowsException_WhenValueIsFalse()
    {
        // Arrange
        bool value = false;
        var layer = Layer.Domain;
        var error = "D-008 : Throw exception when value is false.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsFalse(value, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-008", exception.Code);
        Assert.Equal("Throw exception when value is false.", exception.Message);
    }

    [Fact]
    public void IsGreaterThan_ThrowsException_WhenValueIsGreaterThanCompare()
    {
        // Arrange
        int value = 2;
        int compare = 1;
        var layer = Layer.Domain;
        var error = "D-009 : Throw exception when value is greater than compare.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsGreaterThan(value, compare, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-009", exception.Code);
        Assert.Equal("Throw exception when value is greater than compare.", exception.Message);
    }

    [Fact]
    public void IsGreaterThanOrEqual_ThrowsException_WhenValueIsGreaterThanOrEqualCompare()
    {
        // Arrange
        int value = 2;
        int compare = 2;
        var layer = Layer.Domain;
        var error = "D-010 : Throw exception when value is greater or equal to compare";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsGreaterThanOrEqual(value, compare, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-010", exception.Code);
        Assert.Equal("Throw exception when value is greater or equal to compare", exception.Message);
    }

    [Fact]
    public void IsLessThan_ThrowsException_WhenValueIsLessThanCompare()
    {
        // Arrange
        int value = 1;
        int compare = 2;
        var layer = Layer.Domain;
        var error = "D-011 : Throw exception when value is less than compare.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsLessThan(value, compare, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-011", exception.Code);
        Assert.Equal("Throw exception when value is less than compare.", exception.Message);
    }

    [Fact]
    public void IsLessThanOrEqual_ThrowsException_WhenValueIsLessThanOrEqualCompare()
    {
        // Arrange
        int value = 2;
        int compare = 2;
        var layer = Layer.Domain;
        var error = "D-012 : Throw exception when value is less or equal to compare.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsLessThanOrEqual(value, compare, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-012", exception.Code);
        Assert.Equal("Throw exception when value is less or equal to compare.", exception.Message);
    }

    [Fact]
    public void AreEquals_ThrowsException_WhenValueAreEqualsCompare()
    {
        // Arrange
        int value = 2;
        int compare = 2;
        var layer = Layer.Domain;
        var error = "D-013 : Throw exception when values are equals.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.AreEquals(value, compare, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-013", exception.Code);
        Assert.Equal("Throw exception when values are equals.", exception.Message);
    }

    [Fact]
    public void AreNotEquals_ThrowsException_WhenValueAreNotEqualsCompare()
    {
        // Arrange
        int value = 2;
        int compare = 1;
        var layer = Layer.Domain;
        var error = "D-014 : Throw exception when values are not equals";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.AreNotEquals(value, compare, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-014", exception.Code);
        Assert.Equal("Throw exception when values are not equals", exception.Message);
    }

    [Fact]
    public void IsInRange_ThrowsException_WhenValueIsInRange()
    {
        // Arrange
        int value = 2;
        int min = 1;
        int max = 3;
        var layer = Layer.Domain;
        var error = "D-015 : Throw exception when value is in range of min and max.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsInRange(value, min, max, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-015", exception.Code);
        Assert.Equal("Throw exception when value is in range of min and max.", exception.Message);
    }

    [Fact]
    public void IsNotInRange_ThrowsException_WhenValueIsNotInRange()
    {
        // Arrange
        int value = 2;
        int min = 3;
        int max = 6;
        var layer = Layer.Domain;
        var error = "D-016 : Throw exception when value is out of range of min and max.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsNotInRange(value, min, max, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-016", exception.Code);
        Assert.Equal("Throw exception when value is out of range of min and max.", exception.Message);
    }

    [Fact]
    public void IsEmpty_ThrowsException_WhenValueIsEmpty()
    {
        // Arrange
        var value = Array.Empty<int>();
        var layer = Layer.Domain;
        var error = "D-017 : Throw exception when value is empty.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsEmpty(value, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-017", exception.Code);
        Assert.Equal("Throw exception when value is empty.", exception.Message);
    }

    [Fact]
    public void IsNotEmpty_ThrowsException_WhenValueIsNotEmpty()
    {
        // Arrange
        var value = new List<int>() { 1, 2, 3 };
        var layer = Layer.Domain;
        var error = "D-018 : Thorw exception when value is not empty.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.IsNotEmpty(value, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-018", exception.Code);
        Assert.Equal("Thorw exception when value is not empty.", exception.Message);
    }

    [Fact]
    public void GuidIsEmpty_ThrowsException_WhenValueIsEmpty()
    {
        // Arrange
        var value = Guid.Empty;
        var layer = Layer.Domain;
        var error = "D-019 : Throw exception when value is empty.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.GuidIsEmpty(value, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-019", exception.Code);
        Assert.Equal("Throw exception when value is empty.", exception.Message);
    }

    [Fact]
    public void GuidIsNotEmpty_ThrowsException_WhenValueIsNotEmpty()
    {
        // Arrange
        var value = Guid.NewGuid();
        var layer = Layer.Domain;
        var error = "D-020 : Throw exception when value is not empty.";

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => DomainGuard.GuidIsNotEmpty(value, error));

        // Assert
        Assert.Equal(layer, exception.Layer);
        Assert.Equal("D-020", exception.Code);
        Assert.Equal("Throw exception when value is not empty.", exception.Message);
    }
}
