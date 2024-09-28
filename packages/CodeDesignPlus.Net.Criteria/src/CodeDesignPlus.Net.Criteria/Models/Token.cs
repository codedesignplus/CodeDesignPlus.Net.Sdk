namespace CodeDesignPlus.Net.Criteria.Models;

/// <summary>
/// Represents the type of token.
/// </summary>
public enum TokenType
{
    /// <summary>
    /// Represents a property token.
    /// </summary>
    Property,
    /// <summary>
    /// Represents an operator token.
    /// </summary>
    Operator,
    /// <summary>
    /// Represents a value token.
    /// </summary>
    Value,
    /// <summary>
    /// Represents a logical operator token.
    /// </summary>
    LogicalOperator,
    /// <summary>
    /// Represents a comparison operator token.
    /// </summary>
    ComparisonOperator
}

/// <summary>
/// Representing a token with a type and a value.
/// </summary>
/// <param name="Type">The type of the token.</param>
/// <param name="Value">The value of the token.</param>
public record Token(TokenType Type, string Value);