using CodeDesignPlus.Net.Exceptions;
using CodeDesignPlus.Net.ValueObjects.Location;

namespace CodeDesignPlus.Net.ValueObjects.Test.Location;

public class StateTest
{
    [Fact]
    public void Create_ValidParameters_ReturnsState()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var state = State.Create(id, "Antioquia", "ANT");

        // Assert
        Assert.Equal(id, state.Id);
        Assert.Equal("Antioquia", state.Name);
        Assert.Equal("ANT", state.Code);
    }

    [Fact]
    public void Create_EmptyId_ThrowsCodeDesignPlusException()
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => State.Create(Guid.Empty, "Antioquia", "ANT"));

        Assert.Equal(Errors.StateIDCannotBeEmpty.Code, exception.Code);
        Assert.Equal(Errors.StateIDCannotBeEmpty.Fallback, exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NullOrEmptyName_ThrowsCodeDesignPlusException(string? name)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => State.Create(Guid.NewGuid(), name!, "ANT"));

        Assert.Equal(Errors.StateNameCannotBeNullOrEmpty.Code, exception.Code);
        Assert.Equal(Errors.StateNameCannotBeNullOrEmpty.Fallback, exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_NullOrEmptyCode_ThrowsCodeDesignPlusException(string? code)
    {
        // Act & Assert
        var exception = Assert.Throws<CodeDesignPlusException>(() => State.Create(Guid.NewGuid(), "Antioquia", code!));

        Assert.Equal(Errors.StateCodeCannotBeNullOrEmpty.Code, exception.Code);
        Assert.Equal(Errors.StateCodeCannotBeNullOrEmpty.Fallback, exception.Message);
        Assert.Equal(Layer.None, exception.Layer);
    }

    [Fact]
    public void Create_ValuesWithSurroundingWhitespace_AreTrimmed()
    {
        // Act
        var state = State.Create(Guid.NewGuid(), "  Antioquia  ", "  ANT  ");

        // Assert
        Assert.Equal("Antioquia", state.Name);
        Assert.Equal("ANT", state.Code);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var first = State.Create(id, "Antioquia", "ANT");
        var second = State.Create(id, "Antioquia", "ANT");

        // Assert
        Assert.True(first == second);
        Assert.False(first != second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // Act
        var first = State.Create(Guid.NewGuid(), "Antioquia", "ANT");
        var second = State.Create(Guid.NewGuid(), "Cundinamarca", "CUN");

        // Assert
        Assert.False(first == second);
        Assert.True(first != second);
    }

    [Fact]
    public void Equality_AgainstNull_IsFalse()
    {
        // Act
        var state = State.Create(Guid.NewGuid(), "Antioquia", "ANT");

        // Assert
        Assert.False(state.Equals(null));
        Assert.False(state == null);
    }
}
