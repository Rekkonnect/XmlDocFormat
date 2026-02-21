using Microsoft.CodeAnalysis;

namespace XmlDocFormat.Core.XmlNodes;

public abstract class BaseXmlNodeWrapper<TNode>(TNode node)
    : IXmlNodeWrapper<TNode>
    where TNode : SyntaxNode
{
    public TNode Node { get; } = node;
}
