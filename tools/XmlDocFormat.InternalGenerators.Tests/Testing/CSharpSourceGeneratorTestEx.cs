using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace XmlDocFormat.InternalGenerators.Tests.Testing;

public abstract class CSharpSourceGeneratorTestEx<TSourceGenerator>
    : CSharpSourceGeneratorTest<TSourceGenerator, DefaultVerifier>
    where TSourceGenerator : IIncrementalGenerator, new()
{
    public abstract ReferenceAssemblies DefaultReferenceAssemblies { get; }
    public abstract IEnumerable<MetadataReference> AdditionalReferences { get; }

    public CSharpSourceGeneratorTestEx()
    {
        ReferenceAssemblies = DefaultReferenceAssemblies;
        TestState.AdditionalReferences.AddRange(AdditionalReferences);
    }
}
