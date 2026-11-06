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

        var criteriaException = Assert.IsType<CriteriaException>(exception.InnerException);
        Assert.Equal(CriteriaErrors.InvalidCondition.Code, criteriaException.Code);
    }

    [Fact]
    public void Parse_ConditionsWithoutLogicalOperator_ThrowsCriteriaException()
    {
        // Arrange
        var tokens = new List<Token>
        {
            new (TokenType.Property, "status"),
            new (TokenType.ComparisonOperator, "="),
            new (TokenType.Value, "0"),
            new (TokenType.Property, "status"),
            new (TokenType.ComparisonOperator, "!="),
            new (TokenType.Value, "0")
        };
        var parser = new Parser(tokens);

        // Act
        var exception = Assert.Throws<CriteriaException>(parser.Parse);

        // Assert
        Assert.Equal("The filter condition on 'status' must be joined to the previous one with |and| or |or|.", exception.Message);
    }

    [Fact]
    public void Parse_ConditionsWithoutLogicalOperator_CarriesTheCatalogError()
    {
        // Arrange
        var tokens = new List<Token>
        {
            new (TokenType.Property, "status"),
            new (TokenType.ComparisonOperator, "="),
            new (TokenType.Value, "0"),
            new (TokenType.Property, "name"),
            new (TokenType.ComparisonOperator, "="),
            new (TokenType.Value, "x")
        };
        var parser = new Parser(tokens);

        // Act
        var exception = Assert.Throws<CriteriaException>(parser.Parse);

        // Assert
        Assert.Equal(CriteriaErrors.MissingLogicalOperator.With("name").ToEnglish(), exception.Message);
    }

    [Fact]
    public void Parse_NoTokens_ThrowsInvalidCondition()
    {
        // Arrange
        var parser = new Parser([]);

        // Act
        var exception = Assert.Throws<CriteriaException>(parser.Parse);

        // Assert
        Assert.Equal(CriteriaErrors.InvalidCondition.Code, exception.Code);
    }

    [Fact]
    public void Parse_ConditionsJoinedByLogicalOperators_ReturnsSingleExpression()
    {
        // Arrange
        var tokens = new List<Token>
        {
            new (TokenType.Property, "status"),
            new (TokenType.ComparisonOperator, "="),
            new (TokenType.Value, "0"),
            new (TokenType.LogicalOperator, "or"),
            new (TokenType.Property, "status"),
            new (TokenType.ComparisonOperator, "="),
            new (TokenType.Value, "1")
        };
        var parser = new Parser(tokens);

        // Act
        var root = parser.Parse();

        // Assert
        Assert.Single(root.Children);
    }
}
