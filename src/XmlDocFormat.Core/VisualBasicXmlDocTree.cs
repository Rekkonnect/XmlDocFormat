using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using XmlDocFormat.Core.XmlNodes;

namespace XmlDocFormat.Core;

public sealed class VisualBasicXmlDocTree
    : BaseXmlDocTree<VisualBasicXmlSyntaxFacts>
{
    private VisualBasicXmlDocTree(VisualBasicXmlDocTreeRootNode root)
        : base(root)
    {
    }

    public static VisualBasicXmlDocTree ParseFromDocumentationTrivia(
        DocumentationCommentTriviaSyntax documentation)
    {
        var xmlNodes = documentation.Content;
        var wrappers = xmlNodes.Select(VisualBasicBaseXmlNodeWrapperHelpers.WrapperForNode);
        var root = new VisualBasicXmlDocTreeRootNode(wrappers.ToList()!);
        return new(root);
    }

    private sealed class VisualBasicXmlDocTreeRootNode(IReadOnlyList<IXmlNodeWrapper> children)
        : IXmlDocTreeRootNode
    {
        private readonly IReadOnlyList<IXmlNodeWrapper> _children = children;

        public IEnumerable<IXmlNodeWrapper> Children => _children;
    }
}
