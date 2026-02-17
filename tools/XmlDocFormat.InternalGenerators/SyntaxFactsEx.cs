using Microsoft.CodeAnalysis;

namespace XmlDocFormat.InternalGenerators;

public static class SyntaxFactsEx
{
    public static string? GetArgumentName<T>(T argumentSyntax)
        where T : SyntaxNode
    {
        return argumentSyntax switch
        {
            Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax csArgument
                => csArgument.NameColon?.Name?.Identifier.Text,

            Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax csArgument
                => csArgument.NameColon?.Name?.Identifier.Text,

            _ => null,
        };
    }
}
