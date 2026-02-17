using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using XmlDocFormat.Core.XmlNodes;

namespace XmlDocFormat.Core;

public sealed class CSharpXmlDocTree
    : BaseXmlDocTree<CSharpXmlSyntaxFacts>
{
    private CSharpXmlDocTree(CSharpXmlDocTreeRootNode root)
        : base(root)
    {
    }

    public static CSharpXmlDocTree ParseFromDocumentationTrivia(
        DocumentationCommentTriviaSyntax documentation)
    {
        var xmlNodes = documentation.Content;
        var wrappers = xmlNodes.Select(CSharpBaseXmlNodeWrapperHelpers.WrapperForNode);
        var root = new CSharpXmlDocTreeRootNode(wrappers.ToList()!);
        return new(root);
    }

    private sealed class CSharpXmlDocTreeRootNode(IReadOnlyList<IXmlNodeWrapper> children)
        : IXmlDocTreeRootNode
    {
        private readonly IReadOnlyList<IXmlNodeWrapper> _children = children;

        public IEnumerable<IXmlNodeWrapper> Children => _children;
    }
}
