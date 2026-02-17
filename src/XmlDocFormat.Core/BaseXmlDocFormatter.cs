using Garyon.Extensions;
using Garyon.Objects;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Text;
using XmlDocFormat.Core.XmlNodes;

namespace XmlDocFormat.Core;

public abstract class BaseXmlDocFormatter(XmlDocFormatOptions options)
{
    protected XmlDocFormatOptions Options { get; } = options;

    public string Format(string sourceCode)
    {
        var tree = ParseText(sourceCode);
        var replacements = GetReplacements(sourceCode, tree);

        var replacer = new StringReplacer();
        var result = replacer.Replace(sourceCode, replacements);

        return result;
    }

    protected abstract SyntaxTree ParseText(string text);

    protected abstract IReadOnlyList<StringReplacement> GetReplacements(
        string sourceCode,
        SyntaxTree tree);

    protected static string GetBaseIndentation(
        SyntaxTriviaList leadingTrivia,
        RawSyntaxKind whitespaceKind)
    {
        var lastLeading = leadingTrivia.Last();
        if (lastLeading.RawSyntaxKind == whitespaceKind)
        {
            return lastLeading.ToString();
        }
        return string.Empty;
    }

    protected abstract class BaseXmlDocTreeFormatter<TXmlDocTree, TXmlSyntaxFacts>(
        XmlDocFormatOptions options,
        string baseIndentation,
        string lineEnding)
        where TXmlDocTree : BaseXmlDocTree<TXmlSyntaxFacts>
        where TXmlSyntaxFacts : BaseXmlSyntaxFacts, ISharedInstance
    {
        protected abstract TXmlSyntaxFacts SyntaxFacts { get; }

        public string FormatXmlDocs(TXmlDocTree tree)
        {
            var lines = ProduceLines(tree);
            return FormatLines(lines);
        }

        private IReadOnlyList<string> ProduceLines(TXmlDocTree tree)
        {
            var lineBuilder = new LineBuilder(options, SyntaxFacts, baseIndentation);

            foreach (var child in tree.Root.Children)
            {
                ProcessNode(child, lineBuilder);
            }

            lineBuilder.Flush();
            return lineBuilder.FlushedLines;
        }

        private void ProcessNode(
            IXmlNodeWrapper? node,
            LineBuilder lineBuilder)
        {
            if (node is null)
            {
                return;
            }

            switch (node)
            {
                case IXmlElementWrapper element:
                    ProcessElement(element, lineBuilder);
                    break;

                case IXmlEmptyElementWrapper emptyElement:
                    BaseXmlDocTreeFormatter<TXmlDocTree, TXmlSyntaxFacts>.ProcessEmptyElement(emptyElement, lineBuilder);
                    break;

                case IXmlTextWrapper text:
                    ProcessText(text, lineBuilder);
                    break;

                case IXmlCommentWrapper comment:
                    ProcessComment(comment, lineBuilder);
                    break;

                case IXmlCDataSectionWrapper cdata:
                    BaseXmlDocTreeFormatter<TXmlDocTree, TXmlSyntaxFacts>.ProcessCDataSection(cdata, lineBuilder);
                    break;

                case IXmlProcessingInstructionWrapper pi:
                    BaseXmlDocTreeFormatter<TXmlDocTree, TXmlSyntaxFacts>.ProcessProcessingInstruction(pi, lineBuilder);
                    break;
            }
        }

        private void ProcessElement(
            IXmlElementWrapper element,
            LineBuilder lineBuilder)
        {
            var tagName = element.StartTag.Name.FullName;
            var isInlineElement = IsInlineElementType(tagName);
            var increaseIndentation = ShouldIncreaseIndentation(tagName);

            if (isInlineElement)
            {
                AppendInlineElement(element, lineBuilder);
                return;
            }

            AppendBlockElement();

            void AppendBlockElement()
            {
                lineBuilder.CommitLine();
                lineBuilder.AppendWord(FormatStartTag(element.StartTag));
                lineBuilder.CommitLine();

                if (increaseIndentation)
                {
                    lineBuilder.Indent();
                }

                foreach (var child in element.Content)
                {
                    ProcessNode(child, lineBuilder);
                }

                lineBuilder.CommitLine();

                if (increaseIndentation)
                {
                    lineBuilder.Unindent();
                }

                lineBuilder.AppendWordPart(FormatEndTag(element.EndTag));
                lineBuilder.CommitLine();
            }
        }

        private void AppendInlineElement(
            IXmlElementWrapper element,
            LineBuilder lineBuilder)
        {
            lineBuilder.AppendWordPart(FormatStartTag(element.StartTag));

            foreach (var child in element.Content)
            {
                ProcessNode(child, lineBuilder);
            }

            lineBuilder.AppendWordPart(FormatEndTag(element.EndTag));
        }

        private static void ProcessEmptyElement(
            IXmlEmptyElementWrapper emptyElement,
            LineBuilder lineBuilder)
        {
            var tagName = emptyElement.Name.FullName;
            var elementText = FormatEmptyElement(emptyElement);

            if (IsLineBreakElement(tagName))
            {
                lineBuilder.CommitLine();
                lineBuilder.AppendWord(elementText);
                lineBuilder.CommitLine();
            }
            else
            {
                lineBuilder.AppendWordPart(elementText);
            }
        }

        private void ProcessText(
            IXmlTextWrapper text,
            LineBuilder lineBuilder)
        {
            foreach (var token in text.TextTokens)
            {
                var tokenText = token.Text;
                var rawKind = token.RawSyntaxKind;
                var facts = SyntaxFacts;

                if (rawKind == facts.XmlTextLiteralNewLineToken)
                {
                    lineBuilder.ConsumeWord();
                    continue;
                }

                if (rawKind == facts.XmlEntityLiteralToken)
                {
                    lineBuilder.AppendWordPart(tokenText);
                    continue;
                }

                if (rawKind == facts.XmlTextLiteralToken)
                {
                    var words = ExtractWords(tokenText);

                    if (tokenText is [var first, ..] && first.IsWhiteSpace())
                    {
                        lineBuilder.ConsumeWord();
                    }

                    var isFirst = true;
                    foreach (var word in words)
                    {
                        if (!isFirst)
                        {
                            lineBuilder.ConsumeWord();
                        }

                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            isFirst = false;
                            lineBuilder.AppendWordPart(word);
                        }
                    }

                    if (tokenText is [.., var last] && last.IsWhiteSpace())
                    {
                        lineBuilder.ConsumeWord();
                    }
                }
            }
        }

        private void ProcessComment(
            IXmlCommentWrapper comment,
            LineBuilder lineBuilder)
        {
            lineBuilder.AppendWordPart(comment.StartCommentToken.Text);
            lineBuilder.AppendWordPart(" ");

            var commentTokens = comment.TextTokens;
            foreach (var token in commentTokens)
            {
                if (token.RawKind == SyntaxFacts.XmlTextLiteralNewLineToken)
                {
                    lineBuilder.CommitLine();
                    continue;
                }

                lineBuilder.AppendWordPart(token.Text.Trim());
            }

            lineBuilder.AppendWordPart(" ");
            lineBuilder.AppendWordPart(comment.EndCommentToken.Text);
        }

        private static void ProcessCDataSection(
            IXmlCDataSectionWrapper cdata,
            LineBuilder lineBuilder)
        {
            lineBuilder.CommitLine();
            lineBuilder.AppendWord(cdata.Node.ToFullString().Trim());
        }

        private static void ProcessProcessingInstruction(
            IXmlProcessingInstructionWrapper pi,
            LineBuilder lineBuilder)
        {
            lineBuilder.CommitLine();
            lineBuilder.AppendWord(pi.Node.ToFullString().Trim());
        }

        private static string FormatStartTag(IXmlElementStartTagWrapper startTag)
        {
            var builder = new StringBuilder();
            builder.Append('<');
            builder.Append(startTag.Name.FullName);

            foreach (var attr in startTag.Attributes)
            {
                builder.Append(' ');
                builder.Append(FormatAttribute(attr));
            }

            builder.Append('>');
            return builder.ToString();
        }

        private static string FormatEndTag(IXmlElementEndTagWrapper endTag)
        {
            return $"</{endTag.Name.FullName}>";
        }

        private static string FormatEmptyElement(IXmlEmptyElementWrapper element)
        {
            var builder = new StringBuilder();
            builder.Append('<');
            builder.Append(element.Name.FullName);

            foreach (var attr in element.Attributes)
            {
                builder.Append(' ');
                builder.Append(FormatAttribute(attr));
            }

            builder.Append("/>");
            return builder.ToString();
        }

        private static string FormatAttribute(IXmlAttributeWrapper attr)
        {
            var name = attr.Name.FullName;
            var value = attr switch
            {
                IXmlTextAttributeWrapper textAttr => string.Concat(textAttr.TextTokens.Select(t => t.Text)),
                IXmlCrefAttributeWrapper crefAttr => FormatCref(crefAttr),
                IXmlNameAttributeWrapper nameAttr => nameAttr.Identifier.ToString(),
                _ => string.Empty,
            };

            return $"{name}=\"{value}\"";

            static string FormatCref(IXmlCrefAttributeWrapper cref)
            {
                // TODO: Introduce cref formatting logic
                return cref.Cref.ToString();
            }
        }

        private static IEnumerable<string> ExtractWords(string text)
        {
            var words = new List<string>();
            var currentWord = new StringBuilder();

            foreach (var c in text)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (currentWord.Length > 0)
                    {
                        words.Add(currentWord.ToString());
                        currentWord.Clear();
                    }
                }
                else
                {
                    currentWord.Append(c);
                }
            }

            if (currentWord.Length > 0)
            {
                words.Add(currentWord.ToString());
            }

            return words;
        }

        private static bool IsInlineElementType(string tagName)
        {
            return tagName
                is "see" or "seealso"
                or "paramref" or "typeparamref"
                or "c" or "b" or "i" or "u" or "a"
                ;
        }

        private static bool IsLineBreakElement(string tagName)
        {
            return tagName is "br";
        }

        private static bool ShouldIncreaseIndentation(string tagName)
        {
            return tagName
                is "list" or "code" or "pre"
                ;
        }

        private static RepeatedChar GetIndent(int level)
        {
            return new(' ', level * 4);
        }

        private class LineBuilder(
            XmlDocFormatOptions options,
            TXmlSyntaxFacts facts,
            string baseIndentation)
        {
            private readonly StringBuilder _currentWord = new();
            private readonly StringBuilder _currentLine = new();
            private readonly List<string> _pendingLines = [];
            private readonly List<string> _flushedLines = [];

            private int _currentIndentLevel;

            public IReadOnlyList<string> FlushedLines => _flushedLines;

            public string CommentPrefix => facts.SingleLineDocumentationPrefix;

            public void Indent()
            {
                _currentIndentLevel++;
            }

            public void Unindent()
            {
                if (_currentIndentLevel > 0)
                {
                    _currentIndentLevel--;
                }
            }

            public void AddRawLine(string rawLine)
            {
                Flush();
                _flushedLines.Add(rawLine);
            }

            public void CommitLine()
            {
                ConsumeWord();

                if (_currentLine.Length == 0)
                {
                    return;
                }

                var commentPrefix = CommentPrefix;
                const string prefixSpace = " ";
                var indent = GetIndent(_currentIndentLevel);

                _pendingLines.Add($"{commentPrefix}{prefixSpace}{indent}{_currentLine}");
                _currentLine.Clear();
            }

            public void AppendWordPart(string wordPart)
            {
                _currentWord.Append(wordPart);
            }

            public void AppendWord(string word)
            {
                ConsumeWord();

                var commentPrefix = CommentPrefix;
                const string prefixSpace = " ";

                var indent = GetIndent(_currentIndentLevel);
                var prefixLength = commentPrefix.Length + prefixSpace.Length + indent.Count;
                var currentLength = baseIndentation.Length + prefixLength + _currentLine.Length;

                var spaceNeeded = _currentLine.Length > 0;
                var spaceLength = spaceNeeded ? 1 : 0;
                var wouldExceed = currentLength + spaceLength + word.Length > options.MaxLineLength;

                if (spaceNeeded && wouldExceed)
                {
                    CommitLine();
                    spaceNeeded = false;
                }

                if (spaceNeeded)
                {
                    _currentLine.Append(' ');
                }

                _currentLine.Append(word);
            }

            public void Flush()
            {
                CommitLine();

                foreach (var line in _pendingLines)
                {
                    _flushedLines.Add(line);
                }
                _pendingLines.Clear();
            }

            public void ConsumeWord()
            {
                if (_currentWord.Length == 0)
                {
                    return;
                }

                var word = _currentWord.ToString();
                _currentWord.Clear();
                AppendWord(word);
            }
        }

        private string FormatLines(IReadOnlyList<string> lines)
        {
            var estimatedContentLength = EstimateFinalCapacity(lines.Count, options.MaxLineLength);
            var builder = new StringBuilder(estimatedContentLength);

            var isFirst = true;
            foreach (var line in lines)
            {
                if (!isFirst)
                {
                    builder.Append(lineEnding);
                }
                builder.Append(baseIndentation);
                builder.Append(line);
                isFirst = false;
            }

            if (builder.Length > estimatedContentLength)
            {
                // This warning is placed here to help diagnose possible
                // underestimation in the overflow ratio, it's always best
                // to adjust and overshoot memory allocation than force
                // resizing and re-allocations
                Debug.WriteLine(
                    $"Warning: Exceeded estimated capacity for XML docs in formatting: {builder.Length} / {estimatedContentLength}");
            }

            return builder.ToString();

            static int EstimateFinalCapacity(int lineCount, int maxLineLength)
            {
                const double overflowRatio = 1.2;
                return (int)(lineCount * maxLineLength * overflowRatio);
            }
        }
    }
}
