using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using RoseLynn;
using System.Diagnostics.CodeAnalysis;
using XmlDocFormat.Tests.Shared;

using SampleGaryonType = Garyon.Objects.AppVersionInfo;

namespace XmlDocFormat.InternalGenerators.Tests;

public sealed class SymbolNameHelpersTests
{
    [Test]
    public async Task StringType(CancellationToken cancellationToken)
    {
        const string source =
            """
            string version = null!;
            """;

        var compilation = CreateGaryonAliasedCompilation(
            source, [], out var tree, cancellationToken);

        var variableSpan = SpanFor(source, "string version");
        var root = tree.GetRoot(cancellationToken);
        var variableDeclaration = root.FindNode(variableSpan) as VariableDeclarationSyntax;
        await Assert.That(variableDeclaration).IsNotNull();

        var semanticModel = compilation.GetSemanticModel(tree);
        var typeInfo = semanticModel.GetTypeInfo(variableDeclaration.Type, cancellationToken);
        var type = typeInfo.Type;
        await Assert.That(type).IsNotNull();

        var qualifiedString = SymbolNameHelpers.FullyQualifiedString(type, compilation);
        await Assert.That(qualifiedString).EqualTo("global::System.String");
    }

    [Test]
    public async Task StringTypeWithAvailableIrrelevantAliases(CancellationToken cancellationToken)
    {
        const string source =
            """
            extern alias Gar;

            string version = null!;
            """;

        var compilation = CreateGaryonAliasedCompilation(
            source, ["Gar"], out var tree, cancellationToken);

        var variableSpan = SpanFor(source, "string version");
        var root = tree.GetRoot(cancellationToken);
        var variableDeclaration = root.FindNode(variableSpan) as VariableDeclarationSyntax;
        await Assert.That(variableDeclaration).IsNotNull();

        var semanticModel = compilation.GetSemanticModel(tree);
        var typeInfo = semanticModel.GetTypeInfo(variableDeclaration.Type, cancellationToken);
        var type = typeInfo.Type;
        await Assert.That(type).IsNotNull();

        var qualifiedString = SymbolNameHelpers.FullyQualifiedString(type, compilation);
        await Assert.That(qualifiedString).EqualTo("global::System.String");
    }

    [Test]
    public async Task SingleExternAlias(CancellationToken cancellationToken)
    {
        const string source =
            """
            extern alias Gar;

            Gar::Garyon.Objects.AppVersionInfo version = null!;
            """;

        var compilation = CreateGaryonAliasedCompilation(
            source, ["Gar"], out var tree, cancellationToken);

        var qualificationSpan = SpanFor(source, "Gar::");
        var root = tree.GetRoot(cancellationToken);
        var qualificationNode = root.FindNode(qualificationSpan) as AliasQualifiedNameSyntax;
        await Assert.That(qualificationNode).IsNotNull();

        var variableDeclaration = qualificationNode.FirstAncestorOrSelf<VariableDeclarationSyntax>();
        await Assert.That(variableDeclaration).IsNotNull();

        var semanticModel = compilation.GetSemanticModel(tree);
        var typeInfo = semanticModel.GetTypeInfo(variableDeclaration.Type, cancellationToken);
        var type = typeInfo.Type;
        await Assert.That(type).IsNotNull();

        var qualifiedString = SymbolNameHelpers.FullyQualifiedString(type, compilation);
        await Assert.That(qualifiedString).EqualTo("Gar::Garyon.Objects.AppVersionInfo");
    }

    [Test]
    public async Task MultiExternAliasOverSame(CancellationToken cancellationToken)
    {
        const string source =
            """
            extern alias Gar;
            extern alias Gar2;

            System.Collections.Generic.KeyValuePair<
                Gar::Garyon.Objects.AppVersionInfo,
                Gar2::Garyon.Objects.AppVersionInfo> pair = default!;
            """;

        var compilation = CreateGaryonAliasedCompilation(
            source, ["Gar", "Gar2"], out var tree, cancellationToken);

        var variableNameSpan = SpanFor(source, "AppVersionInfo> pair");
        var root = tree.GetRoot(cancellationToken);
        var variableDeclaration = root.FindNode(variableNameSpan) as VariableDeclarationSyntax;
        await Assert.That(variableDeclaration).IsNotNull();

        var semanticModel = compilation.GetSemanticModel(tree);
        var typeInfo = semanticModel.GetTypeInfo(variableDeclaration.Type, cancellationToken);
        var type = typeInfo.Type;
        await Assert.That(type).IsNotNull();

        var qualifiedString = SymbolNameHelpers.FullyQualifiedString(type, compilation);

        // Notice the double usage of Gar instead of Gar2
        // This is intended and the only reliable way to support extern aliases,
        // as the aliased global namespace is only accessible via any of the the non-global
        // global namespace aliases that the metadata reference holds
        await Assert.That(qualifiedString)
            .EqualTo("global::System.Collections.Generic.KeyValuePair<Gar::Garyon.Objects.AppVersionInfo, Gar::Garyon.Objects.AppVersionInfo>");
    }

    private static CSharpCompilation CreateGaryonAliasedCompilation(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)]
        string source,
        IReadOnlyList<string> aliases,
        out SyntaxTree tree,
        CancellationToken cancellationToken)
    {
        tree = CSharpSyntaxTree.ParseText(
            source,
            cancellationToken: cancellationToken);
        return CreateGaryonAliasedCompilation(tree, aliases);
    }

    private static CSharpCompilation CreateGaryonAliasedCompilation(
        SyntaxTree tree, IReadOnlyList<string> aliases)
    {
        return CSharpCompilation.Create(null)
            .AddSyntaxTrees([tree])
            .AddReferences(
                CreateGaryonMetadataReference()
                    .WithAliases(aliases));
    }

    private static PortableExecutableReference CreateGaryonMetadataReference()
    {
        return MetadataReferenceFactory
            .CreateFromType<SampleGaryonType>();
    }

    private static TextSpan SpanFor(string outer, string substring)
    {
        var start = outer.IndexOf(substring);
        if (start < 0)
        {
            return default;
        }

        return new(start, substring.Length);
    }
}
