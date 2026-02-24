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

    public static void IfNotNull<T>(
        this T? source,
        Action<T> action)
        where T : class
    {
        if (source is not null)
        {
            action(source);
        }
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        where T : class
    {
        return source.Where(static s => s is not null)!;
    }
}
