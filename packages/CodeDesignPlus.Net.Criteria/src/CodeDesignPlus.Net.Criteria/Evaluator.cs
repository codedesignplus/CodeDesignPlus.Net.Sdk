using System.Reflection;
using NodaTime;
using NodaTime.Text;

namespace CodeDesignPlus.Net.Criteria;

/// <summary>
/// Evaluates an abstract syntax tree (AST) and builds an expression to represent the evaluation.
/// </summary>
internal static class Evaluator
{
    private static readonly InstantPattern pattern = InstantPattern.CreateWithInvariantCulture("yyyy-MM-ddTHH:mm:ss'Z'");
    private static readonly string[] Operators = ["~=", "^=", "$=", "!=", "@=", "<=", ">=", "=", "<", ">"];

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
            _ => throw new CriteriaException(CriteriaErrors.InvalidFilter),
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

        if (operatorSymbol == "@=")
            return BuildInExpression(property, value, propertyPath);

        var constant = CreateConstantExpression(value, property.Type, propertyPath);

        return BuildComparisonExpression(property, constant, operatorSymbol, propertyPath);
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
            _ => throw new CriteriaException(CriteriaErrors.InvalidFilter)
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

        throw new CriteriaException(CriteriaErrors.InvalidCondition);
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
        var currentType = parameter.Type;

        foreach (var propertyName in propertyPath.Split('.'))
        {
            var pi = currentType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                ?? throw new CriteriaException(CriteriaErrors.UnknownProperty.With(propertyPath));

            propertyExpression = Expression.Property(propertyExpression, pi);
            currentType = pi.PropertyType;
        }

        return propertyExpression;
    }

    /// <summary>
    /// Creates a constant expression based on the given value and target type.
    /// </summary>
    /// <param name="value">The value to create the constant expression from.</param>
    /// <param name="targetType">The target type of the constant expression.</param>
    /// <param name="propertyPath">The filtered property, named in the error when the value does not fit its type.</param>
    /// <returns>The created constant expression.</returns>
    private static ConstantExpression CreateConstantExpression(string value, Type targetType, string propertyPath)
    {
        try
        {
            if (string.Equals(value, "null", StringComparison.OrdinalIgnoreCase))
            {
                return Expression.Constant(null, targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null
                    ? typeof(object)
                    : targetType);
            }

            // Un tipo opcional (Instant?, int?, un enum?) se convierte como su tipo de fondo: antes caía en
            // Convert.ChangeType, que no sabe de Nullable, y un filtro sobre una fecha opcional lanzaba
            // InvalidCastException (pendings/277).
            var underlyingType = Nullable.GetUnderlyingType(targetType);

            if (underlyingType is not null && targetType != typeof(Guid?))
            {
                if (string.IsNullOrWhiteSpace(value))
                    return Expression.Constant(null, targetType);

                var underlying = CreateConstantExpression(value, underlyingType, propertyPath);

                return Expression.Constant(underlying.Value, targetType);
            }

            if (targetType == typeof(Instant))
            {
                var parseResult = pattern.Parse(value);

                return Expression.Constant(parseResult.Value, targetType);
            }

            if (targetType == typeof(Guid))
            {
                var guidValue = Guid.Parse(value);

                return Expression.Constant(guidValue, targetType);
            }

            if (targetType == typeof(Guid?))
            {
                var guidValue = string.IsNullOrWhiteSpace(value) ? (Guid?)null : Guid.Parse(value);

                return Expression.Constant(guidValue, targetType);
            }

            if (targetType.IsEnum)
            {
                var enumValue = Enum.Parse(targetType, value, ignoreCase: true);
                return Expression.Constant(enumValue, targetType);
            }

            if (targetType == typeof(bool))
            {
                var boolValue = value.ToLowerInvariant() switch
                {
                    "true" or "1" => true,
                    "false" or "0" => false,
                    _ => throw new FormatException($"Cannot convert '{value}' to Boolean.")
                };

                return Expression.Constant(boolValue, targetType);
            }

            var convertedValue = Convert.ChangeType(value, targetType, System.Globalization.CultureInfo.InvariantCulture);

            return Expression.Constant(convertedValue, targetType);
        }
        catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is ArgumentException || ex is OverflowException)
        {
            throw new CriteriaException(CriteriaErrors.InvalidValue.With(value, propertyPath), ex);
        }
    }

    /// <summary>
    /// Builds a comparison expression based on the given property, constant, and operator symbol.
    /// </summary>
    /// <param name="property">The property expression to use in the comparison.</param>
    /// <param name="constant">The constant expression to use in the comparison.</param>
    /// <param name="operatorSymbol">The operator symbol to use in the comparison.</param>
    /// <param name="propertyPath">The filtered property, named in the error when the operator does not apply to it.</param>
    /// <returns>The built comparison expression.</returns>
    /// <remarks>
    /// An operator that the property type does not support (<c>~=</c> on a number, <c>&gt;</c> on a text) used to escape as
    /// an <see cref="InvalidOperationException"/> or an <see cref="ArgumentException"/>, which the microservice answered with
    /// a 500. It is a malformed filter, so it becomes a <see cref="CriteriaException"/> (Kappali pendings/403).
    /// </remarks>
    private static Expression BuildComparisonExpression(Expression property, Expression constant, string operatorSymbol, string propertyPath)
    {
        try
        {
            return operatorSymbol switch
            {
                "^=" => Expression.Call(property, typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!, constant),
                "$=" => Expression.Call(property, typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)])!, constant),
                "~=" => Expression.Call(property, typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!, constant),
                "=" => Expression.Equal(property, constant),
                "!=" => Expression.NotEqual(property, constant),
                "<" => Expression.LessThan(property, constant),
                ">" => Expression.GreaterThan(property, constant),
                "<=" => Expression.LessThanOrEqual(property, constant),
                ">=" => Expression.GreaterThanOrEqual(property, constant),
                _ => throw new CriteriaException(CriteriaErrors.UnsupportedOperator.With(operatorSymbol, propertyPath))
            };
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
        {
            throw new CriteriaException(CriteriaErrors.UnsupportedOperator.With(operatorSymbol, propertyPath), ex);
        }
    }

    /// <summary>
    /// Builds the membership test of the <c>@=</c> operator from a comma-separated list of values.
    /// </summary>
    private static Expression BuildInExpression(Expression property, string value, string propertyPath)
    {
        var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var propertyType = property.Type;

        var convertedValues = values.Select(v => ConvertInValue(v, propertyType, propertyPath)).ToList();

        var listType = typeof(List<>).MakeGenericType(propertyType);
        var list = Activator.CreateInstance(listType);
        var addMethod = listType.GetMethod("Add")!;

        foreach (var item in convertedValues)
            addMethod.Invoke(list, [item]);

        var containsMethod = listType.GetMethod("Contains", [propertyType])!;
        var listConstant = Expression.Constant(list, listType);

        return Expression.Call(listConstant, containsMethod, property);
    }

    /// <summary>
    /// Converts one value of an <c>@=</c> list to the property type; a value that does not fit is a malformed filter.
    /// </summary>
    private static object ConvertInValue(string v, Type propertyType, string propertyPath)
    {
        try
        {
            if (propertyType.IsEnum)
                return Enum.Parse(propertyType, v, ignoreCase: true);

            if (propertyType == typeof(Guid))
                return (object)Guid.Parse(v);

            if (propertyType == typeof(int))
                return Convert.ChangeType(v, typeof(int), System.Globalization.CultureInfo.InvariantCulture);

            if (propertyType == typeof(long))
                return Convert.ChangeType(v, typeof(long), System.Globalization.CultureInfo.InvariantCulture);

            return Convert.ChangeType(v, propertyType, System.Globalization.CultureInfo.InvariantCulture);
        }
        catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is ArgumentException || ex is OverflowException)
        {
            throw new CriteriaException(CriteriaErrors.InvalidValue.With(v, propertyPath), ex);
        }
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
        Expression property = parameter;
        var currentType = typeof(T);

        foreach (var part in propertyName.Split('.'))
        {
            var pi = currentType.GetProperty(part, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                ?? throw new CriteriaException(CriteriaErrors.UnknownProperty.With(propertyName));

            property = Expression.Property(property, pi);
            currentType = pi.PropertyType;
        }

        var conversion = Expression.Convert(property, typeof(object));

        return Expression.Lambda<Func<T, object>>(conversion, parameter);
    }
}