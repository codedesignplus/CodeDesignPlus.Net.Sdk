namespace CodeDesignPlus.Net.Mongo.Extensions;

/// <summary>
/// Extension methods for <see cref="FilterDefinition{TEntity}"/>.
/// </summary>
public static class FilterExtension
{
    /// <summary>
    /// Build a filter to search for entities that inherit from <see cref="AggregateRoot"/> and have the tenant identifier.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to filter.</typeparam>
    /// <param name="filter">The filter to apply.</param>
    /// <param name="tenant">The tenant identifier.</param>
    /// <returns>The filter to apply.</returns>
    /// <exception cref="MongoException">The tenant identifier is required for entities that inherit from <see cref="AggregateRoot"/>.</exception>
    internal static FilterDefinition<TEntity> BuildFilter<TEntity>(this FilterDefinition<TEntity> filter, Guid tenant) where TEntity : class, IEntityBase
    {
        if (typeof(TEntity).IsSubclassOf(typeof(AggregateRoot)))
        {
            if (Guid.Empty.Equals(tenant))
                throw new Exceptions.MongoException("The tenant identifier is required for entities that inherit from AggregateRoot.");

            filter = Builders<TEntity>.Filter.And(filter, Builders<TEntity>.Filter.Eq(e => (e as AggregateRoot)!.Tenant, tenant));
        }

        return filter;
    }

    /// <summary>
    /// Convert a Expression filter to  a MongoDB FilterDefinition
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to filter.</typeparam>
    /// <param name="expression">The expression to convert.</param>
    /// <param name="isAggregation">Indicates if the filter is for an aggregation.</param>
    /// <returns>The filter to apply.</returns>
    internal static FilterDefinition<TEntity> ToFilterDefinition<TEntity>(this Expression<Func<TEntity, bool>> expression, bool isAggregation = false)
        where TEntity : class, IEntityBase
    {
        var parameter = expression.Parameters[0];

        var converter = new ExpressionConverter(parameter, isAggregation);

        var bsonFilter = converter.Convert(expression.Body);

        return new BsonDocumentFilterDefinition<TEntity>(bsonFilter);

    }
}
