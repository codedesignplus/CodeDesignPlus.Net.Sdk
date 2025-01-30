using NodaTime;
using NodaTime.Text;

namespace CodeDesignPlus.Net.Criteria;

/// <summary>
/// Evaluates an abstract syntax tree (AST) and builds an expression to represent the evaluation.
/// </summary>
internal static class Evaluator
{
    private static readonly InstantPattern pattern = InstantPattern.CreateWithInvariantCulture("yyyy-MM-ddTHH:mm:ss'Z'");
    private static readonly string[] Operators = ["~=", "^=", "$=", "<=", ">=", "=", "<", ">"];

    /// <summary>
    /// Evaluates an ASTNode and builds an expression to represent the evaluation.
    /// </summary>
    /// <typeparam name="T">The type of the parameter in the expression.</typeparam>
    /// <param name="node">The ASTNode to evaluate.</param>
    /// <returns>An expression representing the evaluation of the ASTNode.</returns>
    public static Expression<Func<T, bool>> Evaluate<T>(AstNode node)
    {
        var parameter = Expression.Parameter(typeof(T));

        var expression = BuildExpression(node, parameter);

        return Expression.Lambda<Func<T, bool>>(expression, parameter);
    }

    /// <summary>
    /// Builds an expression based on the given AST node and parameter expression.
    /// </summary>
    /// <param name="node">The AST node to build the expression from.</param>
    /// <param name="parameter">The parameter expression to use in the expression.</param>
    /// <returns>The built expression.</returns>
    private static Expression BuildExpression(AstNode node, ParameterExpression parameter)
    {
        return node.Type switch
        {
            AstType.Expression => BuildExpression(node.Children[0], parameter),
            AstType.Condition => BuildConditionExpression(node, parameter),
            AstType.Operator => BuildLogicalExpression(node, parameter),
            _ => throw new CriteriaException($"Invalid AST node type: {node.Type}"),
        };
    }

    /// <summary>
    /// Builds a condition expression based on the given AST node and parameter expression.
    /// </summary>
    /// <param name="node">The AST node to build the condition expression from.</param>
    /// <param name="parameter">The parameter expression to use in the expression.</param>
    /// <returns>The built condition expression.</returns>
    private static Expression BuildConditionExpression(AstNode node, ParameterExpression parameter)
    {
        var (propertyPath, operatorSymbol, value) = SplitCondition(node.Value);
        var property = BuildPropertyExpression(propertyPath, parameter);
        var constant = CreateConstantExpression(value, property.Type);

        return BuildComparisonExpression(property, constant, operatorSymbol);
    }

    /// <summary>
    /// Builds a logical expression based on the given AST node and parameter expression.
    /// </summary>
    /// <param name="node">The AST node to build the logical expression from.</param>
    /// <param name="parameter">The parameter expression to use in the expression.</param>
    /// <returns>The built logical expression.</returns>
    private static BinaryExpression BuildLogicalExpression(AstNode node, ParameterExpression parameter)
    {
        var left = BuildExpression(node.Children[0], parameter);
        var right = BuildExpression(node.Children[1], parameter);

        return node.Value.ToLower() switch
        {
            "and" => Expression.AndAlso(left, right),
            "or" => Expression.OrElse(left, right),
            _ => throw new CriteriaException($"Invalid logical operator: {node.Value}")
        };
    }

    /// <summary>
    /// Splits a condition into property path, operator symbol, and value.
    /// </summary>
    /// <param name="condition">The condition to split.</param>
    /// <returns>A tuple containing the property path, operator symbol, and value.</returns>
    private static (string, string, string) SplitCondition(string condition)
    {
        foreach (var op in Operators)
        {
            var parts = condition.Split(new[] { op }, 2, StringSplitOptions.None);

            if (parts.Length == 2)
                return (parts[0], op, parts[1]);
        }

        throw new CriteriaException("Invalid condition format");
    }

    /// <summary>
    /// Builds a property expression based on the given property path and parameter expression.
    /// </summary>
    /// <param name="propertyPath">The property path to build the expression from.</param>
    /// <param name="parameter">The parameter expression to use in the expression.</param>
    /// <returns>The built property expression.</returns>
    private static Expression BuildPropertyExpression(string propertyPath, ParameterExpression parameter)
    {
        Expression propertyExpression = parameter;

        foreach (var propertyName in propertyPath.Split('.'))
        {
            propertyExpression = Expression.Property(propertyExpression, propertyName);
        }

        return propertyExpression;
    }

    /// <summary>
    /// Creates a constant expression based on the given value and target type.
    /// </summary>
    /// <param name="value">The value to create the constant expression from.</param>
    /// <param name="targetType">The target type of the constant expression.</param>
    /// <returns>The created constant expression.</returns>
    private static ConstantExpression CreateConstantExpression(string value, Type targetType)
    {
        try
        {
            if (targetType == typeof(Instant))
            {
                var parseResult = pattern.Parse(value);

                return Expression.Constant(parseResult.Value, targetType);
            }

            var convertedValue = Convert.ChangeType(value, targetType);
            return Expression.Constant(convertedValue, targetType);
        }
        catch (Exception ex) when (ex is InvalidCastException || ex is FormatException)
        {
            throw new CriteriaException($"Invalid value '{value}' for type {targetType}", ex);
        }
    }

    /// <summary>
    /// Builds a comparison expression based on the given property, constant, and operator symbol.
    /// </summary>
    /// <param name="property">The property expression to use in the comparison.</param>
    /// <param name="constant">The constant expression to use in the comparison.</param>
    /// <param name="operatorSymbol">The operator symbol to use in the comparison.</param>
    /// <returns>The built comparison expression.</returns>
    private static Expression BuildComparisonExpression(Expression property, Expression constant, string operatorSymbol)
    {
        return operatorSymbol switch
        {
            "^=" => Expression.Call(property, typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) })!, constant),
            "$=" => Expression.Call(property, typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string) })!, constant),
            "~=" => Expression.Call(property, typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!, constant),
            "=" => Expression.Equal(property, constant),
            "<" => Expression.LessThan(property, constant),
            ">" => Expression.GreaterThan(property, constant),
            "<=" => Expression.LessThanOrEqual(property, constant),
            ">=" => Expression.GreaterThanOrEqual(property, constant),
            _ => throw new CriteriaException($"Unsupported operator: {operatorSymbol}")
        };
    }

    /// <summary>
    /// Creates a sort expression based on the given property name.
    /// </summary>
    /// <typeparam name="T">The type of the parameter in the expression.</typeparam>
    /// <param name="propertyName">The name of the property to sort by.</param>
    /// <returns>An expression representing the sorting of the property.</returns>
    public static Expression<Func<T, object>> SortBy<T>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyName);
        var conversion = Expression.Convert(property, typeof(object));

        return Expression.Lambda<Func<T, object>>(conversion, parameter);
    }
}