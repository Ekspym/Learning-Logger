using System.Collections.Concurrent;

namespace Infrastructure.Core.Extensions;

public static class EnumerableExtensions
{
    public static List<TResult> SelectList<TSource, TResult>(
        this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        return source.Select(selector).ToList();
    }
    
    public static bool TryTakeAndWait<T>(this BlockingCollection<T> collection, out T item, CancellationToken cancellationToken)
    {
        try
        {
            return collection.TryTake(out item, -1, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            item = default;
            return false;
        }
    }
}