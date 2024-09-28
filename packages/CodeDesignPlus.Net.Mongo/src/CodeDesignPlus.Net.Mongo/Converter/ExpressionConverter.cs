namespace CodeDesignPlus.Net.Mongo.Converter;

/// <summary>
/// Converts LINQ expressions to MongoDB BSON documents.
/// </summary>
public class ExpressionConverter(ParameterExpression parameter, string alias) : MBS.ExpressionVisitor
{
    private readonly ParameterExpression parameter = parameter;
    private readonly string alias = alias;
    private readonly BsonDocument filterDocument = new BsonDocument();

    /// <summary>
    /// Converts the specified expression to a BSON document.
    /// </summary>
    /// <param name="expression">The expression to convert.</param>
    /// <returns>The BSON document representing the expression.</returns>
    /// <exception cref="Exceptions.MongoException">Thrown when the expression contains unsupported operators or expressions.</exception>
    public BsonDocument Convert(Expression expression)
    {
        Visit(expression);
        return filterDocument;
    }

    /// <summary>
    /// Visits the children of the <see cref="BinaryExpression"/>.
    /// </summary>
    /// <param name="node">The binary expression to visit.</param>
    /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
    /// <exception cref="Exceptions.MongoException">Thrown when the binary expression contains unsupported operators.</exception>
    protected override Expression VisitBinary(BinaryExpression node)
    {
        var left = node.Left;
        var right = node.Right;

        switch (node.NodeType)
        {
            case ExpressionType.Equal:
                filterDocument.Add(new BsonElement("$eq", new BsonArray { $"$${alias}.{GetFieldName(left)}", GetConstantValue(right) }));
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
                filterDocument.Add(new BsonElement("$lte", new BsonArray { $"$${alias}.{GetFieldName(left)}", GetConstantValue(right) }));
                break;
            case ExpressionType.GreaterThan:
                filterDocument.Add(new BsonElement("$gt", new BsonArray { $"$${alias}.{GetFieldName(left)}", GetConstantValue(right) }));
                break;
            case ExpressionType.GreaterThanOrEqual:
                filterDocument.Add(new BsonElement("$gte", new BsonArray { $"$${alias}.{GetFieldName(left)}", GetConstantValue(right) }));
                break;
            default:
                throw new Exceptions.MongoException($"The operator '{node.NodeType}' is not supported.");
        }

        return node;
    }

    /// <summary>
    /// Gets the field name from the specified expression.
    /// </summary>
    /// <param name="expression">The expression to extract the field name from.</param>
    /// <returns>The field name.</returns>
    /// <exception cref="Exceptions.MongoException">Thrown when the expression is not a member expression.</exception>
    private static string GetFieldName(Expression expression)
    {
        if (expression is MemberExpression memberExpression)
            return memberExpression.Member.Name;

        throw new Exceptions.MongoException("Only member expressions for field names are supported.");
    }

    /// <summary>
    /// Gets the constant value from the specified expression.
    /// </summary>
    /// <param name="expression">The expression to extract the constant value from.</param>
    /// <returns>The constant value.</returns>
    /// <exception cref="Exceptions.MongoException">Thrown when the expression is not a constant expression.</exception>
    private static BsonValue GetConstantValue(Expression expression)
    {
        if (expression is ConstantExpression constantExpression)
            return BsonValue.Create(constantExpression.Value);

        throw new Exceptions.MongoException("Only constant expressions for values are supported.");
    }
}