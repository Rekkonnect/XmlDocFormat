using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocFormat.Core.XmlNodes;

public static class CSharpBaseXmlNodeWrapperHelpers
{
    public static IXmlNodeWrapper? WrapperForNode(CSharpSyntaxNode node)
    {
        return node switch
        {
            XmlNameSyntax name => new CSharpXmlNameWrapper(name),
            XmlPrefixSyntax prefix => new CSharpXmlPrefixWrapper(prefix),
            XmlNameAttributeSyntax name => new CSharpXmlNameAttributeWrapper(name),
            XmlCrefAttributeSyntax cref => new CSharpXmlCrefAttributeWrapper(cref),
            XmlTextAttributeSyntax text => new CSharpXmlTextAttributeWrapper(text),
            XmlElementSyntax element => new CSharpXmlElementWrapper(element),
            XmlElementStartTagSyntax start => new CSharpXmlElementStartTagWrapper(start),
            XmlElementEndTagSyntax end => new CSharpXmlElementEndTagWrapper(end),
            XmlEmptyElementSyntax empty => new CSharpXmlEmptyElementWrapper(empty),
            XmlTextSyntax text => new CSharpXmlTextWrapper(text),
            XmlCommentSyntax comment => new CSharpXmlCommentWrapper(comment),
            XmlCDataSectionSyntax cdata => new CSharpXmlCDataSectionWrapper(cdata),
            XmlProcessingInstructionSyntax processing => new CSharpXmlProcessingInstructionWrapper(processing),
            _ => null,
        };
    }
}
