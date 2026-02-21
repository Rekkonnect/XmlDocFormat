using Dentextist;
using Microsoft.CodeAnalysis;
using RoseLynn;
using RoseLynn.CSharp;
using System.Collections.Immutable;

namespace XmlDocFormat.InternalGenerators;

public class CSharpCodeBuilder(char indentationCharacter = ' ', int indentationSize = 4)
    : IndentedStringBuilder(indentationCharacter, indentationSize)
{
    protected SeparatableBlockList GlobalSeparatableBlockList
    {
        get => field ??= NewSeparatableBlockList();
    }

    public void SeparateBlock()
    {
        GlobalSeparatableBlockList.SeparateBlock();
    }

    public void CommitBlock()
    {
        GlobalSeparatableBlockList.CommitBlock();
    }

    public SeparatableBlockList NewSeparatableBlockList() => new(this);

    public void AppendDoubleLine()
    {
        AppendLine();
        AppendLine();
    }

    public void CommitSingleLineContent(string content)
    {
        AppendSingleLineContent(content);
        AppendLine();
    }

    public TypeScope AppendPartialType(INamedTypeSymbol type)
    {
        var scope = new TypeScope(this, type);
        return scope;
    }

    protected static string FullyQualifiedName(ITypeSymbol type)
    {
        var displayString = type.ToDisplayString();
        var hasKeyword = type.HasKeywordIdentifier();
        if (hasKeyword)
        {
            return displayString;
        }

        // TODO: Formally support generic type arguments too
        // Right now, they are correctly shown but without the global:: prefix
        return $"global::{displayString}";
    }

    protected static string ParameterPassExpression(IParameterSymbol parameter)
    {
        string prefix = parameter.RefKind switch
        {
            RefKind.In => "in ",
            RefKind.RefReadOnlyParameter => "in ",
            RefKind.Out => "out ",
            RefKind.Ref => "ref ",
            _ => string.Empty,
        };
        return $"{prefix}{parameter.Name}";
    }

    public void WriteParameterList(
        ImmutableArray<IParameterSymbol> parameters)
    {
        WriteCommaSeparatedList(
            parameters.Select(static s => s.ToDisplayString()));
    }

    public void WriteTypeParameterListWithBrackets(
        ImmutableArray<ITypeParameterSymbol> typeParameters)
    {
        if (typeParameters is [])
        {
            return;
        }

        Append('<');
        WriteTypeParameterList(typeParameters);
        Append('>');
    }

    public void WriteTypeParameterList(
        ImmutableArray<ITypeParameterSymbol> typeParameters)
    {
        WriteCommaSeparatedList(
            typeParameters.Select(static s => s.Name));
    }

    public void WriteCommaSeparatedList(
        IEnumerable<string> strings)
    {
        bool first = true;
        foreach (var @string in strings)
        {
            if (!first)
            {
                Append(", ");
            }

            Append(@string);
            first = false;
        }
    }

    protected void WriteAccessibility(ISymbol symbol)
    {
        WriteAccessibility(symbol.DeclaredAccessibility);
    }

    protected void WriteAccessibility(Accessibility accessibility)
    {
        var keyword = GetAccessibilityKeyword(accessibility);
        Append(keyword);
    }

    protected static string GetAccessibilityKeyword(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Private => "private",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.ProtectedAndInternal => "private protected",
            _ => string.Empty,
        };
    }

    protected static string GetTypeKindKeywords(IdentifiableSymbolKind identifiableKind)
    {
        return identifiableKind switch
        {
            IdentifiableSymbolKind.Class => "class",
            IdentifiableSymbolKind.Struct => "struct",
            IdentifiableSymbolKind.Interface => "interface",
            IdentifiableSymbolKind.RecordClass => "record class",
            IdentifiableSymbolKind.RecordStruct => "record struct",
            _ => throw new InvalidOperationException("Unsupported type kind."),
        };
    }

    public BracketBlock EnterBracketBlock(char open = '{', char close = '}')
    {
        return new BracketBlock(this, open, close);
    }

    public sealed class TypeScope : IDisposable
    {
        private readonly CSharpCodeBuilder _builder;
        private int _nestingLevels;

        public TypeScope(CSharpCodeBuilder builder, INamedTypeSymbol type)
        {
            _builder = builder;
            var initialNesting = builder.NestingLevel;
            InitializeForType(type);
            _nestingLevels = builder.NestingLevel - initialNesting;
        }

        private void InitializeForType(INamedTypeSymbol type)
        {
            var containingType = type.ContainingType;
            if (containingType is not null)
            {
                InitializeForType(containingType);
            }
            else
            {
                var containingNamespace = type.ContainingNamespace;
                WriteNamespace(containingNamespace);
            }

            WriteType(type);
        }

        private void WriteNamespace(INamespaceSymbol @namespace)
        {
            if (@namespace.IsGlobalNamespace)
                return;

            _builder.AppendSingleLineContent(
                $"namespace {@namespace.ToDisplayString()};");

            _builder.AppendDoubleLine();
        }

        private void WriteType(INamedTypeSymbol type)
        {
            var identifiableKind = type.GetIdentifiableSymbolKind();
            var typeKind = GetTypeKindKeywords(identifiableKind);
            _builder.Append($"partial {typeKind} {type.Name}");
            _builder.WriteTypeParameterListWithBrackets(type.TypeParameters);
            _builder.AppendLine();
            _builder.AppendLine('{');
            _builder.IncrementNestingLevel();
        }

        public void Dispose()
        {
            for (int i = 0; i < _nestingLevels; i++)
            {
                _builder.NestingLevel--;
                _builder.AppendLine('}');
            }
        }
    }

    public readonly struct BracketBlock : IDisposable
    {
        private readonly CSharpCodeBuilder _builder;

        private readonly char _close;

        [Obsolete("Do not use the parameterless constructor", true)]
        public BracketBlock()
        {
            _builder = null!;
        }

        public BracketBlock(CSharpCodeBuilder builder, char open, char close)
        {
            _builder = builder;
            _close = close;
            _builder.AppendLine(open);
            _builder.NestingLevel++;
        }

        public void Dispose()
        {
            _builder.NestingLevel--;
            _builder.AppendLine(_close);
        }
    }

    public sealed class SeparatableBlockList(CSharpCodeBuilder builder)
    {
        private readonly CSharpCodeBuilder _builder = builder;

        private bool _hasPrevious = false;

        public void SeparateBlock()
        {
            if (_hasPrevious)
            {
                _builder.AppendLine();
            }
        }

        public void CommitBlock()
        {
            _hasPrevious = true;
        }
    }
}
