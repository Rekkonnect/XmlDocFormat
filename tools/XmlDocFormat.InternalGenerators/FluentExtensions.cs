using Microsoft.CodeAnalysis;

namespace XmlDocFormat.InternalGenerators;

public static class FluentExtensions
{
    public static TResult? SelectOrNull<TSource, TResult>(
        this TSource? source,
        Func<TSource, TResult> selector)
        where TSource : class
        where TResult : class
    {
        if (source is null)
        {
            return null;
        }

        return selector(source);
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        where T : class
    {
        return source.Where(static s => s is not null)!;
    }
}
