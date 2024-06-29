namespace CodeDesignPlus.Net.Criteria.Extensions;

/// <summary>
/// Provides extension methods for working with criteria objects.
/// </summary>
public static class CriteriaExtensions
{
    /// <summary>
    /// Gets the filter expression based on the specified criteria.
    /// </summary>
    /// <typeparam name="T">The type of the object being filtered.</typeparam>
    /// <param name="criteria">The criteria object.</param>
    /// <returns>The filter expression as an <see cref="Expression{Func{T, bool}}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="criteria"/> is null.</exception>
    public static Expression<Func<T, bool>> GetFilterExpression<T>(this MC.Criteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        if (string.IsNullOrEmpty(criteria.Filters))
            return x => true;

        var tokens = Tokenizer.Tokenize(criteria.Filters);
        var parser = new Parser(tokens);
        var ast = parser.Parse();

        return Evaluator.Evaluate<T>(ast);
    }

    /// <summary>
    /// Gets the sort by expression based on the specified criteria.
    /// </summary>
    /// <typeparam name="T">The type of the object being sorted.</typeparam>
    /// <param name="criteria">The criteria object.</param>
    /// <returns>The sort by expression as an <see cref="Expression{Func{T, object}}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="criteria"/> is null.</exception>
    public static Expression<Func<T, object>> GetSortByExpression<T>(this MC.Criteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        if (string.IsNullOrEmpty(criteria.OrderBy))
            return null;

        return Evaluator.SortBy<T>(criteria.OrderBy);
    }
}