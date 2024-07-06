using System.Linq.Expressions;
using System.Reflection;
using CodeDesignPlus.Net.Criteria.Extensions;
using CodeDesignPlus.Net.Mongo.Converter;
using CodeDesignPlus.Net.Mongo.Test.Helpers.Models;
using MongoDB.Bson;

namespace CodeDesignPlus.Net.Mongo.Test.Converter;

public class ExpressionConverterTest
{
    [Fact]
    public void Convert_WhenExpressionIsEqual_ReturnsBsonDocumentWithEqOperator()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var parameter = Expression.Parameter(typeof(Order), "x");
        var alias = "alias";
        Expression<Func<Order, bool>> expression = x => x.Id == guid;
        var criteria = new Net.Core.Abstractions.Models.Criteria.Criteria
        {
            Filters = $"Name=Order 1"
        };

        var expression2 = criteria.GetFilterExpression<Order>();
        var converter = new ExpressionConverter(parameter, alias);

        // Act
        var result = converter.Convert(expression2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ElementCount);
        Assert.True(result.Contains("$eq"));
        Assert.IsType<BsonArray>(result["$eq"]);

        var eqArray = (BsonArray)result["$eq"];
        Assert.Equal(2, eqArray.Count);
        Assert.Equal("$$alias.Name", eqArray[0].AsString);
        Assert.Equal("Order 1", eqArray[1].AsString);
    }

    [Fact]
    public void Convert_WhenExpressionIsNotEqual_ThrowException()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var parameter = Expression.Parameter(typeof(Order), "x");
        var alias = "alias";
        var expression = Expression.NotEqual(Expression.Property(parameter, "Id"), Expression.Constant(guid));
        var converter = new ExpressionConverter(parameter, alias);

        // Act
        var exception = Assert.Throws<Mongo.Exceptions.MongoException>(() => converter.Convert(expression));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal("The operator 'NotEqual' is not supported.", exception.Message);
    }

    [Fact]
    public void Convert_WhenExpressionIsLessThan_ReturnsBsonDocumentWithLtOperator()
    {
        // Arrange
        var parameter = Expression.Parameter(typeof(Order), "x");
        var alias = "alias";
        var expression = Expression.LessThan(Expression.Property(parameter, "Total"), Expression.Constant((decimal)10));
        var converter = new ExpressionConverter(parameter, alias);

        // Act
        var result = converter.Convert(expression);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ElementCount);
        Assert.True(result.Contains("$lt"));
        Assert.IsType<BsonArray>(result["$lt"]);

        var ltArray = (BsonArray)result["$lt"];
        Assert.Equal(2, ltArray.Count);
        Assert.Equal("$$alias.Total", ltArray[0].AsString);
        Assert.Equal(10, ltArray[1].AsDecimal);
    }

    [Fact]
    public void Convert_WhenExpressionIsLessThanOrEqual_ReturnsBsonDocumentWithLteOperator()
    {
        // Arrange
        var parameter = Expression.Parameter(typeof(Order), "x");
        var alias = "alias";
        var expression = Expression.LessThanOrEqual(Expression.Property(parameter, "Total"), Expression.Constant((decimal)10));
        var converter = new ExpressionConverter(parameter, alias);

        // Act
        var result = converter.Convert(expression);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ElementCount);
        Assert.True(result.Contains("$lte"));
        Assert.IsType<BsonArray>(result["$lte"]);

        var lteArray = (BsonArray)result["$lte"];
        Assert.Equal(2, lteArray.Count);
        Assert.Equal("$$alias.Total", lteArray[0].AsString);
        Assert.Equal(10, lteArray[1].AsDecimal);
    }

    [Fact]
    public void Convert_WhenExpressionIsGreaterThan_ReturnsBsonDocumentWithGtOperator()
    {
        // Arrange
        var parameter = Expression.Parameter(typeof(Order), "x");
        var alias = "alias";
        var expression = Expression.GreaterThan(Expression.Property(parameter, "Total"), Expression.Constant((decimal)10));
        var converter = new ExpressionConverter(parameter, alias);

        // Act
        var result = converter.Convert(expression);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ElementCount);
        Assert.True(result.Contains("$gt"));
        Assert.IsType<BsonArray>(result["$gt"]);

        var gtArray = (BsonArray)result["$gt"];
        Assert.Equal(2, gtArray.Count);
        Assert.Equal("$$alias.Total", gtArray[0].AsString);
        Assert.Equal(10, gtArray[1].AsDecimal);
    }

    [Fact]
    public void Convert_WhenExpressionIsGreaterThanOrEqual_ReturnsBsonDocumentWithGteOperator()
    {
        // Arrange
        var parameter = Expression.Parameter(typeof(Order), "x");
        var alias = "alias";
        var expression = Expression.GreaterThanOrEqual(Expression.Property(parameter, "Total"), Expression.Constant((decimal)10));
        var converter = new ExpressionConverter(parameter, alias);

        // Act
        var result = converter.Convert(expression);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ElementCount);
        Assert.True(result.Contains("$gte"));
        Assert.IsType<BsonArray>(result["$gte"]);

        var gteArray = (BsonArray)result["$gte"];
        Assert.Equal(2, gteArray.Count);
        Assert.Equal("$$alias.Total", gteArray[0].AsString);
        Assert.Equal(10, gteArray[1].AsDecimal);
    }

    [Fact]
    public void Convert_WhenExpressionIsAndAlso_ReturnsBsonDocumentWithAndOperator()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var parameter = Expression.Parameter(typeof(Order), "x");

        var criteria = new Core.Abstractions.Models.Criteria.Criteria
        {
            Filters = "Name=Order|and|Description=Descript"
        };

        var expression2 = criteria.GetFilterExpression<Order>();

        var converter = new ExpressionConverter(parameter, null!);

        // Act
        var result = converter.Convert(expression2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ElementCount);
        Assert.True(result.Contains("$and"));
        Assert.IsType<BsonArray>(result["$and"]);

        var andArray = (BsonArray)result["$and"];
        Assert.Equal(2, andArray.Count);
        Assert.IsType<BsonDocument>(andArray[0]);
        Assert.IsType<BsonDocument>(andArray[1]);

        var leftAnd = (BsonDocument)andArray[0];
        var rightAnd = (BsonDocument)andArray[1];
        Assert.True(leftAnd.Contains("$eq"));
        Assert.True(rightAnd.Contains("$eq"));
        Assert.Equal("$$.Name", ((BsonArray)leftAnd["$eq"])[0].AsString);
        Assert.Equal("Order", ((BsonArray)leftAnd["$eq"])[1].AsString);
        Assert.Equal("$$.Description", ((BsonArray)rightAnd["$eq"])[0].AsString);
        Assert.Equal("Descript", ((BsonArray)rightAnd["$eq"])[1].AsString);
    }

    [Fact]
    public void Convert_WhenExpressionIsOrAlso_ReturnsBsonDocumentWithAndOperator()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var parameter = Expression.Parameter(typeof(Order), "x");

        var criteria = new Core.Abstractions.Models.Criteria.Criteria
        {
            Filters = "Name=Order|or|Description=Descript"
        };

        var expression2 = criteria.GetFilterExpression<Order>();

        var converter = new ExpressionConverter(parameter, null!);

        // Act
        var result = converter.Convert(expression2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ElementCount);
        Assert.True(result.Contains("$or"));
        Assert.IsType<BsonArray>(result["$or"]);

        var andArray = (BsonArray)result["$or"];
        Assert.Equal(2, andArray.Count);
        Assert.IsType<BsonDocument>(andArray[0]);
        Assert.IsType<BsonDocument>(andArray[1]);

        var leftAnd = (BsonDocument)andArray[0];
        var rightAnd = (BsonDocument)andArray[1];
        Assert.True(leftAnd.Contains("$eq"));
        Assert.True(rightAnd.Contains("$eq"));
        Assert.Equal("$$.Name", ((BsonArray)leftAnd["$eq"])[0].AsString);
        Assert.Equal("Order", ((BsonArray)leftAnd["$eq"])[1].AsString);
        Assert.Equal("$$.Description", ((BsonArray)rightAnd["$eq"])[0].AsString);
        Assert.Equal("Descript", ((BsonArray)rightAnd["$eq"])[1].AsString);
    }

    [Fact]
    public void GetFieldName_InvalidExpression_ThrowsMongoException()
    {
        // Arrange
        var converterType = typeof(ExpressionConverter);
        var methodInfo = converterType.GetMethod("GetFieldName", BindingFlags.NonPublic | BindingFlags.Static);
        var invalidExpression = Expression.Constant(5);

        // Act
        var exception = Record.Exception(() => methodInfo!.Invoke(null, [invalidExpression]));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<TargetInvocationException>(exception);
        Assert.IsType<Mongo.Exceptions.MongoException>(exception.InnerException);
        Assert.Equal("Only member expressions for field names are supported.", exception.InnerException.Message);
    }

    [Fact]
    public void GetConstantValue_InvalidExpression_ThrowsMongoException()
    {
        // Arrange
        var converterType = typeof(ExpressionConverter);
        var methodInfo = converterType.GetMethod("GetConstantValue", BindingFlags.NonPublic | BindingFlags.Static);
        var invalidExpression = Expression.Parameter(typeof(int), "x"); // Using a parameter expression to trigger the exception

        // Act
        var exception = Record.Exception(() => methodInfo!.Invoke(null, [invalidExpression]));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<TargetInvocationException>(exception);
        Assert.IsType<Mongo.Exceptions.MongoException>(exception.InnerException);
        Assert.Equal("Only constant expressions for values are supported.", exception.InnerException.Message);
    }
}
