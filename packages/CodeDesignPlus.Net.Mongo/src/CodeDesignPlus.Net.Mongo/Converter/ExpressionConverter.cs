using MongoDB.Bson;
using System.Linq.Expressions;

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

        var fieldName = GetFieldName(left);
        var value = GetConstantValue(right);

        var aliasedFieldName = $"$${alias}.{fieldName}";

        switch (node.NodeType)
        {
            case ExpressionType.Equal:
                filterDocument.Add(new BsonElement("$eq", new BsonArray { aliasedFieldName, value }));
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
                filterDocument.Add(new BsonElement("$lt", new BsonArray { aliasedFieldName, value }));
                break;
            case ExpressionType.LessThanOrEqual:
                filterDocument.Add(new BsonElement("$lte", new BsonArray { aliasedFieldName, value }));
                break;
            case ExpressionType.GreaterThan:
                filterDocument.Add(new BsonElement("$gt", new BsonArray { aliasedFieldName, value }));
                break;
            case ExpressionType.GreaterThanOrEqual:
                filterDocument.Add(new BsonElement("$gte", new BsonArray { aliasedFieldName, value }));
                break;
            default:
                throw new NotSupportedException($"Operador binario no soportado: {node.NodeType}");
        }

        return node;
    }

    private static string GetFieldName(Expression expression)
    {
        if (expression is MemberExpression memberExpression)
            return memberExpression.Member.Name;

        throw new NotSupportedException("Solo se soportan expresiones de miembro para nombres de campos.");
    }

    private static BsonValue GetConstantValue(Expression expression)
    {
        if (expression is ConstantExpression constantExpression)
            return BsonValue.Create(constantExpression.Value);

        throw new NotSupportedException("Solo se soportan expresiones constantes para valores.");
    }
}