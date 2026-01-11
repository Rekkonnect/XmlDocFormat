using Garyon.Objects.Strings;

namespace XmlDocFormat.Core;

public static class WhitespaceHelpers
{
    public static WhitespaceKind GetWhitespaceKind(this char c)
    {
        return c switch
        {
            WhitespaceFacts.CarriageReturn => WhitespaceKind.NewLine,
            WhitespaceFacts.LineFeed => WhitespaceKind.NewLine,
            WhitespaceFacts.Space => WhitespaceKind.Space,
            WhitespaceFacts.Tab => WhitespaceKind.Space,
            _ => WhitespaceKind.None,
        };
    }

    public static IEnumerable<string> TokenizeWhitespace(
        this string @string)
    {
        var previousKind = WhitespaceKind.None;
        var currentStart = 0;
        for (int i = 0; i < @string.Length; i++)
        {
            var currentKind = @string[i].GetWhitespaceKind();

            if (currentKind != previousKind)
            {
                if (previousKind is not WhitespaceKind.None)
                {
                    yield return @string[currentStart..i].ToString();
                }
                currentStart = i;
                previousKind = currentKind;
            }
        }

        if (previousKind is not WhitespaceKind.None)
        {
            yield return @string[currentStart..@string.Length].ToString();
        }
    }
}
