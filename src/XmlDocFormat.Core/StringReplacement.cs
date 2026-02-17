using Microsoft.CodeAnalysis.Text;

namespace XmlDocFormat.Core;

[GaryonUtility]
public readonly record struct StringReplacement(int Start, int Length, string NewText)
{
    public int SourceEnd => Start + Length;
    public int NewLength => NewText.Length;
    public int NewEnd => Start + NewLength;
    public int LengthDifference => NewLength - Length;

    public StringReplacement(TextSpan span, string newText)
        : this(span.Start, span.Length, newText)
    {
    }

    public string Apply(string source)
    {
        return source[..Start] + NewText + source[(Start + Length)..];
    }
}
