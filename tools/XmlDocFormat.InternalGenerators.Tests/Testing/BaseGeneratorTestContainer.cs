using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using RoseLynn.Generators;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using XmlDocFormat.Core.FileExtensions;
using XmlDocFormat.Tests.Shared;

namespace XmlDocFormat.InternalGenerators.Tests.Testing;

public abstract partial class BaseGeneratorTestContainer<TSourceGenerator>
    : BaseDrivenCSharpIncrementalGeneratorTestContainer<
        TSourceGenerator,
        BaseGeneratorTestContainer<TSourceGenerator>.Test>
    where TSourceGenerator : IIncrementalGenerator, new()
{
    protected override IEnumerable<MetadataReference> DefaultMetadataReferences
        => BaseGeneratorMetadataReferences.BaseReferences;

    public async Task VerifyOrWriteExpectationAsync(
        string source,
        GeneratedSourceMappings mappings,
        CancellationToken cancellationToken = default)
    {
        await VerifyOrWriteExpectationAsync([source], mappings, cancellationToken);
    }

    public async Task VerifyOrWriteExpectationAsync(
        IEnumerable<string> sources,
        GeneratedSourceMappings mappings,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await VerifyAsync(sources, mappings, cancellationToken);
        }
        catch
        {
            await HandleGeneratorAssertionFailure(
                sources, mappings, cancellationToken);
            throw;
        }
    }

    private async Task HandleGeneratorAssertionFailure(
        IEnumerable<string> sources,
        GeneratedSourceMappings mappings,
        CancellationToken cancellationToken)
    {
        // Rerun the generator
        var context = TestContext.Current;
        if (context is null)
        {
            throw new InvalidCastException("Cannot retrieve the current test context");
        }

        var testName = $"{context.ClassContext.ClassType.Name}.{context.Metadata.TestName}";
        var outputFile = await RunTestWriteGeneratorOutput(testName, sources, cancellationToken);

        context.Output.AttachArtifact(
            filePath: outputFile.FullName,
            description: "Expected generator sources output");
    }

    private async Task<IFileInfo> RunTestWriteGeneratorOutput(
        string testName,
        IEnumerable<string> sources,
        CancellationToken cancellationToken = default)
    {
        var resultingCompilation = CreateCompilationRunGenerator(
            sources,
            out var generator,
            out var resultingGeneratorDriver,
            out var initialCompilation,
            cancellationToken);

        return await BaseGeneratorTestContainer<TSourceGenerator>.WriteTestGeneratorOutput(
            testName,
            resultingGeneratorDriver,
            cancellationToken);
    }

    private static async Task<IFileInfo> WriteTestGeneratorOutput(
        string testName,
        GeneratorDriver resultingGeneratorDriver,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var resultFileName = $"generator_outputs/{testName}_{now:yyyyMMdd_HHmmss}.cs";

        var runResult = resultingGeneratorDriver.GetRunResult();
        var results = runResult.Results;
        var outputMappings = new GeneratedSourceMappings();

        foreach (var result in results)
        {
            foreach (var source in result.GeneratedSources)
            {
                var fileName = source.HintName;
                var text = source.SourceText;

                outputMappings.Add(fileName, text);
            }
        }

        var code = GeneratorOutputWriter.GenerateSourceMappingsCode(outputMappings);
        var fileSystem = new FileSystem();
        var targetFile = fileSystem.FileInfo.New(resultFileName);
        targetFile.Directory?.CreateSafe();
        await File.WriteAllTextAsync(targetFile.FullName, code, cancellationToken);

        return targetFile;
    }

    public static SourceText Utf8CSharpSource(
        [StringSyntax(PredefinedEmbeddedLanguageNames.CSharpTest)]
        string source)
    {
        return GeneralGeneratorTestHelpers.Utf8CSharpSource(source);
    }

    public sealed class Test : CSharpSourceGeneratorTestEx<TSourceGenerator>
    {
        public override ReferenceAssemblies DefaultReferenceAssemblies
            => ReferenceAssemblies.Net.Net100;
        public override IEnumerable<MetadataReference> AdditionalReferences
            => BaseGeneratorMetadataReferences.BaseReferences;
    }
}
