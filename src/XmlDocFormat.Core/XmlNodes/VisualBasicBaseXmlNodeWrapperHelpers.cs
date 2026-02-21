using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace XmlDocFormat.Core.XmlNodes;

public static class VisualBasicBaseXmlNodeWrapperHelpers
{
    public static IXmlNodeWrapper? WrapperForNode(VisualBasicSyntaxNode node)
    {
        return node switch
        {
            XmlNameSyntax name => new VisualBasicXmlNameWrapper(name),
            XmlPrefixSyntax prefix => new VisualBasicXmlPrefixWrapper(prefix),
            XmlNameAttributeSyntax name => new VisualBasicXmlNameAttributeWrapper(name),
            XmlCrefAttributeSyntax cref => new VisualBasicXmlCrefAttributeWrapper(cref),
            XmlAttributeSyntax text => new VisualBasicXmlTextAttributeWrapper(text),
            XmlElementSyntax element => new VisualBasicXmlElementWrapper(element),
            XmlElementStartTagSyntax start => new VisualBasicXmlElementStartTagWrapper(start),
            XmlElementEndTagSyntax end => new VisualBasicXmlElementEndTagWrapper(end),
            XmlEmptyElementSyntax empty => new VisualBasicXmlEmptyElementWrapper(empty),
            XmlTextSyntax text => new VisualBasicXmlTextWrapper(text),
            XmlCommentSyntax comment => new VisualBasicXmlCommentWrapper(comment),
            XmlCDataSectionSyntax cdata => new VisualBasicXmlCDataSectionWrapper(cdata),
            XmlProcessingInstructionSyntax processing => new VisualBasicXmlProcessingInstructionWrapper(processing),
            _ => null,
        };
    }
}
