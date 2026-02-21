using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace XmlDocFormat.Core.XmlNodes;

public abstract class CSharpBaseXmlNodeWrapper<TNode>(TNode node)
    : BaseXmlNodeWrapper<TNode>(node)
    where TNode : CSharpSyntaxNode
{
    protected static IReadOnlyList<IXmlNodeWrapper> NodeWrappers<TXmlNode>(
        SyntaxList<TXmlNode> nodes)
        where TXmlNode : CSharpSyntaxNode
    {
        return nodes.Select(WrapperForNode).ToArray()!;
    }

    protected static IReadOnlyList<TXmlWrapper> NodeWrappers<TXmlNode, TXmlWrapper>(
        SyntaxList<TXmlNode> nodes)
        where TXmlNode : CSharpSyntaxNode
        where TXmlWrapper : class, IXmlNodeWrapper
    {
        return nodes.Select(WrapperForNode<TXmlWrapper>).ToArray()!;
    }

    protected static IReadOnlyList<IXmlNodeWrapper> NodeWrappers(
        IEnumerable<CSharpSyntaxNode> nodes)
    {
        return nodes.Select(WrapperForNode).ToArray()!;
    }

    protected static IXmlNodeWrapper? WrapperForNode(CSharpSyntaxNode node)
    {
        return CSharpBaseXmlNodeWrapperHelpers.WrapperForNode(node);
    }

    protected static TXmlWrapper? WrapperForNode<TXmlWrapper>(CSharpSyntaxNode node)
        where TXmlWrapper : class, IXmlNodeWrapper
    {
        return WrapperForNode(node) as TXmlWrapper;
    }
}
