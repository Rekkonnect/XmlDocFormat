using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;

namespace XmlDocFormat.Core.XmlNodes;

public abstract class VisualBasicBaseXmlNodeWrapper<TNode>(TNode node)
    : BaseXmlNodeWrapper<TNode>(node)
    where TNode : VisualBasicSyntaxNode
{
    protected static IReadOnlyList<IXmlNodeWrapper> NodeWrappers<TXmlNode>(
        SyntaxList<TXmlNode> nodes)
        where TXmlNode : VisualBasicSyntaxNode
    {
        return nodes.Select(WrapperForNode).ToArray()!;
    }

    protected static IReadOnlyList<TXmlWrapper> NodeWrappers<TXmlNode, TXmlWrapper>(
        SyntaxList<TXmlNode> nodes)
        where TXmlNode : VisualBasicSyntaxNode
        where TXmlWrapper : class, IXmlNodeWrapper
    {
        return nodes.Select(WrapperForNode<TXmlWrapper>).ToArray()!;
    }

    protected static IReadOnlyList<IXmlNodeWrapper> NodeWrappers(
        IEnumerable<VisualBasicSyntaxNode> nodes)
    {
        return nodes.Select(WrapperForNode).ToArray()!;
    }

    protected static IXmlNodeWrapper? WrapperForNode(VisualBasicSyntaxNode node)
    {
        return VisualBasicBaseXmlNodeWrapperHelpers.WrapperForNode(node);
    }

    protected static TXmlWrapper? WrapperForNode<TXmlWrapper>(VisualBasicSyntaxNode node)
        where TXmlWrapper : class, IXmlNodeWrapper
    {
        return WrapperForNode(node) as TXmlWrapper;
    }
}
