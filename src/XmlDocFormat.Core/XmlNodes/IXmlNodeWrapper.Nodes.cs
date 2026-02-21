using Microsoft.CodeAnalysis;

namespace XmlDocFormat.Core.XmlNodes;

public interface IXmlNameWrapper : IXmlNodeWrapper
{
    public IXmlPrefixWrapper? Prefix { get; }
    public SyntaxToken LocalName { get; }

    public sealed string FullName
    {
        get
        {
            if (Prefix is null)
                return LocalName.ValueText;

            return $"{Prefix.Prefix.ValueText}:{LocalName.ValueText}";
        }
    }
}

public interface IXmlPrefixWrapper : IXmlNodeWrapper
{
    public SyntaxToken Prefix { get; }
}

public interface IXmlAttributeWrapper : IXmlNodeWrapper
{
    public IXmlNameWrapper Name { get; }

    public SyntaxToken EqualsToken { get; }
    public SyntaxToken StartQuoteToken { get; }
    public SyntaxToken EndQuoteToken { get; }
}

public interface IXmlNameAttributeWrapper : IXmlAttributeWrapper
{
    public SyntaxNode Identifier { get; }
}

public interface IXmlCrefAttributeWrapper : IXmlAttributeWrapper
{
    public SyntaxNode Cref { get; }
}

public interface IXmlTextAttributeWrapper : IXmlAttributeWrapper
{
    public SyntaxTokenList TextTokens { get; }
}

public interface IXmlElementWrapper : IXmlNodeWrapper
{
    public IXmlElementStartTagWrapper StartTag { get; }
    public IXmlElementEndTagWrapper EndTag { get; }

    public IReadOnlyList<IXmlNodeWrapper> Content { get; }
}

public interface IXmlElementStartTagWrapper : IXmlNodeWrapper
{
    public IXmlNameWrapper Name { get; }
    public SyntaxToken LessThanToken { get; }
    public SyntaxToken GreaterThanToken { get; }

    public IReadOnlyList<IXmlAttributeWrapper> Attributes { get; }
}

public interface IXmlElementEndTagWrapper : IXmlNodeWrapper
{
    public IXmlNameWrapper Name { get; }
    public SyntaxToken LessThanSlashToken { get; }
    public SyntaxToken GreaterThanToken { get; }
}

public interface IXmlEmptyElementWrapper : IXmlNodeWrapper
{
    public IXmlNameWrapper Name { get; }
    public SyntaxToken LessThanToken { get; }
    public SyntaxToken SlashGreaterThanToken { get; }

    public IReadOnlyList<IXmlAttributeWrapper> Attributes { get; }
}

public interface IXmlCommentWrapper : IXmlNodeWrapper
{
    public SyntaxToken StartCommentToken { get; }
    public SyntaxToken EndCommentToken { get; }
    public SyntaxTokenList TextTokens { get; }
}

public interface IXmlCDataSectionWrapper : IXmlNodeWrapper
{
    public SyntaxToken StartCDataToken { get; }
    public SyntaxToken EndCDataToken { get; }
    public SyntaxTokenList TextTokens { get; }
}

public interface IXmlProcessingInstructionWrapper : IXmlNodeWrapper
{
    public IXmlNameWrapper Name { get; }
    public SyntaxToken StartProcessingInstructionToken { get; }
    public SyntaxToken EndProcessingInstructionToken { get; }
    public SyntaxTokenList TextTokens { get; }
}

public interface IXmlTextWrapper : IXmlNodeWrapper
{
    public SyntaxTokenList TextTokens { get; }
}
