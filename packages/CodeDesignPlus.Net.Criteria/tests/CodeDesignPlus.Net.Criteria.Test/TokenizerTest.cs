using CodeDesignPlus.Net.Criteria.Exceptions;
using CodeDesignPlus.Net.Criteria.Models;
using System.Reflection;

namespace CodeDesignPlus.Net.Criteria.Test;

public class TokenizerTest
{
    [Fact]
    public void CreateTokensFromPart_ValidComparisonOperator_TokensGenerated()
    {
        // Arrange
        string part = "Total>=30";
        var methodInfo = typeof(Tokenizer).GetMethod("CreateTokensFromPart", BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var result = (IEnumerable<Token>)methodInfo!.Invoke(null, [part])!;

        // Assert
        var tokens = result.ToList();
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Property, tokens[0].Type);
        Assert.Equal("Total", tokens[0].Value);
        Assert.Equal(TokenType.ComparisonOperator, tokens[1].Type);
        Assert.Equal(">=", tokens[1].Value);
        Assert.Equal(TokenType.Value, tokens[2].Type);
        Assert.Equal("30", tokens[2].Value);
    }

    // A part without a comparison operator used to vanish without any error (Kappali pendings/403).
    [Fact]
    public void CreateTokensFromPart_NoComparisonOperator_ThrowsInvalidCondition()
    {
        // Arrange
        string part = "invalidPart";
        var methodInfo = typeof(Tokenizer).GetMethod("CreateTokensFromPart", BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var exception = Assert.Throws<TargetInvocationException>(() => methodInfo!.Invoke(null, [part]));

        // Assert
        Assert.Equal(CriteriaErrors.InvalidCondition.Code, Assert.IsType<CriteriaException>(exception.InnerException).Code);
    }

    [Theory]
    [InlineData("Total>=30|foo")]
    [InlineData("Total>=30|and|foo")]
    [InlineData("Total>=30|")]
    public void Tokenize_PartWithoutComparisonOperator_ThrowsInvalidCondition(string input)
    {
        // Act
        var exception = Assert.Throws<CriteriaException>(() => Tokenizer.Tokenize(input));

        // Assert
        Assert.Equal(CriteriaErrors.InvalidCondition.Code, exception.Code);
    }
}
