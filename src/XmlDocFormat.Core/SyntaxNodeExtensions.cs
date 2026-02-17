using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocFormat.Core;

public static class SyntaxExtensions
{
    extension(SyntaxNode node)
    {
        public RawSyntaxKind RawSyntaxKind => node.RawKind;
    }

    extension(SyntaxToken token)
    {
        public RawSyntaxKind RawSyntaxKind => token.RawKind;
    }

    extension(SyntaxTrivia trivia)
    {
        public RawSyntaxKind RawSyntaxKind => trivia.RawKind;
    }
}
