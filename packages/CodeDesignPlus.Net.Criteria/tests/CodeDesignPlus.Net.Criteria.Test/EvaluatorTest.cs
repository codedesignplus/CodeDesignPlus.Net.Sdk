using System.Linq.Expressions;
using System.Reflection;
using CodeDesignPlus.Net.Criteria.Exceptions;
using CodeDesignPlus.Net.Criteria.Models;

namespace CodeDesignPlus.Net.Criteria.Test;

public class EvaluatorTest
{
    [Fact]
    public void BuildExpression_InvalidNodeType_ThrowsCriteriaException()
    {
        // Arrange
        var parameter = Expression.Parameter(typeof(Order), "param");
        var node = new AstNode((AstType)999, "invalid", []);

        var methodInfo = typeof(Evaluator).GetMethod("BuildExpression", BindingFlags.NonPublic | BindingFlags.Static);

        // Act & Assert
        var exception = Assert.Throws<TargetInvocationException>(() => methodInfo!.Invoke(null, [node, parameter]));
        Assert.IsType<CriteriaException>(exception.InnerException);

        Assert.Equal("Invalid AST node type: 999", exception.InnerException.Message);
    }

    [Fact]
    public void BuildLogicalExpression_InvalidOperator_ThrowsCriteriaException()
    {
        // Arrange
        var parameter = Expression.Parameter(typeof(Order), "param");
        var leftNode = new AstNode(AstType.Condition, "Total>30", []);
        var rightNode = new AstNode(AstType.Condition, "Name~=Order", []);
        var logicalNode = new AstNode(AstType.Operator, "invalidOperator", [leftNode, rightNode]);

        var methodInfo = typeof(Evaluator).GetMethod("BuildLogicalExpression", BindingFlags.NonPublic | BindingFlags.Static);

        // Act & Assert
        var exception = Assert.Throws<TargetInvocationException>(() => methodInfo!.Invoke(null, [logicalNode, parameter]));
        Assert.IsType<CriteriaException>(exception.InnerException);
        Assert.Equal("Invalid logical operator: invalidOperator", exception.InnerException.Message);
    }

    [Fact]
    public void SplitCondition_InvalidFormat_ThrowsCriteriaException()
    {
        // Arrange
        string invalidCondition = "invalidConditionFormat";

        var methodInfo = typeof(Evaluator).GetMethod("SplitCondition", BindingFlags.NonPublic | BindingFlags.Static);

        // Act & Assert
        var exception = Assert.Throws<TargetInvocationException>(() => methodInfo!.Invoke(null, [invalidCondition]));

        Assert.IsType<CriteriaException>(exception.InnerException);
        Assert.Equal("Invalid condition format", exception.InnerException.Message);
    }

    [Fact]
    public void CreateConstantExpression_InvalidValue_ThrowsCriteriaException()
    {
        // Arrange
        string invalidValue = "invalid";
        Type targetType = typeof(int);
        var methodInfo = typeof(Evaluator).GetMethod("CreateConstantExpression", BindingFlags.NonPublic | BindingFlags.Static);

        // Act & Assert
        var exception = Assert.Throws<TargetInvocationException>(() => methodInfo!.Invoke(null, [invalidValue, targetType]));
        Assert.IsType<CriteriaException>(exception.InnerException);

        Assert.Contains($"Invalid value '{invalidValue}' for type {targetType}", exception.InnerException.Message);
    }

    [Fact]
    public void BuildComparisonExpression_UnsupportedOperator_ThrowsCriteriaException()
    {
        // Arrange
        var property = Expression.Property(Expression.Parameter(typeof(string), "x"), "Length");
        var constant = Expression.Constant(5);
        string unsupportedOperator = "!=";
        var methodInfo = typeof(Evaluator).GetMethod("BuildComparisonExpression", BindingFlags.NonPublic | BindingFlags.Static);

        // Act & Assert
        var exception = Assert.Throws<TargetInvocationException>(() => methodInfo!.Invoke(null, [property, constant, unsupportedOperator]));
        Assert.IsType<CriteriaException>(exception.InnerException);
        Assert.Equal($"Unsupported operator: {unsupportedOperator}", exception.InnerException.Message);
    }
}