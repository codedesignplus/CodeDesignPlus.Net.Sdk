using CodeDesignPlus.Net.Criteria.Exceptions;
using CodeDesignPlus.Net.Criteria.Models;
using System.Reflection;

namespace CodeDesignPlus.Net.Criteria.Test;

public class ParserTest
{

    [Fact]
    public void ParseCondition_ValidTokens_ReturnsASTNode()
    {
        // Arrange
        var tokens = new List<Token>
        {
            new ( TokenType.Property, "age"),
            new ( TokenType.ComparisonOperator,">" ),
            new ( TokenType.Value,"30" )
        };

        var parser = new Parser(tokens);

        // Use reflection to access the private method
        var methodInfo = typeof(Parser).GetMethod("ParseCondition", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = (AstNode)methodInfo!.Invoke(parser, null)!;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(AstType.Condition, result.Type);
        Assert.Equal("age>30", result.Value);
        Assert.Empty(result.Children);
    }

    [Fact]
    public void ParseCondition_InvalidTokens_ThrowsCriteriaException()
    {
        // Arrange
        var tokens = new List<Token>
        {
            new (TokenType.Property,  "age" ),
            new (TokenType.Value,  "30" )
        };
        var parser = new Parser(tokens);

        // Use reflection to access the private method
        var methodInfo = typeof(Parser).GetMethod("ParseCondition", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act & Assert
        var exception = Assert.Throws<TargetInvocationException>(() => methodInfo!.Invoke(parser, null));

        Assert.IsType<CriteriaException>(exception.InnerException);
        Assert.Equal("Invalid condition format", exception.InnerException.Message);
    }
}
