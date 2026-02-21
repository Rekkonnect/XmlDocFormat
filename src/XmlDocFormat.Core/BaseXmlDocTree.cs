using Garyon.Objects;
using XmlDocFormat.Core.XmlNodes;

namespace XmlDocFormat.Core;

public abstract class BaseXmlDocTree(IXmlDocTreeRootNode root)
{
    public readonly IXmlDocTreeRootNode Root = root;
}

public abstract class BaseXmlDocTree<TXmlSyntaxFacts>(IXmlDocTreeRootNode root)
    : BaseXmlDocTree(root)
    where TXmlSyntaxFacts : BaseXmlSyntaxFacts, ISharedInstance
{
}
