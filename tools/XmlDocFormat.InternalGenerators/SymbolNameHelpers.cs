using Microsoft.CodeAnalysis;
using Polyfills;
using RoseLynn.CSharp;
using System.Collections.Immutable;
using System.Text;
using XmlDocFormat.InternalGenerators.Core;

namespace XmlDocFormat.InternalGenerators;

public static class SymbolNameHelpers
{
    public static string FullyQualifiedString(
        ITypeSymbol type,
        Compilation? compilation = null)
    {
        return FullyQualified(type, compilation).Name;
    }

    public static FullyQualifiedName FullyQualified(
        ITypeSymbol type,
        Compilation? compilation = null)
    {
        return new FullyQualifiedNameBuilder(compilation)
            .Build(type);
    }

    public static FullyQualifiedName FullyQualifiedOrKeyword(
        ITypeSymbol type,
        Compilation? compilation = null)
    {
        return type.GetKeywordIdentifierForPredefinedType()
            ?? FullyQualified(type, compilation);
    }

    // REF: Copied from Roslyn's CSharpAddImportFeatureService
    public static string? GetExternAlias(
        INamespaceSymbol namespaceSymbol,
        Compilation compilation)
    {
        var metadataReference = compilation
            .GetMetadataReference(namespaceSymbol.ContainingAssembly);

        var aliases = metadataReference?.Properties.Aliases
            .Where(a => a != MetadataReferenceProperties.GlobalAlias);

        return aliases?.FirstOrDefault();
    }

    // TODO: Handle nullable types gracefully

    private sealed class FullyQualifiedNameBuilder()
        : SymbolVisitor
    {
        private const string _globalNamespaceAlias = "global";

        private readonly Compilation? _compilation;

        private readonly StringBuilder _builder = new();
        private HashSet<string>? _externAliases = null;

        public FullyQualifiedNameBuilder(Compilation? compilation)
            : this()
        {
            _compilation = compilation;
        }

        public override void VisitArrayType(IArrayTypeSymbol symbol)
        {
            Visit(symbol.ElementType);
            _builder.Append('[');
            _builder.Append(',', symbol.Rank - 1);
            _builder.Append(']');
        }

        public override void VisitPointerType(IPointerTypeSymbol symbol)
        {
            Visit(symbol.PointedAtType);
            _builder.Append('*');
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            if (symbol.ContainingNamespace is not null and var parentNamespace)
            {
                VisitNamespace(parentNamespace);
                AppendRegularNamespaceSeparator(parentNamespace);
            }

            AppendNamespace(symbol);
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            if (symbol.ContainingType is not null and var parentType)
            {
                Visit(parentType);
                _builder.Append('.');
            }
            else if (symbol.ContainingNamespace is not null and var @namespace)
            {
                Visit(@namespace);
                AppendRegularNamespaceSeparator(@namespace);
            }

            AppendNamedTypeIdentifier(symbol);
        }

        public override void DefaultVisit(ISymbol symbol)
        {
            _builder.Append(symbol.Name);
        }

        private void AppendNamespace(INamespaceSymbol @namespace)
        {
            if (@namespace.IsGlobalNamespace)
            {
                AppendNamespaceAlias(@namespace);
                return;
            }

            AppendRegularNamespace(@namespace);
        }

        private void AppendRegularNamespaceSeparator(INamespaceSymbol @namespace)
        {
            if (!@namespace.IsGlobalNamespace)
            {
                _builder.Append('.');
            }
        }

        private void AppendRegularNamespace(INamespaceSymbol @namespace)
        {
            GoodAssert.False(@namespace.IsGlobalNamespace);
            _builder.Append(@namespace.Name);
        }

        private void AppendNamespaceAlias(
            INamespaceSymbol @namespace,
            bool appendDoubleColon = true)
        {
            GoodAssert.True(@namespace.IsGlobalNamespace);

            var prefix = GlobalNamespacePrefix(@namespace);
            _builder.Append(prefix);
            if (appendDoubleColon)
            {
                _builder.Append("::");
            }
        }

        public override void VisitFunctionPointerType(IFunctionPointerTypeSymbol symbol)
        {
            // TODO
        }

        public override void VisitTypeParameter(ITypeParameterSymbol symbol)
        {
            _builder.Append(symbol.Name);
        }

        public override void VisitDynamicType(IDynamicTypeSymbol symbol)
        {
            _builder.Append("dynamic");
        }

        private void AppendNamedTypeIdentifier(INamedTypeSymbol symbol)
        {
            _builder.Append(symbol.Name);

            if (symbol.IsGenericType)
            {
                AppendGenericArguments(symbol.TypeArguments);
            }
        }

        private void AppendGenericArguments(ImmutableArray<ITypeSymbol> typeArguments)
        {
            _builder.Append('<');

            var hasPrevious = false;
            foreach (var typeArgument in typeArguments)
            {
                if (hasPrevious)
                {
                    _builder.Append(", ");
                }
                Visit(typeArgument);
                hasPrevious = true;
            }

            _builder.Append('>');
        }

        private HashSet<string> ExternAliasesSet() => _externAliases = new();

        public FullyQualifiedName Build(ISymbol symbol)
        {
            Visit(symbol);
            var name = _builder.ToString();
            var externAliases = _externAliases?.ToImmutableArray() ?? [];
            return new(name, externAliases);
        }

        private string GlobalNamespacePrefix(INamespaceSymbol @namespace)
        {
            string? alias = null;

            if (_compilation is not null)
            {
                alias = GetExternAlias(@namespace, _compilation);
                alias.IfNotNull(RegisterExternAlias);
            }

            return alias ?? _globalNamespaceAlias;
        }

        private void RegisterExternAlias(string alias)
        {
            ExternAliasesSet().Add(alias);
        }
    }

    public sealed record FullyQualifiedName(
        string Name,
        ImmutableArray<string> ExternAliases)
    {
        public FullyQualifiedName(string name)
            : this(name, []) { }

        public static implicit operator FullyQualifiedName(string name) => new(name);
    }
}
