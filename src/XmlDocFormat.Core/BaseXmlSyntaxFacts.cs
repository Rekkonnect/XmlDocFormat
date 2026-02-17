namespace XmlDocFormat.Core;

public abstract class BaseXmlSyntaxFacts
{
    // Nodes
    public abstract RawSyntaxKind XmlName { get; }
    public abstract RawSyntaxKind XmlPrefix { get; }
    public abstract RawSyntaxKind XmlNameAttribute { get; }
    public abstract RawSyntaxKind XmlCrefAttribute { get; }
    public abstract RawSyntaxKind XmlTextAttribute { get; }
    public abstract RawSyntaxKind XmlElement { get; }
    public abstract RawSyntaxKind XmlElementStartTag { get; }
    public abstract RawSyntaxKind XmlElementEndTag { get; }
    public abstract RawSyntaxKind XmlEmptyElement { get; }
    public abstract RawSyntaxKind XmlComment { get; }
    public abstract RawSyntaxKind XmlCDataSection { get; }
    public abstract RawSyntaxKind XmlProcessingInstruction { get; }

    // Tokens
    public abstract RawSyntaxKind XmlTextLiteralNewLineToken { get; }
    public abstract RawSyntaxKind XmlEntityLiteralToken { get; }
    public abstract RawSyntaxKind XmlTextLiteralToken { get; }

    // Documentation Prefixes
    public abstract string SingleLineDocumentationPrefix { get; }
    public abstract string? MultiLineDocumentationPrefix { get; }
}
