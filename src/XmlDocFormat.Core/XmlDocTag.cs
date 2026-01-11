using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace XmlDocFormat.Core;

/// <summary>
/// Represents an XML documentation tag.
/// </summary>
/// <param name="tagName">The name of the tag.</param>
/// <param name="content">
/// The syntax content of the tag. This is only stored for reference.
/// The content does not include the tag tokens themselves.
/// </param>
/// <remarks>
/// Such tags include summary, remarks, param, typeparam, returns, etc.,
/// but not content tags like see or paramref.
/// </remarks>
public abstract class XmlDocTag(
    string tagName,
    ImmutableArray<SyntaxToken> content)
{
    public string Name { get; } = tagName;

    public ImmutableArray<SyntaxToken> Content { get; } = content;
}

public sealed class BasicXmlDocTag(
    string tagName,
    ImmutableArray<SyntaxToken> content)
    : XmlDocTag(tagName, content)
{
}

public sealed class ParamXmlDocTag(
    string tagName,
    string paramName,
    ImmutableArray<SyntaxToken> content)
    : XmlDocTag(tagName, content)
{
    public string ParameterName { get; } = paramName;
}

public sealed class TypeParamRefXmlDocTag(
    string tagName,
    string paramName,
    ImmutableArray<SyntaxToken> content)
    : XmlDocTag(tagName, content)
{
    public string ParameterName { get; } = paramName;
}
