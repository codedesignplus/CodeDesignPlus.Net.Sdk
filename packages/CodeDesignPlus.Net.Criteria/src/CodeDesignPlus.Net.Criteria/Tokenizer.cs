using CodeDesignPlus.Net.Criteria.Models;

namespace CodeDesignPlus.Net.Criteria;

// public class Tokenizer
// {
//     private static readonly string[] LogicalOperators = ["and", "or"];
//     private static readonly string[] ComparisonOperators = ["~=", "^=", "$=", "<=", ">=", "=", "<", ">"];

//     public static List<Token> Tokenize(string input)
//     {
//         var tokens = new List<Token>();
//         var parts = input.Split('|');

//         foreach (var part in parts)
//         {
//             if (LogicalOperators.Contains(part.ToLower()))
//             {
//                 tokens.Add(new Token(TokenType.LogicalOperator, part.ToLower()));
//             }
//             else
//             {
//                 foreach (var compOperator in ComparisonOperators)
//                 {
//                     var split = part.Split(new[] { compOperator }, 2, StringSplitOptions.None);

//                     if (split.Length == 2)
//                     {
//                         tokens.Add(new Token(TokenType.Property, split[0]));
//                         tokens.Add(new Token(TokenType.ComparisonOperator, compOperator));
//                         tokens.Add(new Token(TokenType.Value, split[1]));
//                         break;
//                     }
//                 }
//             }
//         }

//         return tokens;
//     }
// }

public class Tokenizer
    {
        private static readonly string[] LogicalOperators = { "and", "or" };
        private static readonly string[] ComparisonOperators = { "~=", "^=", "$=", "<=", ">=", "=", "<", ">" };

        public static List<Token> Tokenize(string input)
        {
            var tokens = new List<Token>();

            foreach (var part in input.Split('|'))
            {
                if (LogicalOperators.Contains(part.ToLower()))
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

        private static IEnumerable<Token> CreateTokensFromPart(string part)
        {
            foreach (var compOperator in ComparisonOperators)
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