using System.Linq.Expressions;
using CodeDesignPlus.Net.Criteria.Models;

namespace CodeDesignPlus.Net.Criteria;

// public class Evaluator
// {
//     private static readonly string[] operators = ["~=", "^=", "$=", "<=", ">=", "=", "<", ">"];

//     public static Expression<Func<T, bool>> Evaluate<T>(ASTNode node)
//     {
//         var parameter = Expression.Parameter(typeof(T), typeof(T).Name);

//         var expression = BuildExpression(node, parameter);

//         return Expression.Lambda<Func<T, bool>>(expression, parameter);
//     }

//     private static Expression BuildExpression(ASTNode node, ParameterExpression parameter)
//     {
//         if (node.Type == ASTType.Expression)
//         {
//             return BuildExpression(node.Children[0], parameter);
//         }
//         else if (node.Type == ASTType.Condition)
//         {
//             var parts = SplitCondition(node.Value);
//             var propertyPath = parts.Item1;
//             var operatorSymbol = parts.Item2;
//             var propertyValue = parts.Item3;

//             var property = BuildPropertyExpression(propertyPath, parameter);
//             var constant = CreateConstantExpression(propertyValue, property.Type);

//             return BuildComparisonExpression(property, constant, operatorSymbol);
//         }
//         else if (node.Type == ASTType.Operator)
//         {
//             var left = BuildExpression(node.Children[0], parameter);
//             var right = BuildExpression(node.Children[1], parameter);

//             if (node.Value == "and" || node.Value == "AND")
//                 return Expression.AndAlso(left, right);
//             else if (node.Value == "or" || node.Value == "OR")
//                 return Expression.OrElse(left, right);
//         }

//         throw new Exception($"AST Node Invalid: {node.Type}");
//     }

//     private static (string, string, string) SplitCondition(string condition)
//     {
//         foreach (var op in operators)
//         {
//             var parts = condition.Split(new[] { op }, 2, StringSplitOptions.None);

//             if (parts.Length == 2)
//                 return (parts[0], op, parts[1]);
//         }

//         throw new Exception("Invalid condition format");
//     }

//     private static Expression BuildPropertyExpression(string propertyPath, ParameterExpression parameter)
//     {
//         var properties = propertyPath.Split('.');

//         Expression propertyExpression = parameter;

//         foreach (var propertyName in properties)
//         {
//             propertyExpression = Expression.Property(propertyExpression, propertyName);
//         }
//         return propertyExpression;
//     }

//     private static ConstantExpression CreateConstantExpression(string value, Type targetType)
//     {
//         object convertedValue;

//         try
//         {
//             convertedValue = Convert.ChangeType(value, targetType);
//         }
//         catch (InvalidCastException)
//         {
//             throw new Exception($"Unsupported data type for comparison: {targetType}");
//         }
//         catch (FormatException)
//         {
//             throw new Exception($"Invalid format for value: {value} for type {targetType}");
//         }

//         return Expression.Constant(convertedValue, targetType);
//     }

//     private static Expression BuildComparisonExpression(Expression property, Expression constant, string operatorSymbol)
//     {
//         return operatorSymbol switch
//         {
//             "^=" => Expression.Call(property, typeof(string).GetMethod("StartsWith", [typeof(string)])!, constant),
//             "$=" => Expression.Call(property, typeof(string).GetMethod("EndsWith", [typeof(string)])!, constant),
//             "~=" => Expression.Call(property, typeof(string).GetMethod("Contains", [typeof(string)])!, constant),
//             "=" => Expression.Equal(property, constant),
//             "<" => Expression.LessThan(property, constant),
//             ">" => Expression.GreaterThan(property, constant),
//             "<=" => Expression.LessThanOrEqual(property, constant),
//             ">=" => Expression.GreaterThanOrEqual(property, constant),
//             _ => throw new Exception($"Unsupported operator: {operatorSymbol}"),
//         };
//     }

//     public static Expression<Func<T, object>> SortBy<T>(string propertyName)
//     {
//         var parameter = Expression.Parameter(typeof(T), "x");
//         var property = Expression.Property(parameter, propertyName);
//         var conversion = Expression.Convert(property, typeof(object));
//         var lambda = Expression.Lambda<Func<T, object>>(conversion, parameter);

//         return lambda;
//     }
// }


public class Evaluator
{
    private static readonly string[] Operators = { "~=", "^=", "$=", "<=", ">=", "=", "<", ">" };

    public static Expression<Func<T, bool>> Evaluate<T>(ASTNode node)
    {
        var parameter = Expression.Parameter(typeof(T));
        var expression = BuildExpression(node, parameter);

        return Expression.Lambda<Func<T, bool>>(expression, parameter);
    }

    private static Expression BuildExpression(ASTNode node, ParameterExpression parameter)
    {
        return node.Type switch
        {
            ASTType.Expression => BuildExpression(node.Children[0], parameter),
            ASTType.Condition => BuildConditionExpression(node, parameter),
            ASTType.Operator => BuildLogicalExpression(node, parameter),
            _ => throw new InvalidOperationException($"Invalid AST node type: {node.Type}"),
        };
    }

    private static Expression BuildConditionExpression(ASTNode node, ParameterExpression parameter)
    {
        var (propertyPath, operatorSymbol, value) = SplitCondition(node.Value);
        var property = BuildPropertyExpression(propertyPath, parameter);
        var constant = CreateConstantExpression(value, property.Type);

        return BuildComparisonExpression(property, constant, operatorSymbol);
    }

    private static BinaryExpression BuildLogicalExpression(ASTNode node, ParameterExpression parameter)
    {
        var left = BuildExpression(node.Children[0], parameter);
        var right = BuildExpression(node.Children[1], parameter);

        return node.Value.ToLower() switch
        {
            "and" => Expression.AndAlso(left, right),
            "or" => Expression.OrElse(left, right),
            _ => throw new InvalidOperationException($"Invalid logical operator: {node.Value}")
        };
    }

    private static (string, string, string) SplitCondition(string condition)
    {
        foreach (var op in Operators)
        {
            var parts = condition.Split(new[] { op }, 2, StringSplitOptions.None);

            if (parts.Length == 2)
                return (parts[0], op, parts[1]);
        }

        throw new InvalidOperationException("Invalid condition format");
    }

    private static Expression BuildPropertyExpression(string propertyPath, ParameterExpression parameter)
    {
        Expression propertyExpression = parameter;

        foreach (var propertyName in propertyPath.Split('.'))
        {
            propertyExpression = Expression.Property(propertyExpression, propertyName);
        }

        return propertyExpression;
    }

    private static ConstantExpression CreateConstantExpression(string value, Type targetType)
    {
        try
        {
            var convertedValue = Convert.ChangeType(value, targetType);
            return Expression.Constant(convertedValue, targetType);
        }
        catch (Exception ex) when (ex is InvalidCastException || ex is FormatException)
        {
            throw new InvalidOperationException($"Invalid value '{value}' for type {targetType}", ex);
        }
    }

    private static Expression BuildComparisonExpression(Expression property, Expression constant, string operatorSymbol)
    {
        return operatorSymbol switch
        {
            "^=" => Expression.Call(property, typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!, constant),
            "$=" => Expression.Call(property, typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)])!, constant),
            "~=" => Expression.Call(property, typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!, constant),
            "=" => Expression.Equal(property, constant),
            "<" => Expression.LessThan(property, constant),
            ">" => Expression.GreaterThan(property, constant),
            "<=" => Expression.LessThanOrEqual(property, constant),
            ">=" => Expression.GreaterThanOrEqual(property, constant),
            _ => throw new InvalidOperationException($"Unsupported operator: {operatorSymbol}")
        };
    }

    public static Expression<Func<T, object>> SortBy<T>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyName);
        var conversion = Expression.Convert(property, typeof(object));
        return Expression.Lambda<Func<T, object>>(conversion, parameter);
    }
}