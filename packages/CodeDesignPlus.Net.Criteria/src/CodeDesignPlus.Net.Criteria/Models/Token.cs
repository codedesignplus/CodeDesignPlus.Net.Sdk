namespace CodeDesignPlus.Net.Criteria.Models;

/// <summary>
/// Tokenizes the input string into a list of tokens.
/// </summary>
/// <param name="input">The input filter string.</param>
/// <returns>A list of tokens.</returns>
public enum TokenType
{
    Property,
    Operator,
    Value,
    LogicalOperator,
    ComparisonOperator
}

/// <summary>
/// Representing a token with a type and a value.
/// </summary>
/// <param name="Type">The type of the token.</param>
/// <param name="Value">The value of the token.</param>
public record Token(TokenType Type, string Value);