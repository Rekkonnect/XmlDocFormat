using Garyon.Objects;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace XmlDocFormat.Core;

public sealed class CSharpXmlDocFormatter(XmlDocFormatOptions options)
    : BaseXmlDocFormatter(options)
{
    protected override SyntaxTree ParseText(string text)
    {
        return CSharpSyntaxTree.ParseText(text);
    }

    protected override IReadOnlyList<StringReplacement> GetReplacements(
        string sourceCode,
        SyntaxTree tree)
    {
        var root = tree.GetRoot();

        var lineEnding = sourceCode.GetAppliedLineEnding(Environment.NewLine);

        var replacements = new List<StringReplacement>();

        foreach (var trivia in root.DescendantTrivia())
        {
            var triviaKind = trivia.Kind();
            var isDocumentationTrivia = triviaKind
                is SyntaxKind.SingleLineDocumentationCommentTrivia
                or SyntaxKind.MultiLineDocumentationCommentTrivia;
            var isMultiline = triviaKind is SyntaxKind.MultiLineDocumentationCommentTrivia;
            if (isDocumentationTrivia &&
                trivia.HasStructure &&
                trivia.GetStructure() is DocumentationCommentTriviaSyntax docComment)
            {
                var docTree = CSharpXmlDocTree.ParseFromDocumentationTrivia(docComment);

                var token = trivia.Token;
                var leading = token.LeadingTrivia;
                var baseIndentation = GetBaseIndentation(leading, SyntaxKind.WhitespaceTrivia);

                var formatter = new CSharpXmlDocTreeFormatter(Options, baseIndentation, lineEnding);
                var formattedText = formatter.FormatXmlDocs(docTree);

                var replacedSpan = trivia.FullSpan;

                // Also include the whitespace trivia before the documentation comment within the line
                // The leading trivia of the line does not belong to the documentation comment trivia itself,
                // so we need to also include it
                var documentationTriviaIndex = leading.IndexOf(trivia);
                var previousTriviaIndex = documentationTriviaIndex - 1;
                if (previousTriviaIndex >= 0
                    && leading[previousTriviaIndex] is var previousTrivia
                    && previousTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    replacedSpan = TextSpan.FromBounds(previousTrivia.SpanStart, replacedSpan.End);
                }
                var lineSuffix = isMultiline ? string.Empty : lineEnding;
                replacements.Add(new(replacedSpan, formattedText + lineSuffix));
            }
        }

        return replacements;
    }

    public static string Format(string code, XmlDocFormatOptions options)
    {
        var formatter = new CSharpXmlDocFormatter(options);
        return formatter.Format(code);
    }

    private sealed class CSharpXmlDocTreeFormatter(
        XmlDocFormatOptions options,
        string baseIndentation,
        string lineEnding)
        : BaseXmlDocTreeFormatter<CSharpXmlDocTree, CSharpXmlSyntaxFacts>(
            options, baseIndentation, lineEnding)
    {
        protected override CSharpXmlSyntaxFacts SyntaxFacts => CSharpXmlSyntaxFacts.Shared;
    }
}
