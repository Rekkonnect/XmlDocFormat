namespace XmlDocFormat.Core.XmlNodes;

public interface IXmlDocTreeRootNode : IXmlDocTreeNode
{
    public IEnumerable<IXmlNodeWrapper> Children { get; }
}
