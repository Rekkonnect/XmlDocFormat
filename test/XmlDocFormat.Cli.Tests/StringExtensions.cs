using Garyon.Extensions;

namespace XmlDocFormat.Cli.Tests;

public static class StringExtensions
{
    extension(string str)
    {
        public string WithTrailingNewLine()
        {
            return str.EnsureEndsWith("\r\n");
        }
    }
}
