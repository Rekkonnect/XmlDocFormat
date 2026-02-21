using Microsoft.CodeAnalysis;

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
