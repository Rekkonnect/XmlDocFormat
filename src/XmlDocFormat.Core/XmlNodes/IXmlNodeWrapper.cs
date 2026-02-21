using Microsoft.CodeAnalysis;

namespace XmlDocFormat.Core.XmlNodes;

public interface IXmlNodeWrapper : IXmlDocTreeNode
{
    public SyntaxNode Node { get; }
}

public interface IXmlNodeWrapper<TNode> : IXmlNodeWrapper
    where TNode : SyntaxNode
{
    SyntaxNode IXmlNodeWrapper.Node => Node;

    public new TNode Node { get; }
}
