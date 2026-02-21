using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using RoseLynn.Generators;
using RoseLynn.Testing;

namespace XmlDocFormat.InternalGenerators.Tests.Testing;

// Copied from RoseLynn, pending update for the new components
public abstract class BaseDrivenCSharpIncrementalGeneratorTestContainer<TSourceGenerator, TTest>
    : CSharpSourceGeneratorVerifier<TSourceGenerator, DefaultVerifier>
    where TSourceGenerator : IIncrementalGenerator, new()
    where TTest : CSharpSourceGeneratorTest<TSourceGenerator, DefaultVerifier>, new()
{
    /// <summary>
    /// Gets the default metadata references that are mandatory for the operation of the test container
    /// which will be used when creating <seealso cref="Compilation"/> instances to run the generator.
    /// </summary>
    protected virtual IEnumerable<MetadataReference> DefaultMetadataReferences => [];

    /// <summary>
    /// Gets the default language version to use when creating <seealso cref="Compilation"/> instances.
    /// </summary>
    protected virtual LanguageVersion LanguageVersion => LanguageVersion.LatestMajor;

    /// <summary>
    /// Gets the default output kind to use when creating <seealso cref="Compilation"/> instances.
    /// </summary>
    protected virtual OutputKind OutputKind => OutputKind.NetModule;

    /// <summary>
    /// Creates a <seealso cref="Compilation"/> and for the provided collection of sources and initializes
    /// instances around running the generator on the provided sources.
    /// </summary>
    /// <param name="sources">The collection of sources the generator will act upon.</param>
    /// <param name="generator">The generator instance that was created.</param>
    /// <param name="resultingGeneratorDriver">The resulting generator driver.</param>
    /// <param name="initialCompilation">The initial compilation before the generator runs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The resulting compilation after the source generator is run.</returns>
    protected Compilation CreateCompilationRunGenerator(
        IEnumerable<string> sources,
        out TSourceGenerator generator,
        out GeneratorDriver resultingGeneratorDriver,
        out Compilation initialCompilation,
        CancellationToken cancellationToken = default)
    {
        CreateDriverGenerator(out generator, out var driver);

        return CreateCompilationRunGenerator(
            sources,
            driver,
            out resultingGeneratorDriver,
            out initialCompilation,
            cancellationToken);
    }

    /// <summary>
    /// Creates a <seealso cref="Compilation"/> and for the provided collection of sources and initializes
    /// instances around running the generator on the provided sources.
    /// </summary>
    /// <param name="source">The collection of sources the generator will act upon.</param>
    /// <inheritdoc cref="CreateCompilationRunGenerator(IEnumerable{string}, out TSourceGenerator, out GeneratorDriver, out Compilation, CancellationToken)"/>
    protected Compilation CreateCompilationRunGenerator(
        string source,
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
        out TSourceGenerator generator,
        out GeneratorDriver resultingGeneratorDriver,
        out Compilation initialCompilation,
        CancellationToken cancellationToken = default)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
    {
        return CreateCompilationRunGenerator(
            [source],
            out generator,
            out resultingGeneratorDriver,
            out initialCompilation,
            cancellationToken);
    }

    private Compilation CreateCompilationRunGenerator(
        IEnumerable<string> sources,
        GeneratorDriver driver,
        out GeneratorDriver resultingGeneratorDriver,
        out Compilation initialCompilation,
        CancellationToken cancellationToken = default)
    {
        var references = DefaultMetadataReferences;
        var parseOptions = new CSharpParseOptions(LanguageVersion);
        var trees = sources
            .Select(source => CSharpSyntaxTree.ParseText(source, options: parseOptions));
        var options = new CSharpCompilationOptions(OutputKind);
        initialCompilation = CSharpCompilation.Create(null, trees, references, options);

        resultingGeneratorDriver = driver.RunGeneratorsAndUpdateCompilation(
            initialCompilation, out var resultCompilation, out _, cancellationToken);
        return resultCompilation;
    }

    /// <summary>
    /// Creates a new instance of the <typeparamref name="TSourceGenerator"/>
    /// type and initializes a <seealso cref="GeneratorDriver"/> using that
    /// generator.
    /// </summary>
    /// <param name="generator">The initialized generator instance.</param>
    /// <param name="driver">The initiailzed <seealso cref="GeneratorDriver"/> instance.</param>
    protected virtual void CreateDriverGenerator(
        out TSourceGenerator generator,
        out GeneratorDriver driver)
    {
        generator = new();
        driver = CSharpGeneratorDriver.Create(generator);
    }

    #region Verifying
    /// <summary>
    /// Verifies that the provided collection of sources causes the generator
    /// to produce sources matching the given mappings.
    /// </summary>
    /// <param name="sources">The original collection of sources that the source generator will act upon.</param>
    /// <param name="mappings">The expected generated source mappings.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task VerifyAsync(
        IEnumerable<string> sources,
        GeneratedSourceMappings mappings,
        CancellationToken cancellationToken = default)
    {
        await VerifyAsync(
            sources,
            mappings,
            new TTest(),
            cancellationToken);
    }

    /// <summary>
    /// Verifies that the provided collection of sources causes the generator to produce
    /// sources matching the given mappings, acting on a pre-initialized
    /// <typeparamref name="TTest"/> instance.
    /// </summary>
    /// <param name="sources">The original collection of sources that the source generator will act upon.</param>
    /// <param name="mappings">The expected generated source mappings.</param>
    /// <param name="test">
    /// The custom
    /// <typeparamref name="TTest"/> instance.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task VerifyAsync(
        IEnumerable<string> sources,
        GeneratedSourceMappings mappings,
        TTest test,
        CancellationToken cancellationToken = default)
    {
        test.TestState.Sources.AddRange(sources);
        foreach (var mapping in mappings)
        {
            test.TestState.GeneratedSources.Add((typeof(TSourceGenerator), mapping.Key, mapping.Value));
        }

        await test.RunAsync(cancellationToken);
    }
    /// <summary>
    /// Verifies that the provided source causes the generator to produce sources matching the given mappings.
    /// </summary>
    /// <param name="source">The original source that the source generator will act upon.</param>
    /// <param name="mappings">The expected generated source mappings.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task VerifyAsync(
        string source,
        GeneratedSourceMappings mappings,
        CancellationToken cancellationToken = default)
    {
        await VerifyAsync([source], mappings, cancellationToken);
    }

    /// <summary>
    /// Verifies that the provided source causes the generator to produce sources matching the given mappings,
    /// acting on a pre-initialized <typeparamref name="TTest"/> instance.
    /// </summary>
    /// <param name="source">The original source that the source generator will act upon.</param>
    /// <param name="mappings">The expected generated source mappings.</param>
    /// <param name="test">The custom <typeparamref name="TTest"/> instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task VerifyAsync(
        string source,
        GeneratedSourceMappings mappings,
        TTest test,
        CancellationToken cancellationToken = default)
    {
        await VerifyAsync([source], mappings, test, cancellationToken);
    }

    /// <summary>
    /// Verifies that the provided source causes the generator to produce one single source,
    /// with the specified hint name and source.
    /// </summary>
    /// <param name="source">The original source that the source generator will act upon.</param>
    /// <param name="generatedHintName">The expected hint name of the single generated source.</param>
    /// <param name="generatedSource">The single generated source.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task VerifyAsync(
        string source,
        string generatedHintName,
        string generatedSource,
        CancellationToken cancellationToken = default)
    {
        var mappings = new GeneratedSourceMappings()
        {
            { generatedHintName, generatedSource }
        };
        await VerifyAsync(source, mappings, cancellationToken);
    }

    /// <inheritdoc cref="VerifyAsync(IEnumerable{string}, GeneratedSourceMappings, CancellationToken)"/>
    protected async Task VerifyAsync(
        string source,
        IEnumerable<(string fileName, string source)> mappings,
        CancellationToken cancellationToken = default)
    {
        await VerifyAsync(
            [source],
            mappings,
            new TTest(),
            cancellationToken);
    }

    /// <inheritdoc cref="VerifyAsync(IEnumerable{string}, GeneratedSourceMappings, CancellationToken)"/>
    protected async Task VerifyAsync(
        string source,
        IEnumerable<(string fileName, SourceText source)> mappings,
        CancellationToken cancellationToken = default)
    {
        await VerifyAsync(
            [source],
            mappings,
            new TTest(),
            cancellationToken);
    }

    /// <inheritdoc cref="VerifyAsync(IEnumerable{string}, GeneratedSourceMappings, TTest, CancellationToken)"/>
    protected async Task VerifyAsync(
        IEnumerable<string> sources,
        IEnumerable<(string fileName, SourceText source)> mappings,
        TTest test,
        CancellationToken cancellationToken = default)
    {
        test.TestState.Sources.AddRange(sources);
        foreach (var (fileName, source) in mappings)
        {
            test.TestState.GeneratedSources.Add((typeof(TSourceGenerator), fileName, source));
        }

        await test.RunAsync(cancellationToken);
    }

    /// <inheritdoc cref="VerifyAsync(IEnumerable{string}, GeneratedSourceMappings, TTest, CancellationToken)"/>
    protected async Task VerifyAsync(
        IEnumerable<string> sources,
        IEnumerable<(string fileName, string source)> mappings,
        TTest test,
        CancellationToken cancellationToken = default)
    {
        test.TestState.Sources.AddRange(sources);
        foreach (var (fileName, source) in mappings)
        {
            test.TestState.GeneratedSources.Add((typeof(TSourceGenerator), fileName, source));
        }

        await test.RunAsync(cancellationToken);
    }
    #endregion
}
