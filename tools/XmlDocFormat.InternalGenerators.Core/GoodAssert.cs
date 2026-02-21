using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace XmlDocFormat.InternalGenerators.Core;

public static class GoodAssert
{
    public static T OfType<T>(
        object? @object,
        string? message = null,
        [CallerArgumentExpression(nameof(@object))]
        string argumentExpression = "")
        where T : notnull
    {
        if (@object is not T t)
        {
            throw new AssertionException(message, argumentExpression);
        }

        return t;
    }

    public static void NotNull<T>(
        [NotNull]
        T? nullable,
        string? message = null,
        [CallerArgumentExpression(nameof(nullable))]
        string argumentExpression = "")
    {
        if (nullable is null)
        {
            throw new AssertionException(message, argumentExpression);
        }
    }

    public static void True(
        bool condition,
        string? message = null,
        [CallerArgumentExpression(nameof(condition))]
        string argumentExpression = "")
    {
        if (!condition)
        {
            throw new AssertionException(message, argumentExpression);
        }
    }

    public static void False(
        bool condition,
        string? message = null,
        [CallerArgumentExpression(nameof(condition))]
        string argumentExpression = "")
    {
        if (condition)
        {
            throw new AssertionException(message, argumentExpression);
        }
    }
}
