using Microsoft.CodeAnalysis;
using RoseLynn;
using System.Collections.Immutable;
using XmlDocFormat.Core;
using XmlDocFormat.InternalGenerators.Core;

namespace XmlDocFormat.InternalGenerators.Tests.Testing;

public static class BaseGeneratorMetadataReferences
{
    public static readonly ImmutableArray<MetadataReference> BaseReferences;

    static BaseGeneratorMetadataReferences()
    {
        BaseReferences = [
            MetadataReferenceFactory.CreateFromType<AssemblyMarker_XmlDocFormatCore>(),
            MetadataReferenceFactory.CreateFromType<AssemblyMarker_XmlDocFormatInternalGeneratorsCore>(),
        ];
    }
}
