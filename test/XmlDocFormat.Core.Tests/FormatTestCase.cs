using System.Diagnostics.CodeAnalysis;

namespace XmlDocFormat.Core.Tests;

public sealed record FormatTestCase(
    [StringSyntax("C#-test")]
    string Source,
    [StringSyntax("C#-test")]
    string Formatted,
    XmlDocFormatOptions FormatOptions);
