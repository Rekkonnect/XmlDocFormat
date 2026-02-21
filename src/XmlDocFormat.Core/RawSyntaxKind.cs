using Vogen;
using CSharpSyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;
using VisualBasicSyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind;

namespace XmlDocFormat.Core;

[ValueObject<int>(
    fromPrimitiveCasting: CastOperator.Implicit,
    toPrimitiveCasting: CastOperator.Implicit)]
public readonly partial struct RawSyntaxKind
{
    public static readonly RawSyntaxKind None = From(0);

    private const int VisualBasicEnd = 8192;
    private const int CSharpStart = 8193;

    public bool IsValid => Value > 0;

    public bool IsVisualBasic => Value is > 0 and <= VisualBasicEnd;
    public bool IsCSharp => Value is > CSharpStart;

    public static implicit operator RawSyntaxKind(CSharpSyntaxKind kind) => From((int)kind);
    public static implicit operator CSharpSyntaxKind(RawSyntaxKind kind) => (CSharpSyntaxKind)kind.Value;

    public static implicit operator RawSyntaxKind(VisualBasicSyntaxKind kind) => From((int)kind);
    public static implicit operator VisualBasicSyntaxKind(RawSyntaxKind kind) => (VisualBasicSyntaxKind)kind.Value;
}
