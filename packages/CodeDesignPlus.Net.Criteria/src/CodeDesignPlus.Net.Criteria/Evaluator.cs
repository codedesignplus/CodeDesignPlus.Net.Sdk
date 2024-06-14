using System.Linq.Expressions;

namespace CodeDesignPlus.Net.Criteria;


/// <summary>
/// Evaluator class that converts an abstract syntax tree (AST) into a LINQ expression.
/// </summary>
public class Evaluator
{
    // Supported comparison operators
    private static readonly string[] operators = ["*=*", "*=", "=*", "<=", ">=", "=", "<", ">"];

    /// <summary>
    /// Evaluates the AST into a LINQ expression.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="node">The root node of the AST.</param>
    /// <returns>A LINQ expression.</returns>
    public static Expression<Func<T, bool>> Evaluate<T>(ASTNode node)
    {
        var parameter = Expression.Parameter(typeof(T), typeof(T).Name);

        var expression = BuildExpression(node, parameter);

        return Expression.Lambda<Func<T, bool>>(expression, parameter);
    }

    /// <summary>
    /// Builds a LINQ expression from an AST node.
    /// </summary>
    /// <param name="node">The node to build the expression from.</param>
    /// <param name="parameter">The parameter expression to use in the expression.</param>
    /// <returns>A LINQ expression.</returns>
    private static Expression BuildExpression(ASTNode node, ParameterExpression parameter)
    {
        if (node.Type == ASTType.Expression)
        {
            return BuildExpression(node.Children[0], parameter);
        }
        else if (node.Type == ASTType.Condition)
        {
            var parts = SplitCondition(node.Value);
            var propertyPath = parts.Item1;
            var operatorSymbol = parts.Item2;
            var propertyValue = parts.Item3;

            var property = BuildPropertyExpression(propertyPath, parameter);
            var constant = CreateConstantExpression(propertyValue, property.Type);

            return BuildComparisonExpression(property, constant, operatorSymbol);
        }
        else if (node.Type == ASTType.Operator)
        {
            var left = BuildExpression(node.Children[0], parameter);
            var right = BuildExpression(node.Children[1], parameter);

            if (node.Value == "and")
                return Expression.AndAlso(left, right);
            else if (node.Value == "or")
                return Expression.OrElse(left, right);
        }

        throw new Exception($"Invalid AST node: {node.Type}");
    }

    private static (string, string, string) SplitCondition(string condition)
    {
        foreach (var op in operators)
        {
            var parts = condition.Split(new[] { op }, 2, StringSplitOptions.None);

            if (parts.Length == 2)
                return (parts[0], op, parts[1]);
        }

        throw new Exception("Invalid condition format");
    }

    /// <summary>
    /// Builds a property expression from a property path.
    /// </summary>
    /// <param name="propertyPath">The property path.</param>
    /// <param name="parameter">The parameter expression to use in the expression.</param>
    private static Expression BuildPropertyExpression(string propertyPath, ParameterExpression parameter)
    {
        var properties = propertyPath.Split('.');

        Expression propertyExpression = parameter;

        foreach (var propertyName in properties)
        {
            propertyExpression = Expression.Property(propertyExpression, propertyName);
        }
        return propertyExpression;
    }

    /// <summary>
    /// Creates a constant expression from a string value.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="targetType">The target type of the constant.</param>
    private static ConstantExpression CreateConstantExpression(string value, Type targetType)
    {
        object convertedValue;

        try
        {
            convertedValue = Convert.ChangeType(value, targetType);
        }
        catch (InvalidCastException)
        {
            throw new Exception($"Unsupported data type for comparison: {targetType}");
        }
        catch (FormatException)
        {
            throw new Exception($"Invalid format for value: {value} for type {targetType}");
        }

        return Expression.Constant(convertedValue, targetType);
    }

    /// <summary>
    /// Builds a comparison expression from a property and a constant.
    /// </summary>
    /// <param name="property">The property expression.</param>
    /// <param name="constant">The constant expression.</param>
    /// <param name="operatorSymbol">The operator symbol.</param>
    private static Expression BuildComparisonExpression(Expression property, Expression constant, string operatorSymbol)
    {
        return operatorSymbol switch
        {
            "=" => Expression.Equal(property, constant),
            "<" => Expression.LessThan(property, constant),
            ">" => Expression.GreaterThan(property, constant),
            "<=" => Expression.LessThanOrEqual(property, constant),
            ">=" => Expression.GreaterThanOrEqual(property, constant),
            "*=" => Expression.Call(property, typeof(string).GetMethod("StartsWith", [typeof(string)])!, constant),
            "=*" => Expression.Call(property, typeof(string).GetMethod("EndsWith", [typeof(string)])!, constant),
            "*=*" => Expression.Call(property, typeof(string).GetMethod("Contains", [typeof(string)])!, constant),
            _ => throw new Exception($"Unsupported operator: {operatorSymbol}"),
        };
    }

    /// <summary>
    /// Creates a sort expression for a property.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="propertyName">The name of the property to sort by.</param>
    public static Expression<Func<T, object>> SortBy<T>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyName);
        var conversion = Expression.Convert(property, typeof(object));
        var lambda = Expression.Lambda<Func<T, object>>(conversion, parameter);

        return lambda;
    }
}