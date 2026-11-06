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
    /// <remarks>
    /// The whole filter is a single expression. A condition followed by anything other than a logical operator is
    /// rejected instead of starting a second expression: the evaluator only reads the first one, so
    /// <c>status=0|status!=0</c> used to filter by <c>status=0</c> alone, without any error (Kappali pendings/403).
    /// <para>
    /// Logical operators are left-associative and share the same precedence: <c>a|or|b|and|c</c> is
    /// <c>(a or b) and c</c>. Clients rely on it to append a scope at the end of a filter, so it is kept on purpose.
    /// </para>
    /// </remarks>
    /// <returns>The root node of the AST.</returns>
    /// <exception cref="CriteriaException">Thrown when a condition is malformed or is not joined to the previous one by a logical operator.</exception>
    public AstNode Parse()
    {
        // A non-empty filter that yields no tokens (no comparison operator anywhere) used to reach the evaluator with
        // an empty root and fail with an index out of range, a 500.
        if (tokens.Count == 0)
        {
            throw new CriteriaException(CriteriaErrors.InvalidCondition);
        }

        var root = new AstNode(AstType.Expression, null, []);

        root.Children.Add(ParseExpression());

        if (position < tokens.Count)
        {
            throw new CriteriaException(CriteriaErrors.MissingLogicalOperator.With(tokens[position].Value));
        }

        return root;
    }

    private int position;

    private AstNode ParseExpression()
    {
        var left = ParseCondition();

        while (position < tokens.Count && tokens[position].Type == TokenType.LogicalOperator)
        {
            var logicalOperator = tokens[position].Value;
            position++;

            var right = ParseCondition();
            left = new AstNode(AstType.Operator, logicalOperator, [left, right]);
        }

        return left;
    }

    private AstNode ParseCondition()
    {
        if (position + 2 < tokens.Count && tokens[position].Type == TokenType.Property)
        {
            var property = tokens[position++].Value;
            var comparisonOperator = tokens[position++].Value;
            var value = tokens[position++].Value;

            return new AstNode(AstType.Condition, $"{property}{comparisonOperator}{value}", []);
        }

        throw new CriteriaException(CriteriaErrors.InvalidCondition);
    }
}