namespace XmlDocFormat.Core;

public static class SourceFileExtensionFacts
{
    public const string CSharp = ".cs";
    public const string VisualBasic = ".vb";

    public const string AnyCSharpFile = $"*{CSharp}";
    public const string AnyVisualBasicFile = $"*{VisualBasic}";

    public static IReadOnlyList<string> AnyFileExtensions
        => [AnyCSharpFile, AnyVisualBasicFile];

    public static bool IsNetLanguageFileExtension(this string extension)
    {
        return extension is CSharp or VisualBasic;
    }
}
