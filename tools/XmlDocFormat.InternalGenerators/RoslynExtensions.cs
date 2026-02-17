using Microsoft.CodeAnalysis;

namespace XmlDocFormat.InternalGenerators;

public static class RoslynExtensions
{
    public static T? TryGetAt<T>(this SeparatedSyntaxList<T> list, int index)
        where T : SyntaxNode
    {
        if (list.Count <= index)
        {
            return null;
        }

        return list[index];
    }
}
