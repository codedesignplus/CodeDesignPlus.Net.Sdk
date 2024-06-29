namespace CodeDesignPlus.Net.Criteria.Models;

/// <summary>
/// Abstract syntax tree (AST) type.
/// </summary>
public enum ASTType
{
    Expression,
    Operator,
    Condition
}

/// <summary>
/// Representing a node in the abstract syntax tree.
/// </summary>
/// <param name="Type">The type of the node.</param>
/// <param name="Value">The value of the node.</param>
/// <param name="Children">The children of the node.</param>
public record ASTNode(ASTType Type, string Value, List<ASTNode> Children);