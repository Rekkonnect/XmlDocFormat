using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using XmlDocFormat.InternalGenerators.Core;

namespace XmlDocFormat.Core.XmlNodes;

[ConstructFromImplicitCast]
public sealed partial class VisualBasicXmlNameWrapper(XmlNameSyntax name)
    : VisualBasicBaseXmlNodeWrapper<XmlNameSyntax>(name), IXmlNameWrapper
{
    IXmlPrefixWrapper? IXmlNameWrapper.Prefix => Prefix;
    public VisualBasicXmlPrefixWrapper? Prefix => Node.Prefix;

    public SyntaxToken LocalName => Node.LocalName;
}

[ConstructFromImplicitCast]
public sealed partial class VisualBasicXmlPrefixWrapper(XmlPrefixSyntax prefix)
    : VisualBasicBaseXmlNodeWrapper<XmlPrefixSyntax>(prefix), IXmlPrefixWrapper
{
    public SyntaxToken Prefix => Node.Name;
}

public abstract partial class VisualBasicBaseXmlAttributeWrapper<TNode>(TNode attribute)
    : VisualBasicBaseXmlNodeWrapper<TNode>(attribute)
    where TNode : BaseXmlAttributeSyntax
{
}

[ConstructFromImplicitCast]
public sealed partial class VisualBasicXmlNameAttributeWrapper(XmlNameAttributeSyntax name)
    : VisualBasicBaseXmlAttributeWrapper<XmlNameAttributeSyntax>(name), IXmlNameAttributeWrapper
{
    public SyntaxNode Identifier => Node.Reference;

    public VisualBasicXmlNameWrapper Name => Node.Name;
    IXmlNameWrapper IXmlAttributeWrapper.Name => Name;

    public SyntaxToken EqualsToken => Node.EqualsToken;
    public SyntaxToken StartQuoteToken => Node.StartQuoteToken;
    public SyntaxToken EndQuoteToken => Node.EndQuoteToken;
}

[ConstructFromImplicitCast]
public sealed partial class VisualBasicXmlCrefAttributeWrapper(XmlCrefAttributeSyntax cref)
    : VisualBasicBaseXmlAttributeWrapper<XmlCrefAttributeSyntax>(cref), IXmlCrefAttributeWrapper
{
    public SyntaxNode Cref => Node.Reference;

    public VisualBasicXmlNameWrapper Name => Node.Name;
    IXmlNameWrapper IXmlAttributeWrapper.Name => Name;

    public SyntaxToken EqualsToken => Node.EqualsToken;
    public SyntaxToken StartQuoteToken => Node.StartQuoteToken;
    public SyntaxToken EndQuoteToken => Node.EndQuoteToken;
}

[ConstructFromImplicitCast]
public sealed partial class VisualBasicXmlTextAttributeWrapper(XmlAttributeSyntax text)
    : VisualBasicBaseXmlAttributeWrapper<XmlAttributeSyntax>(text), IXmlTextAttributeWrapper
{
    public SyntaxTokenList TextTokens => ValueString?.TextTokens ?? [];

    public VisualBasicXmlNameWrapper? Name => Node.Name as XmlNameSyntax;
    IXmlNameWrapper IXmlAttributeWrapper.Name => Name!;

    public SyntaxToken EqualsToken => Node.EqualsToken;
    public SyntaxToken StartQuoteToken => ValueString?.StartQuoteToken ?? default;
    public SyntaxToken EndQuoteToken => ValueString?.EndQuoteToken ?? default;

    private XmlStringSyntax? ValueString => Node.Value as XmlStringSyntax;
}

[ConstructFromImplicitCast]
public sealed partial class VisualBasicXmlElementWrapper(XmlElementSyntax element)
    : VisualBasicBaseXmlNodeWrapper<XmlElementSyntax>(element), IXmlElementWrapper
{
    IXmlElementStartTagWrapper IXmlElementWrapper.StartTag => StartTag;
    public VisualBasicXmlElementStartTagWrapper StartTag => Node.StartTag;

    IXmlElementEndTagWrapper IXmlElementWrapper.EndTag => EndTag;
    public VisualBasicXmlElementEndTagWrapper EndTag => Node.EndTag;

    public IReadOnlyList<IXmlNodeWrapper> Content => NodeWrappers(Node.Content);
}

[ConstructFromImplicitCast]
public sealed partial class VisualBasicXmlElementStartTagWrapper(XmlElementStartTagSyntax start)
    : VisualBasicBaseXmlNodeWrapper<XmlElementStartTagSyntax>(start), IXmlElementStartTagWrapper
{
    public VisualBasicXmlNameWrapper? Name => Node.Name as XmlNameSyntax;
    IXmlNameWrapper IXmlElementStartTagWrapper.Name => Name!;

    public SyntaxToken LessThanToken => Node.LessThanToken;
    public SyntaxToken GreaterThanToken => Node.GreaterThanToken;
    public IReadOnlyList<IXmlAttributeWrapper> Attributes
        => NodeWrappers<XmlNodeSyntax, IXmlAttributeWrapper>(Node.Attributes);
}

[ConstructFromImplicitCast]
public sealed partial class VisualBasicXmlElementEndTagWrapper(XmlElementEndTagSyntax end)
    : VisualBasicBaseXmlNodeWrapper<XmlElementEndTagSyntax>(end), IXmlElementEndTagWrapper
{
    public VisualBasicXmlNameWrapper Name => Node.Name;
    IXmlNameWrapper IXmlElementEndTagWrapper.Name => Name;

    public SyntaxToken LessThanSlashToken => Node.LessThanSlashToken;
    public SyntaxToken GreaterThanToken => Node.GreaterThanToken;
}

[ConstructFromImplicitCast]
public sealed partial class VisualBasicXmlEmptyElementWrapper(XmlEmptyElementSyntax element)
    : VisualBasicBaseXmlNodeWrapper<XmlEmptyElementSyntax>(element), IXmlEmptyElementWrapper
{
    public VisualBasicXmlNameWrapper? Name => Node.Name as XmlNameSyntax;
    IXmlNameWrapper IXmlEmptyElementWrapper.Name => Name!;

    public SyntaxToken LessThanToken => Node.LessThanToken;
    public SyntaxToken SlashGreaterThanToken => Node.SlashGreaterThanToken;
    public IReadOnlyList<IXmlAttributeWrapper> Attributes
        => NodeWrappers<XmlNodeSyntax, IXmlAttributeWrapper>(Node.Attributes);
}

[ConstructFromImplicitCast]
public sealed partial class VisualBasicXmlCommentWrapper(XmlCommentSyntax element)
    : VisualBasicBaseXmlNodeWrapper<XmlCommentSyntax>(element), IXmlCommentWrapper
{
    // InterestingNamesDotDotDotExclamationToken
    public SyntaxToken StartCommentToken => Node.LessThanExclamationMinusMinusToken;
    public SyntaxToken EndCommentToken => Node.MinusMinusGreaterThanToken;
    public SyntaxTokenList TextTokens => Node.TextTokens;
}

[ConstructFromImplicitCast]
public sealed partial class VisualBasicXmlCDataSectionWrapper(XmlCDataSectionSyntax element)
    : VisualBasicBaseXmlNodeWrapper<XmlCDataSectionSyntax>(element), IXmlCDataSectionWrapper
{
    public SyntaxToken StartCDataToken => Node.BeginCDataToken;
    public SyntaxToken EndCDataToken => Node.EndCDataToken;
    public SyntaxTokenList TextTokens => Node.TextTokens;
}

[ConstructFromImplicitCast]
public sealed partial class VisualBasicXmlProcessingInstructionWrapper(XmlProcessingInstructionSyntax element)
    : VisualBasicBaseXmlNodeWrapper<XmlProcessingInstructionSyntax>(element), IXmlProcessingInstructionWrapper
{
    public IXmlNameWrapper Name => new VisualBasicXmlSimpleNameTokenWrapper(Node.Name);

    public SyntaxToken StartProcessingInstructionToken => Node.LessThanQuestionToken;
    public SyntaxToken EndProcessingInstructionToken => Node.QuestionGreaterThanToken;
    public SyntaxTokenList TextTokens => Node.TextTokens;
}

[ConstructFromImplicitCast]
public sealed partial class VisualBasicXmlTextWrapper(XmlTextSyntax text)
    : VisualBasicBaseXmlNodeWrapper<XmlTextSyntax>(text), IXmlTextWrapper
{
    public SyntaxTokenList TextTokens => Node.TextTokens;
}

public sealed class VisualBasicXmlSimpleNameTokenWrapper(SyntaxToken token)
    : IXmlNameWrapper
{
    public IXmlPrefixWrapper? Prefix => null;

    public SyntaxToken LocalName => token;

    public SyntaxNode Node => null!;
}
