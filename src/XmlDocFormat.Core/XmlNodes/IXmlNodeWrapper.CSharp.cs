using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using XmlDocFormat.InternalGenerators.Core;

namespace XmlDocFormat.Core.XmlNodes;

[ConstructFromImplicitCast]
public sealed partial class CSharpXmlNameWrapper(XmlNameSyntax name)
    : CSharpBaseXmlNodeWrapper<XmlNameSyntax>(name), IXmlNameWrapper
{
    IXmlPrefixWrapper? IXmlNameWrapper.Prefix => Prefix;
    public CSharpXmlPrefixWrapper? Prefix => Node.Prefix;

    public SyntaxToken LocalName => Node.LocalName;
}

[ConstructFromImplicitCast]
public sealed partial class CSharpXmlPrefixWrapper(XmlPrefixSyntax prefix)
    : CSharpBaseXmlNodeWrapper<XmlPrefixSyntax>(prefix), IXmlPrefixWrapper
{
    public SyntaxToken Prefix => Node.Prefix;
}

public abstract partial class CSharpBaseXmlAttributeWrapper<TNode>(TNode attribute)
    : CSharpBaseXmlNodeWrapper<TNode>(attribute), IXmlAttributeWrapper
    where TNode : XmlAttributeSyntax
{
    public CSharpXmlNameWrapper Name => Node.Name;
    IXmlNameWrapper IXmlAttributeWrapper.Name => Name;

    public SyntaxToken EqualsToken => Node.EqualsToken;
    public SyntaxToken StartQuoteToken => Node.StartQuoteToken;
    public SyntaxToken EndQuoteToken => Node.EndQuoteToken;
}

[ConstructFromImplicitCast]
public sealed partial class CSharpXmlNameAttributeWrapper(XmlNameAttributeSyntax name)
    : CSharpBaseXmlAttributeWrapper<XmlNameAttributeSyntax>(name), IXmlNameAttributeWrapper
{
    public SyntaxNode Identifier => Node.Identifier;
}

[ConstructFromImplicitCast]
public sealed partial class CSharpXmlCrefAttributeWrapper(XmlCrefAttributeSyntax cref)
    : CSharpBaseXmlAttributeWrapper<XmlCrefAttributeSyntax>(cref), IXmlCrefAttributeWrapper
{
    public SyntaxNode Cref => Node.Cref;
}

[ConstructFromImplicitCast]
public sealed partial class CSharpXmlTextAttributeWrapper(XmlTextAttributeSyntax text)
    : CSharpBaseXmlAttributeWrapper<XmlTextAttributeSyntax>(text), IXmlTextAttributeWrapper
{
    public SyntaxTokenList TextTokens => Node.TextTokens;
}

[ConstructFromImplicitCast]
public sealed partial class CSharpXmlElementWrapper(XmlElementSyntax element)
    : CSharpBaseXmlNodeWrapper<XmlElementSyntax>(element), IXmlElementWrapper
{
    IXmlElementStartTagWrapper IXmlElementWrapper.StartTag => StartTag;
    public CSharpXmlElementStartTagWrapper StartTag => Node.StartTag;

    IXmlElementEndTagWrapper IXmlElementWrapper.EndTag => EndTag;
    public CSharpXmlElementEndTagWrapper EndTag => Node.EndTag;

    public IReadOnlyList<IXmlNodeWrapper> Content => NodeWrappers(Node.Content);
}

[ConstructFromImplicitCast]
public sealed partial class CSharpXmlElementStartTagWrapper(XmlElementStartTagSyntax start)
    : CSharpBaseXmlNodeWrapper<XmlElementStartTagSyntax>(start), IXmlElementStartTagWrapper
{
    public CSharpXmlNameWrapper Name => Node.Name;
    IXmlNameWrapper IXmlElementStartTagWrapper.Name => Name;

    public SyntaxToken LessThanToken => Node.LessThanToken;
    public SyntaxToken GreaterThanToken => Node.GreaterThanToken;
    public IReadOnlyList<IXmlAttributeWrapper> Attributes
        => NodeWrappers<XmlAttributeSyntax, IXmlAttributeWrapper>(Node.Attributes);
}

[ConstructFromImplicitCast]
public sealed partial class CSharpXmlElementEndTagWrapper(XmlElementEndTagSyntax end)
    : CSharpBaseXmlNodeWrapper<XmlElementEndTagSyntax>(end), IXmlElementEndTagWrapper
{
    public CSharpXmlNameWrapper Name => Node.Name;
    IXmlNameWrapper IXmlElementEndTagWrapper.Name => Name;

    public SyntaxToken LessThanSlashToken => Node.LessThanSlashToken;
    public SyntaxToken GreaterThanToken => Node.GreaterThanToken;
}

[ConstructFromImplicitCast]
public sealed partial class CSharpXmlEmptyElementWrapper(XmlEmptyElementSyntax element)
    : CSharpBaseXmlNodeWrapper<XmlEmptyElementSyntax>(element), IXmlEmptyElementWrapper
{
    public CSharpXmlNameWrapper Name => Node.Name;
    IXmlNameWrapper IXmlEmptyElementWrapper.Name => Name;

    public SyntaxToken LessThanToken => Node.LessThanToken;
    public SyntaxToken SlashGreaterThanToken => Node.SlashGreaterThanToken;
    public IReadOnlyList<IXmlAttributeWrapper> Attributes
        => NodeWrappers<XmlAttributeSyntax, IXmlAttributeWrapper>(Node.Attributes);
}

[ConstructFromImplicitCast]
public sealed partial class CSharpXmlCommentWrapper(XmlCommentSyntax element)
    : CSharpBaseXmlNodeWrapper<XmlCommentSyntax>(element), IXmlCommentWrapper
{
    // InterestingNamesDotDotDotExclamationToken
    public SyntaxToken StartCommentToken => Node.LessThanExclamationMinusMinusToken;
    public SyntaxToken EndCommentToken => Node.MinusMinusGreaterThanToken;
    public SyntaxTokenList TextTokens => Node.TextTokens;
}

[ConstructFromImplicitCast]
public sealed partial class CSharpXmlCDataSectionWrapper(XmlCDataSectionSyntax element)
    : CSharpBaseXmlNodeWrapper<XmlCDataSectionSyntax>(element), IXmlCDataSectionWrapper
{
    public SyntaxToken StartCDataToken => Node.StartCDataToken;
    public SyntaxToken EndCDataToken => Node.EndCDataToken;
    public SyntaxTokenList TextTokens => Node.TextTokens;
}

[ConstructFromImplicitCast]
public sealed partial class CSharpXmlProcessingInstructionWrapper(XmlProcessingInstructionSyntax element)
    : CSharpBaseXmlNodeWrapper<XmlProcessingInstructionSyntax>(element), IXmlProcessingInstructionWrapper
{
    public CSharpXmlNameWrapper Name => Node.Name;
    IXmlNameWrapper IXmlProcessingInstructionWrapper.Name => Name;

    public SyntaxToken StartProcessingInstructionToken => Node.StartProcessingInstructionToken;
    public SyntaxToken EndProcessingInstructionToken => Node.EndProcessingInstructionToken;
    public SyntaxTokenList TextTokens => Node.TextTokens;
}

[ConstructFromImplicitCast]
public sealed partial class CSharpXmlTextWrapper(XmlTextSyntax text)
    : CSharpBaseXmlNodeWrapper<XmlTextSyntax>(text), IXmlTextWrapper
{
    public SyntaxTokenList TextTokens => Node.TextTokens;
}
