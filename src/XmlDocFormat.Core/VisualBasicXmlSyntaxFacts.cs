using Garyon.Objects;
using Microsoft.CodeAnalysis.VisualBasic;

namespace XmlDocFormat.Core;

public sealed class VisualBasicXmlSyntaxFacts : BaseXmlSyntaxFacts, ISharedInstance
{
    // Nodes
    public override RawSyntaxKind XmlName => SyntaxKind.XmlName;
    public override RawSyntaxKind XmlPrefix => SyntaxKind.XmlPrefix;
    public override RawSyntaxKind XmlNameAttribute => SyntaxKind.XmlNameAttribute;
    public override RawSyntaxKind XmlCrefAttribute => SyntaxKind.XmlCrefAttribute;
    public override RawSyntaxKind XmlTextAttribute => SyntaxKind.XmlAttribute;
    public override RawSyntaxKind XmlElement => SyntaxKind.XmlElement;
    public override RawSyntaxKind XmlElementStartTag => SyntaxKind.XmlElementStartTag;
    public override RawSyntaxKind XmlElementEndTag => SyntaxKind.XmlElementEndTag;
    public override RawSyntaxKind XmlEmptyElement => SyntaxKind.XmlEmptyElement;
    public override RawSyntaxKind XmlComment => SyntaxKind.XmlComment;
    public override RawSyntaxKind XmlCDataSection => SyntaxKind.XmlCDataSection;
    public override RawSyntaxKind XmlProcessingInstruction => SyntaxKind.XmlProcessingInstruction;

    // Tokens
    public override RawSyntaxKind XmlTextLiteralNewLineToken => RawSyntaxKind.None;
    public override RawSyntaxKind XmlEntityLiteralToken => SyntaxKind.XmlEntityLiteralToken;
    public override RawSyntaxKind XmlTextLiteralToken => SyntaxKind.XmlTextLiteralToken;

    // Documentation Prefixes
    public override string SingleLineDocumentationPrefix => "'''";
    public override string? MultiLineDocumentationPrefix => null;
}
