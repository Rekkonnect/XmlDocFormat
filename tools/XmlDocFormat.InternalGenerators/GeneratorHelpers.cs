using Microsoft.CodeAnalysis;

namespace XmlDocFormat.InternalGenerators;

public static class GeneratorHelpers
{
    public static string FileNameForType(INamedTypeSymbol type)
    {
        var @namespace = type.ContainingNamespace;
        var namespaceString = @namespace.ToDisplayString();
        return $"{namespaceString}.{type.MetadataName}";
    }

    public static string GeneratedHintNameForType(
        INamedTypeSymbol type,
        string? subPart)
    {
        var baseFileName = FileNameForType(type);

        var suffix = string.Empty;
        if (subPart is not null)
        {
            suffix = $".{subPart}";
        }

        return $"{baseFileName}{suffix}.g.cs";
    }
}
