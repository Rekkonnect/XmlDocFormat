using Microsoft.CodeAnalysis;

namespace XmlDocFormat.InternalGenerators;

public sealed record CompilationWrappedSymbol<TSymbol>(
    Compilation Compilation,
    TSymbol Symbol)
    where TSymbol : ISymbol
{
    public static implicit operator TSymbol(CompilationWrappedSymbol<TSymbol> wrapper)
        => wrapper.Symbol;
}