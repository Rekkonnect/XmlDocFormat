using System.Diagnostics.CodeAnalysis;
using XmlDocFormat.Core;

namespace XmlDocFormat.Tests.Shared;

public sealed record CSharpFormatTestCase(
    [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)]
    string Source,
    [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)]
    string Formatted,
    XmlDocFormatOptions FormatOptions)
    : BaseFormatTestCase(Source, Formatted, FormatOptions);
