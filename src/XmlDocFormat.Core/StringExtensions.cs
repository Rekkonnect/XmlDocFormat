using System.Text;

namespace XmlDocFormat.Core;

public static class StringExtensions
{
    extension(string text)
    {
        public string? DetectLineEnding()
        {
            if (text.Contains("\r\n"))
            {
                return "\r\n";
            }

            if (text.Contains('\n'))
            {
                return "\n";
            }

            return null;
        }

        public string GetAppliedLineEnding(string overridden)
        {
            return text.DetectLineEnding() ?? overridden;
        }

        public string NormalizeEnvironmentNewLines()
        {
            return text.NormalizeNewLines(Environment.NewLine);
        }

        public string NormalizeNewLines(string newLine)
        {
            var builder = new StringBuilder((int)(text.Length * 1.2));

            foreach (var token in text.EnumerateLineTokens())
            {
                if (token.Kind is LineToken.TokenKind.NewLine)
                {
                    builder.Append(newLine);
                    continue;
                }

                builder.Append(token.Span);
            }

            return builder.ToString();
        }

        public StringLineTokenEnumerator EnumerateLineTokens()
        {
            return new(text.AsSpan());
        }
    }
}
