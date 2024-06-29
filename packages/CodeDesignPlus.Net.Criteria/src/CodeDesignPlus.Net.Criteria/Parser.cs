namespace CodeDesignPlus.Net.Criteria;

/// <summary>
/// Represents a parser for converting a list of tokens into an Abstract Syntax Tree (AST).
/// </summary>
internal class Parser
{
    private readonly List<Token> tokens;

    /// <summary>
    /// Initializes a new instance of the <see cref="Parser"/> class.
    /// </summary>
    /// <param name="tokens">The list of tokens to parse.</param>
    public Parser(List<Token> tokens)
    {
        ArgumentNullException.ThrowIfNull(tokens);

        this.tokens = tokens;
        position = 0;
    }

    /// <summary>
    /// Parses the list of tokens and returns the root node of the Abstract Syntax Tree (AST).
    /// </summary>
    /// <returns>The root node of the AST.</returns>
    public ASTNode Parse()
    {
        var root = new ASTNode(ASTType.Expression, null, []);

        while (position < tokens.Count)
        {
            root.Children.Add(ParseExpression());
        }

        return root;
    }

    private int position;

    private ASTNode ParseExpression()
    {
        var left = ParseCondition();

        while (position < tokens.Count && tokens[position].Type == TokenType.LogicalOperator)
        {
            var logicalOperator = tokens[position].Value;
            position++;

            var right = ParseCondition();
            left = new ASTNode(ASTType.Operator, logicalOperator, [left, right]);
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

            return new ASTNode(ASTType.Condition, $"{property}{comparisonOperator}{value}", []);
        }

        throw new CriteriaException("Invalid condition format");
    }
}