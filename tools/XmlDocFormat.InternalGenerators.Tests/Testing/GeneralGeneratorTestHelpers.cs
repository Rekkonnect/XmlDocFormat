using Microsoft.CodeAnalysis.Text;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using XmlDocFormat.Tests.Shared;

namespace XmlDocFormat.InternalGenerators.Tests.Testing;

public static class GeneralGeneratorTestHelpers
{
    public static SourceText Utf8CSharpSource(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)]
        string source)
    {
        return SourceText.From(source, Encoding.UTF8);
    }
}
