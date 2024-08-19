namespace CodeDesignPlus.Net.Criteria.Models;

/// <summary>
/// Abstract syntax tree (AST) type.
/// </summary>
public enum AstType
{
    /// <summary>
    /// Represents an expression.
    /// </summary>
    Expression,
    /// <summary>
    /// Represents an operator.
    /// </summary>
    Operator,
    /// <summary>
    /// Represents a condition.
    /// </summary>
    Condition
}

/// <summary>
/// Representing a node in the abstract syntax tree.
/// </summary>
/// <param name="Type">The type of the node.</param>
/// <param name="Value">The value of the node.</param>
/// <param name="Children">The children of the node.</param>
public record AstNode(AstType Type, string Value, List<AstNode> Children);