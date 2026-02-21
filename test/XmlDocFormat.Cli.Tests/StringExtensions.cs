using Garyon.Extensions;

namespace XmlDocFormat.Cli.Tests;

public static class StringExtensions
{
    extension(string str)
    {
        public string WithTrailingEnvironmentNewLine()
        {
            return str.WithTrailingNewLine(Environment.NewLine);
        }

        public string WithTrailingNewLine(string newLine)
        {
            return str.EnsureEndsWith(newLine);
        }
    }
}
