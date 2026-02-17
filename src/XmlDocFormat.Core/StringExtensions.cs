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
    }
}
