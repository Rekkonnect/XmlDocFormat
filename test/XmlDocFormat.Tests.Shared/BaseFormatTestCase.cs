using XmlDocFormat.Core;

namespace XmlDocFormat.Tests.Shared;

public abstract record BaseFormatTestCase(
    string Source,
    string Formatted,
    XmlDocFormatOptions FormatOptions);
