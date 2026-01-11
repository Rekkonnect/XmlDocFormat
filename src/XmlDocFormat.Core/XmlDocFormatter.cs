using Garyon.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace XmlDocFormat.Core;

/*
 * Unfortunately the current implementation is very reliant on syntax,
 * instead of isolating the real content from the syntax, and then
 * reconstructing it in either /// or /** form
 * This is something that must be done for 1.1.0, so that both styles
 * can be supported properly, without problems back and forth.
 * 
 * To support the current test suite, there's a lot of workarounds, hacks
 * and bool flags, only to work around the above design limitation. All
 * such weird and unreasonable logic shortcuts are bound to be eliminated
 * once the design is properly refactored.
 */

public class XmlDocFormatter(XmlDocFormatOptions options)
{
    public string Format(string sourceCode)
    {
        var tree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = tree.GetRoot();
        var rewriter = new XmlDocRewriter(options);
        var newRoot = rewriter.Visit(root);
        return newRoot.ToFullString();
    }

    public static string Format(string code, XmlDocFormatOptions options)
    {
        var formatter = new XmlDocFormatter(options);
        return formatter.Format(code);
    }

    private class XmlDocRewriter(XmlDocFormatOptions options)
        : CSharpSyntaxRewriter
    {
        public override SyntaxToken VisitToken(SyntaxToken token)
        {
            var newToken = base.VisitToken(token);

            if (newToken.HasLeadingTrivia)
            {
                var triviaList = newToken.LeadingTrivia;
                for (int i = 0; i < triviaList.Count; i++)
                {
                    triviaList = newToken.LeadingTrivia;
                    var trivia = triviaList[i];
                    if (trivia.Kind()
                        is SyntaxKind.SingleLineDocumentationCommentTrivia
                        or SyntaxKind.MultiLineDocumentationCommentTrivia)
                    {
                        newToken = ReplaceTriviaIndentation(
                            token,
                            newToken,
                            triviaList,
                            i);
                    }
                }
            }

            return newToken;

            static SyntaxToken ReplaceTriviaIndentation(
                SyntaxToken token,
                SyntaxToken newToken,
                SyntaxTriviaList triviaList,
                int docCommentIndex)
            {
                var lineSpan = token.GetLocation().GetLineSpan();
                var indentationColumn = lineSpan.StartLinePosition.Character;
                var indentString = new string(' ', indentationColumn);
                var indentTrivia = SyntaxFactory.Whitespace(indentString);

                var newTrivia = triviaList.ToList();

                if (docCommentIndex > 0 && newTrivia[docCommentIndex - 1].IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    var prev = newTrivia[docCommentIndex - 1];
                    newTrivia[docCommentIndex - 1] = indentTrivia;
                }
                else
                {
                    newTrivia.Insert(docCommentIndex, indentTrivia);
                }

                newToken = newToken.WithLeadingTrivia(newTrivia);
                return newToken;
            }

        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (trivia.HasStructure
                && trivia.Kind()
                    is SyntaxKind.SingleLineDocumentationCommentTrivia
                    or SyntaxKind.MultiLineDocumentationCommentTrivia)
            {
                var structure = (DocumentationCommentTriviaSyntax)trivia.GetStructure()!;
                var visited = VisitDocumentationCommentTrivia(structure);
                if (visited != structure && visited != null)
                {
                    return SyntaxFactory.Trivia((StructuredTriviaSyntax)visited);
                }
            }
            return base.VisitTrivia(trivia);
        }

        public override SyntaxNode? VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
            var isMultiline = node.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia);
            var indentation = GetIndentation(node);

            var newContent = new List<XmlNodeSyntax>();

            foreach (var child in node.Content)
            {
                if (child is XmlElementSyntax element)
                {
                    newContent.Add(FormatElement(element, indentation, indentation, indentation));
                }
                else if (child is XmlTextSyntax text)
                {
                    if (isMultiline)
                    {
                        var tokenized = TokenizeWhitespaces(text);
                        newContent.AddRange(tokenized);

                        static IEnumerable<XmlTextSyntax> TokenizeWhitespaces(XmlTextSyntax node)
                        {
                            node = FormatXmlText(node);

                            if (node.GetText().ToString() is var text
                                && text.IsWhiteSpace())
                            {
                                return text.TokenizeWhitespace()
                                    .Select(SyntaxFactory.XmlTextLiteral)
                                    .Select(static token => SyntaxFactory.XmlText(token));
                            }

                            return [node];
                        }
                    }
                    else
                    {
                        newContent.Add(text);
                    }
                }
                else
                {
                    newContent.Add(child);
                }
            }

            if (isMultiline)
            {
                if (newContent is [.., var last]
                    && last is XmlTextSyntax { TextTokens: [SyntaxToken singleToken] }
                    && singleToken.Kind()
                        is SyntaxKind.XmlTextLiteralNewLineToken
                        or SyntaxKind.XmlTextLiteralToken)
                {
                    newContent.RemoveLast();
                }

                var isLineStart = true;
                for (int i = 0; i < newContent.Count; i++)
                {
                    ProcessContent();

                    void ProcessContent()
                    {
                        var content = newContent[i];

                        if (isLineStart)
                        {
                            var hasLeadingSlashes = content.HasLeadingTrivia
                                && content.GetLeadingTrivia() is var leading
                                && leading.Any(IsSingleLineExteriorTrivia)
                                ;

                            var shouldInsertTrivia = !hasLeadingSlashes;
                            if (shouldInsertTrivia)
                            {
                                var replaced = InsertDocumentationExteriorTrivia(content);
                                content = replaced;
                            }
                        }

                        newContent[i] = content;
                        isLineStart = content.GetText()[^1] is '\r' or '\n';
                    }
                }
            }

            return SyntaxFactory.DocumentationCommentTrivia(
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxFactory.List(newContent));
        }

        private static TNode InsertDocumentationExteriorTrivia<TNode>(TNode node)
            where TNode : SyntaxNode
        {
            return node.WithLeadingTrivia(
                InsertDocumentationExteriorTrivia(node.GetLeadingTrivia()));
        }

        private static SyntaxTriviaList InsertDocumentationExteriorTrivia(SyntaxTriviaList list)
        {
            var trivia = SyntaxFactory.DocumentationCommentExterior("///");
            return list.Insert(0, trivia);
        }

        private static bool IsSingleLineExteriorTrivia(SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia)
                && trivia.ToString() is "///"
                ;
        }

        private static bool IsMultilineExteriorTrivia(SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia)
                && trivia.ToString() is not "///"
                ;
        }

        private static XmlTextSyntax FormatXmlText(XmlTextSyntax node)
        {
            // Strip all tokens of their trivia,
            // which handles the case of leading asterisks in /** comments
            var textTokens = node.TextTokens
                .Select(RemoveMultilineExteriorTrivia);
            node = node.WithTextTokens([.. textTokens]);

            return SyntaxFactory.XmlText(
                SyntaxFactory.TokenList(textTokens));
        }

        private static SyntaxToken RemoveMultilineExteriorTrivia(SyntaxToken token)
        {
            return token.WithLeadingTrivia(
                    token.LeadingTrivia.WhereNot(IsMultilineExteriorTrivia))
                .WithTrailingTrivia(
                    token.TrailingTrivia.WhereNot(IsMultilineExteriorTrivia))
                ;
        }

        private XmlElementSyntax FormatElement(XmlElementSyntax element, int contentIndentation, int endTagIndentation, int baseIndentation)
        {
            var items = FlattenContent(element.Content, contentIndentation, baseIndentation).ToList();
            var newContent = FlowContent(items, contentIndentation, baseIndentation);

            var endIndentString = new string(' ', baseIndentation);
            var innerIndentation = new string(' ', Math.Max(0, endTagIndentation - baseIndentation));
            var endExterior = endIndentString + "/// " + innerIndentation;
            // Use Whitespace trivia
            var endTagTrivia = SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, endExterior);

            var endTag = element.EndTag;
            // Use DocumentationCommentExterior for EndTag trivia
            var newEndTag = endTag.WithLeadingTrivia(endTagTrivia);

            return element
                .WithContent(SyntaxFactory.List(newContent))
                .WithEndTag(newEndTag);
        }

        private IEnumerable<FormatItem> FlattenContent(SyntaxList<XmlNodeSyntax> content, int indentation, int baseIndentation)
        {
            var items = content.SelectMany(node => CreateFormatItem(node, indentation, baseIndentation));
            return MergeItems(items);

            static IEnumerable<FormatItem> MergeItems(IEnumerable<FormatItem> items)
            {
                FormatItem? flushed;
                var buffer = new List<FormatItem>();
                foreach (var item in items)
                {
                    if (item.Type is ItemType.Word or ItemType.EmptyNode)
                    {
                        if (buffer.Count is 0 || item.NoLeadingSpace)
                        {
                            buffer.Add(item);
                            continue;
                        }
                    }

                    flushed = Flush();
                    if (flushed is not null)
                    {
                        yield return flushed;
                    }

                    yield return item;
                }

                flushed = Flush();
                if (flushed is not null)
                {
                    yield return flushed;
                }

                FormatItem? Flush()
                {
                    var item = CreateItem();
                    buffer.Clear();
                    return item;

                    FormatItem? CreateItem()
                    {
                        if (buffer is [])
                        {
                            return null;
                        }

                        var noLeadingSpace = buffer.First().NoLeadingSpace;
                        var concatenatedText = string.Concat(buffer.Select(i => i.ActualText));
                        var resultingItem = new FormatItem(
                            ItemType.Word,
                            concatenatedText,
                            node: null,
                            noLeadingSpace);

                        return resultingItem;
                    }
                }
            }
        }

        private IEnumerable<FormatItem> CreateFormatItem(XmlNodeSyntax node, int indentation, int baseIndentation)
        {
            switch (node)
            {
                case XmlTextSyntax textSyntax:
                    return XmlTextNodeItems(textSyntax).ToList();

                case XmlElementSyntax element:
                    if (XmlTagNameMatches(element, "para"))
                    {
                        var formatted = FormatElement(element, indentation, indentation, baseIndentation);
                        return Item(new(ItemType.InlineNode, formatted));
                    }
                    if (XmlTagNameMatches(element, "list"))
                    {
                        // Increase indentation for list items content, but explicitly keep end tag at current indentation
                        var formatted = FormatElement(element, indentation + 4, indentation, baseIndentation);
                        return Item(new(ItemType.InlineNode, formatted));
                    }
                    if (XmlTagNameMatches(element, "item"))
                    {
                        var formatted = FormatElement(element, indentation, indentation, baseIndentation);
                        return Item(new(ItemType.InlineNode, formatted));
                    }
                    return Item(new(ItemType.InlineNode, element));

                case XmlEmptyElementSyntax emptyElement:
                    if (XmlTagNameMatches(emptyElement, "br"))
                    {
                        return Item(new(ItemType.HardBreak, emptyElement));
                    }

                    return Item(new(ItemType.EmptyNode, text: null, emptyElement, noLeadingSpace: true));

                default:
                    return Item(new(ItemType.InlineNode, node));
            }

            static IReadOnlyList<FormatItem> Item(FormatItem item)
            {
                return [item];
            }

            static IEnumerable<FormatItem> XmlTextNodeItems(XmlTextSyntax textSyntax)
            {
                bool isFirstToken = true;
                var mergedTokens = MergeTextWithEntityTokens(textSyntax.TextTokens);
                foreach (var token in mergedTokens)
                {
                    if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                    {
                        yield return new(ItemType.Space);
                        isFirstToken = false;
                        continue;
                    }

                    bool hasLeadingSpace = token.Text.Length > 0 && char.IsWhiteSpace(token.Text[0]);
                    bool hasTrailingSpace = token.Text.Length > 0 && char.IsWhiteSpace(token.Text[^1]);

                    var parts = token.Text
                        .Split([' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

                    bool isFirstPart = true;
                    foreach (var part in parts)
                    {
                        var noLeadingSpace = isFirstToken && isFirstPart && !hasLeadingSpace;
                        yield return new FormatItem(ItemType.Word, part, null, noLeadingSpace);
                        isFirstPart = false;
                    }

                    if (hasTrailingSpace)
                    {
                        yield return new FormatItem(ItemType.Space);
                    }

                    isFirstToken = false;
                }
            }

            static IEnumerable<SyntaxToken> MergeTextWithEntityTokens(
                IEnumerable<SyntaxToken> tokens)
            {
                var merged = new List<SyntaxToken>();
                var buffer = new List<SyntaxToken>();

                foreach (var token in tokens)
                {
                    if (IsMergable(token))
                    {
                        buffer.Add(token);
                        continue;
                    }

                    Flush();
                    merged.Add(token);
                }

                Flush();
                return merged;

                static bool IsMergable(SyntaxToken token)
                {
                    return token.Kind()
                        is SyntaxKind.XmlTextLiteralToken
                        or SyntaxKind.XmlEntityLiteralToken;
                }

                void Flush()
                {
                    if (buffer is [])
                    {
                        return;
                    }

                    var mergedText = string.Concat(buffer.Select(t => t.Text));
                    var newToken = SyntaxFactory.Token(
                        buffer.First().LeadingTrivia,
                        SyntaxKind.XmlTextLiteralToken,
                        mergedText,
                        mergedText,
                        buffer.Last().TrailingTrivia);
                    merged.Add(newToken);
                    buffer.Clear();
                }
            }
        }

        private static bool XmlTagNameMatches(XmlNodeSyntax syntax, string name)
        {
            if (syntax is XmlEmptyElementSyntax empty)
            {
                var syntaxName = empty.Name;
                return XmlTagNameMatches(syntaxName, name);
            }

            if (syntax is XmlElementSyntax element)
            {
                var syntaxName = element.StartTag.Name;
                return XmlTagNameMatches(syntaxName, name);
            }

            return false;
        }

        private static bool XmlTagNameMatches(XmlNameSyntax syntax, string name)
        {
            if (syntax.Prefix is not null)
            {
                return false;
            }

            return syntax.LocalName.ValueText.Trim() == name;
        }

        private IEnumerable<XmlNodeSyntax> FlowContent(
            List<FormatItem> items,
            int contentIndentation,
            int baseIndentation)
        {
            var nodes = new List<XmlNodeSyntax>();
            var currentLineWords = new List<string>();
            var linePrefixLength = contentIndentation + 4; // Assuming 4 for ///
            var currentLineLength = linePrefixLength;
            var maxLen = options.MaxLineLength;

            var baseIndentString = new string(' ', baseIndentation);
            var extraIndent = Math.Max(0, contentIndentation - baseIndentation);
            var extraIndentString = new string(' ', extraIndent);

            nodes.Add(SyntaxFactory.XmlText(SyntaxFactory.XmlTextLiteral("\r\n")));

            bool startOfLine = true;
            bool suppressLeadingSpace = false;
            var nextSpace = SpaceType.None;

            foreach (var item in items)
            {
                switch (item.Type)
                {
                    case ItemType.Space:
                    case ItemType.RequiredSpace:
                        break;

                    case ItemType.HardBreak:
                        HandleHardBreak();
                        break;

                    case ItemType.InlineNode:
                        HandleInlineNode();
                        break;

                    case ItemType.Word:
                        HandleWord();
                        break;

                    case ItemType.EmptyNode:
                        throw new UnreachableException("Should not be handling empty nodes here");
                }

                nextSpace = item.Type switch
                {
                    ItemType.Space => SpaceType.Regular,
                    ItemType.RequiredSpace => SpaceType.Required,
                    _ => SpaceType.None,
                };

                void HandleHardBreak()
                {
                    FlushLine(true);
                    var node = item.Node;
                    if (startOfLine && node != null)
                    {
                        var first = node.GetFirstToken();
                        var newFirst = first.WithLeadingTrivia(SyntaxFactory.TriviaList(
                            SyntaxFactory.Whitespace(baseIndentString + "/// " + extraIndentString)));
                        node = node.ReplaceToken(first, newFirst);
                    }
                    if (node != null)
                    {
                        nodes.Add(node);
                    }

                    nodes.Add(SyntaxFactory.XmlText(SyntaxFactory.XmlTextLiteral("\r\n")));
                    startOfLine = true;
                    currentLineLength = linePrefixLength;
                }

                void HandleInlineNode()
                {
                    var nodeText = item.Node!.ToString();
                    var hasPreviousContent = !startOfLine || currentLineWords.Count > 0;
                    var effectiveLen = nodeText.Length + (hasPreviousContent ? 1 : 0);

                    if (currentLineLength + effectiveLen > maxLen && hasPreviousContent)
                    {
                        FlushLine(true);
                        effectiveLen = nodeText.Length;
                    }

                    if (currentLineWords.Count > 0)
                    {
                        FlushLine(false);
                    }

                    if (startOfLine)
                    {
                        var first = item.Node!.GetFirstToken();
                        var newFirst = first.WithLeadingTrivia(SyntaxFactory.TriviaList(
                            SyntaxFactory.Whitespace(baseIndentString + "/// " + extraIndentString)));
                        nodes.Add(item.Node!.ReplaceToken(first, newFirst));
                        startOfLine = false;
                    }
                    else
                    {
                        // TODO: Use item.NoLeadingSpace here too if we want to support InlineNode attachments
                        nodes.Add(SyntaxFactory.XmlText(SyntaxFactory.XmlTextLiteral(" ", " ")));
                        nodes.Add(item.Node!);
                    }

                    currentLineLength += effectiveLen;
                }

                void HandleWord()
                {
                    var wordLen = item.Text!.Length;
                    var hasPreviousContent = !startOfLine || currentLineWords.Count > 0;

                    var spaceNeeded = hasPreviousContent
                        && (!item.NoLeadingSpace || nextSpace is not SpaceType.None);

                    var effectiveLen = wordLen + (spaceNeeded ? 1 : 0);

                    if (currentLineLength + effectiveLen > maxLen && hasPreviousContent)
                    {
                        FlushLine(true);
                        // After flush, startOfLine is true. spaceNeeded becomes false.
                        effectiveLen = wordLen;
                        spaceNeeded = false;
                    }

                    if (currentLineWords.Count == 0 && item.NoLeadingSpace)
                    {
                        // If we are starting a fresh batch of words (e.g. after InlineNode)
                        // and we want no leading space, we must tell FlushLine to suppress it.
                        suppressLeadingSpace = true;
                    }

                    // If we have words already, simply appending to the list "joins" with space.
                    var canJoinWord = currentLineWords.Count > 0 && !spaceNeeded;
                    if (canJoinWord)
                    {
                        var last = currentLineWords[^1];
                        currentLineWords[^1] = last + item.Text;
                    }
                    else
                    {
                        currentLineWords.Add(item.Text!);
                    }

                    if (startOfLine && currentLineWords.Count == 1)
                    {
                        currentLineLength = linePrefixLength + wordLen;
                    }
                    else
                    {
                        currentLineLength += effectiveLen;
                    }
                }
            }

            FlushLine(true);
            return nodes;

            void FlushLine(bool finishLine)
            {
                if (currentLineWords.Count == 0)
                {
                    if (finishLine && !startOfLine)
                    {
                        nodes.Add(SyntaxFactory.XmlText(SyntaxFactory.XmlTextLiteral("\r\n")));
                        startOfLine = true;
                        currentLineLength = linePrefixLength;
                        suppressLeadingSpace = false;
                    }
                    return;
                }

                var text = string.Join(" ", currentLineWords);

                // Add indentation if start of line
                string tokenText;
                if (startOfLine)
                {
                    tokenText = text; // Content only
                }
                else
                {
                    tokenText = (suppressLeadingSpace ? "" : " ") + text;
                }

                var exteriorTrivia = SyntaxFactory.TriviaList(
                    SyntaxFactory.Whitespace(baseIndentString + "/// " + extraIndentString));

                var token = SyntaxFactory.XmlTextLiteral(
                    startOfLine ? exteriorTrivia : SyntaxFactory.TriviaList(),
                    tokenText,
                    tokenText,
                    SyntaxFactory.TriviaList());

                nodes.Add(SyntaxFactory.XmlText(token));

                if (finishLine)
                {
                    nodes.Add(SyntaxFactory.XmlText(SyntaxFactory.XmlTextLiteral("\r\n")));
                    startOfLine = true;
                    currentLineLength = linePrefixLength;
                    suppressLeadingSpace = false;
                }
                else
                {
                    startOfLine = false;
                    suppressLeadingSpace = false;
                }
                currentLineWords.Clear();
            }
        }

        private static int GetIndentation(DocumentationCommentTriviaSyntax node)
        {
            if (node.ParentTrivia.Token != default)
            {
                var token = node.ParentTrivia.Token;
                // Use the token's start character position to align documentation with the element
                var lineSpan = token.GetLocation().GetLineSpan();
                return lineSpan.StartLinePosition.Character;
            }
            return 0;
        }

        private class FormatItem(ItemType type, string? text, XmlNodeSyntax? node, bool noLeadingSpace = false)
        {
            public ItemType Type { get; init; } = type;
            public string? Text { get; init; } = text;
            public XmlNodeSyntax? Node { get; init; } = node;
            public bool NoLeadingSpace { get; init; } = noLeadingSpace;

            public string ActualText => Text ?? Node?.ToString() ?? string.Empty;

            public FormatItem(ItemType type)
                : this(type, null, null) { }

            public FormatItem(ItemType type, string text)
                : this(type, text, null) { }

            public FormatItem(ItemType type, XmlNodeSyntax node)
                : this(type, null, node) { }
        }

        private enum ItemType
        {
            Word,
            Space,
            RequiredSpace,
            HardBreak,
            InlineNode,
            EmptyNode,
        }

        private enum SpaceType
        {
            None,
            Regular,
            Required,
        }
    }
}
