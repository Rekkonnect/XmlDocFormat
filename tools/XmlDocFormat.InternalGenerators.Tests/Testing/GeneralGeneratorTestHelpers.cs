using Garyon.Objects.Strings;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using XmlDocFormat.Core;
using XmlDocFormat.Tests.Shared;

namespace XmlDocFormat.InternalGenerators.Tests.Testing;

public static class GeneralGeneratorTestHelpers
{
    public static SourceText Utf8CSharpSource(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)]
        string source)
    {
        var normalizedSource = source.NormalizeNewLines(WhitespaceFacts.CrLf);
        return SourceText.From(normalizedSource, Encoding.UTF8);
    }
}
