using System.Linq.Expressions;
using C = CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;

namespace CodeDesignPlus.Net.Criteria.Extensions;


public static class CriteriaExtensions
{
    public static Expression<Func<T, bool>> GetFilterExpression<T>(this C.Criteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria, nameof(criteria));

        if(string.IsNullOrEmpty(criteria.Filters))
            return x => true;

        var tokens = Tokenizer.Tokenize(criteria.Filters);
        var parser = new Parser(tokens);
        var ast = parser.Parse();

        return Evaluator.Evaluate<T>(ast);
    }

    public static Expression<Func<T, object>> GetSortByExpression<T>(this C.Criteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria, nameof(criteria));

        if(string.IsNullOrEmpty(criteria.Order?.OrderBy))
            return null;

        return Evaluator.SortBy<T>(criteria.Order.OrderBy);
    }
}