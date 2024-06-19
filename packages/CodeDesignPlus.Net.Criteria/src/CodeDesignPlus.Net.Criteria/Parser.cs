using CodeDesignPlus.Net.Criteria.Models;

namespace CodeDesignPlus.Net.Criteria;


// /// <summary>
// /// Parser class that converts a list of tokens into an abstract syntax tree (AST).
// /// </summary>
// public class Parser(List<Token> tokens)
// {
//     private readonly List<Token> tokens = tokens;
//     private int position = 0;

//     /// <summary>
//     /// Parses the tokens into an AST.
//     /// </summary>
//     /// <returns>The root node of the AST.</returns>
//     public ASTNode Parse()
//     {
//         var root = new ASTNode(ASTType.Expression, null, []);

//         while (position < tokens.Count)
//         {
//             root.Children.Add(ParseExpression());
//         }

//         return root;
//     }

//     /// <summary>
//     /// Parses an expression from the tokens.
//     /// </summary>
//     /// <returns>The root node of the expression.</returns>
//     private ASTNode ParseExpression()
//     {
//         var left = ParseCondition();

//         while (position < tokens.Count && tokens[position].Type == TokenType.LogicalOperator)
//         {
//             var logicalOperator = tokens[position].Value;
//             position++;

//             var right = ParseCondition();
//             left = new ASTNode(ASTType.Operator, logicalOperator, [left, right]);
//         }

//         return left;
//     }

//     /// <summary>
//     /// Parses a condition from the tokens.
//     /// </summary>
//     /// <returns>The root node of the condition.</returns>
//     private ASTNode ParseCondition()
//     {
//         if (position < tokens.Count && tokens[position].Type == TokenType.Property)
//         {
//             var property = tokens[position].Value;
//             position++;

//             var comparisonOperator = tokens[position].Value;
//             position++;

//             var value = tokens[position].Value;
//             position++;

//             return new ASTNode(ASTType.Condition, $"{property}{comparisonOperator}{value}", []);
//         }

//         throw new Exception("Invalid condition format");
//     }
// }


public class Parser(List<Token> tokens)
{
    private readonly List<Token> tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
    private int position = 0;

    public ASTNode Parse()
    {
        var root = new ASTNode(ASTType.Expression, null, []);

        while (position < tokens.Count)
        {
            root.Children.Add(ParseExpression());
        }

        return root;
    }

    private ASTNode ParseExpression()
    {
        var left = ParseCondition();

        while (position < tokens.Count && tokens[position].Type == TokenType.LogicalOperator)
        {
            var logicalOperator = tokens[position].Value;
            position++;

            var right = ParseCondition();
            left = new ASTNode(ASTType.Operator, logicalOperator, new List<ASTNode> { left, right });
        }

        return left;
    }

    private ASTNode ParseCondition()
    {
        if (position + 2 < tokens.Count && tokens[position].Type == TokenType.Property)
        {
            var property = tokens[position++].Value;
            var comparisonOperator = tokens[position++].Value;
            var value = tokens[position++].Value;

            return new ASTNode(ASTType.Condition, $"{property}{comparisonOperator}{value}", new List<ASTNode>());
        }

        throw new InvalidOperationException("Invalid condition format");
    }
}