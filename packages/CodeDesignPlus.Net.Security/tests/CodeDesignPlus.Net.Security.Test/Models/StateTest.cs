using System;
using CodeDesignPlus.Net.Security.Abstractions.Models;
using Xunit;

namespace CodeDesignPlus.Net.Security.Test.Models;

public class StateTest
{
    [Fact]
    public void State_Id_Should_Be_Guid()
    {
        // Arrange
        var state = new State();
        var expectedId = Guid.NewGuid();

        // Act
        state.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, state.Id);
    }

    [Fact]
    public void State_Name_Should_Be_String()
    {
        // Arrange
        var state = new State();
        var expectedName = "TestState";

        // Act
        state.Name = expectedName;

        // Assert
        Assert.Equal(expectedName, state.Name);
    }

    [Fact]
    public void State_Code_Should_Be_String()
    {
        // Arrange
        var state = new State();
        var expectedCode = "TS";

        // Act
        state.Code = expectedCode;

        // Assert
        Assert.Equal(expectedCode, state.Code);
    }
}
