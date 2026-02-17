using System.Security;

namespace XmlDocFormat.Core;

public sealed class HybridXmlDocFormatter(XmlDocFormatOptions options)
{
    private readonly CSharpXmlDocFormatter _csFormatter = new(options);
    private readonly VisualBasicXmlDocFormatter _vbFormatter = new(options);

    public string FormatFile(string fileName, string source)
    {
        var extension = Path.GetExtension(fileName);
        var formatter = FormatterForFile(extension);
        if (formatter is null)
        {
            return source;
        }

        return formatter.Format(source);
    }

    private BaseXmlDocFormatter? FormatterForFile(string extension)
    {
        return extension switch
        {
            SourceFileExtensionFacts.CSharp => _csFormatter,
            SourceFileExtensionFacts.VisualBasic => _vbFormatter,
            _ => null,
        };
    }
}
