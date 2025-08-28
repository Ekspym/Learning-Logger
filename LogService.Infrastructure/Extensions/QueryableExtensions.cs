using System.Linq.Expressions;

namespace LogService.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IOrderedQueryable<TSource> SortLogs<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, bool isAscending)
    {
        return isAscending ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
    }
}