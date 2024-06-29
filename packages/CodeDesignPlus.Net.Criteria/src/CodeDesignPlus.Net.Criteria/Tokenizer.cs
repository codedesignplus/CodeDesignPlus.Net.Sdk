namespace CodeDesignPlus.Net.Criteria;

/// <summary>
/// Represents a class that tokenizes an input string into a list of tokens.
/// </summary>
internal static class Tokenizer
{
    private static readonly string[] logicalOperators = ["and", "or"];
    private static readonly string[] comparisonOperators = ["~=", "^=", "$=", "<=", ">=", "=", "<", ">"];

    /// <summary>
    /// Tokenizes the input string.
    /// </summary>
    /// <param name="input">The input string to tokenize.</param>
    /// <returns>A list of tokens.</returns>
    public static List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();

        foreach (var part in input.Split('|'))
        {
            if (logicalOperators.Contains(part.ToLower()))
            {
                tokens.Add(new Token(TokenType.LogicalOperator, part.ToLower()));
            }
            else
            {
                tokens.AddRange(CreateTokensFromPart(part));
            }
        }

        return tokens;
    }

    /// <summary>
    /// Creates tokens from a given part by splitting it using comparison operators.
    /// </summary>
    /// <param name="part">The part to create tokens from.</param>
    /// <returns>An enumerable collection of tokens.</returns>
    private static IEnumerable<Token> CreateTokensFromPart(string part)
    {
        foreach (var compOperator in comparisonOperators)
        {
            var split = part.Split(new[] { compOperator }, 2, StringSplitOptions.None);

            if (split.Length == 2)
            {
                yield return new Token(TokenType.Property, split[0]);
                yield return new Token(TokenType.ComparisonOperator, compOperator);
                yield return new Token(TokenType.Value, split[1]);
                yield break;
            }
        }
    }
}