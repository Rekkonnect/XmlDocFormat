using System.Diagnostics.CodeAnalysis;
using XmlDocFormat.Core;

namespace XmlDocFormat.Tests.Shared;

// Currently, vb-test is not supported to use in C#
// Tracking issue here: https://github.com/dotnet/roslyn/issues/82352
public sealed record VisualBasicFormatTestCase(
    [StringSyntax("vb-test")]
    string Source,
    [StringSyntax("vb-test")]
    string Formatted,
    XmlDocFormatOptions FormatOptions)
    : BaseFormatTestCase(Source, Formatted, FormatOptions);
