namespace CodeDesignPlus.Net.Criteria;


/// <summary>
/// Tokenizer class that breaks down a filter string into tokens for logical operators, comparison operators, properties, and values.
/// </summary>
public class Tokenizer
{
    // Array of logical operators to be recognized
    private static readonly string[] LogicalOperators = ["and", "or"];
    // Array of comparison operators to be recognized
    private static readonly string[] ComparisonOperators = ["*=*", "*=", "=*", "<=", ">=", "=", "<", ">"];

    /// <summary>
    /// Tokenizes the input string into a list of tokens.
    /// </summary>
    /// <param name="input">The input filter string.</param>
    /// <returns>A list of tokens.</returns>
    public static List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();
        var parts = input.Split('|');

        foreach (var part in parts)
        {
            if (LogicalOperators.Contains(part.ToLower()))
            {
                tokens.Add(new Token(TokenType.LogicalOperator, part.ToLower()));
            }
            else
            {
                foreach (var compOperator in ComparisonOperators)
                {
                    var split = part.Split(new[] { compOperator }, 2, StringSplitOptions.None);
                    
                    if (split.Length == 2)
                    {
                        tokens.Add(new Token(TokenType.Property, split[0]));
                        tokens.Add(new Token(TokenType.ComparisonOperator, compOperator));
                        tokens.Add(new Token(TokenType.Value, split[1]));
                        break;
                    }
                }
            }
        }

        return tokens;
    }
}