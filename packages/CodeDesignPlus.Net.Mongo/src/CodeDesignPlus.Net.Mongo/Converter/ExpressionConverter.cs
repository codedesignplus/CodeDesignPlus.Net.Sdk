using System;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CodeDesignPlus.Net.Mongo.Converter;

public class ExpressionConverter(ParameterExpression parameter, string alias) : ExpressionVisitor
{
    private readonly ParameterExpression parameter = parameter;
    private readonly string alias = alias;
    private readonly BsonDocument filterDocument = [];

    public BsonDocument Convert(Expression expression)
    {
        Visit(expression);
        return filterDocument;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        var left = node.Left;
        var right = node.Right;


        switch (node.NodeType)
        {
            case ExpressionType.Equal:
                filterDocument.Add(new BsonElement("$eq", new BsonArray {  $"$${alias}.{GetFieldName(left)}", GetConstantValue(right) }));
                break;
            case ExpressionType.AndAlso:
                var leftAnd = new ExpressionConverter(parameter, alias).Convert(left);
                var rightAnd = new ExpressionConverter(parameter, alias).Convert(right);
                filterDocument.Add("$and", new BsonArray { leftAnd, rightAnd });
                break;
            case ExpressionType.OrElse:
                var leftOr = new ExpressionConverter(parameter, alias).Convert(left);
                var rightOr = new ExpressionConverter(parameter, alias).Convert(right);
                filterDocument.Add("$or", new BsonArray { leftOr, rightOr });
                break;
            case ExpressionType.LessThan:
                filterDocument.Add(new BsonElement("$lt", new BsonArray { $"$${alias}.{GetFieldName(left)}", GetConstantValue(right) }));
                break;
            case ExpressionType.LessThanOrEqual:
                filterDocument.Add(new BsonElement("$lte", new BsonArray {  $"$${alias}.{GetFieldName(left)}", GetConstantValue(right) }));
                break;
            case ExpressionType.GreaterThan:
                filterDocument.Add(new BsonElement("$gt", new BsonArray {  $"$${alias}.{GetFieldName(left)}", GetConstantValue(right) }));
                break;
            case ExpressionType.GreaterThanOrEqual:
                filterDocument.Add(new BsonElement("$gte", new BsonArray {  $"$${alias}.{GetFieldName(left)}", GetConstantValue(right) }));
                break;
            default:
                throw new Exceptions.MongoException($"The operator '{node.NodeType}' is not supported.");
        }

        return node;
    }

    private static string GetFieldName(Expression expression)
    {
        if (expression is MemberExpression memberExpression)
            return memberExpression.Member.Name;

        throw new Exceptions.MongoException("Only member expressions for field names are supported.");
    }
    
    private static BsonValue GetConstantValue(Expression expression)
    {
        if (expression is ConstantExpression constantExpression)
            return BsonValue.Create(constantExpression.Value);

        throw new Exceptions.MongoException("Only constant expressions for values are supported.");
    }

}