using System.Reflection;
using CodeDesignPlus.Net.Criteria.Models;

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

    [Fact]
    public void CreateTokensFromPart_NoComparisonOperator_NoTokensGenerated()
    {
        // Arrange
        string part = "invalidPart";
        var methodInfo = typeof(Tokenizer).GetMethod("CreateTokensFromPart", BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var result = (IEnumerable<Token>)methodInfo!.Invoke(null, [part])!;

        // Assert
        Assert.Empty(result);
    }
}
