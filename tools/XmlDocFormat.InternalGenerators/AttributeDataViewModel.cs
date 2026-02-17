using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using XmlDocFormat.Core;

namespace XmlDocFormat.InternalGenerators;

[RoseLynnUtility]
public sealed class AttributeDataViewModel
{
    public required AttributeData AttributeData { get; init; }
    public required ImmutableArray<MappedAttributeArgument> ConstructorArguments { get; init; }
    public required ImmutableArray<MappedAttributeArgument> NamedArguments { get; init; }

    public ImmutableArray<MappedAttributeArgument> AllArguments
    {
        get
        {
            return
            [
                .. ConstructorArguments,
                .. NamedArguments,
            ];
        }
    }

    private AttributeDataViewModel() { }

    public static async Task<AttributeDataViewModel?> CreateAsync(
        AttributeData data,
        CancellationToken cancellationToken = default)
    {
        if (data.HasNoArguments())
            return Empty(data);

        if (data.ApplicationSyntaxReference is null)
            return null;

        var syntax = await data.ApplicationSyntaxReference.GetSyntaxAsync(cancellationToken);
        if (syntax is null)
            return null;

        return GetModelForSyntax(data, syntax);
    }

    public static AttributeDataViewModel? Create(
        AttributeData data,
        CancellationToken cancellationToken = default)
    {
        if (data.HasNoArguments())
            return Empty(data);

        var syntax = data.ApplicationSyntaxReference?.GetSyntax(cancellationToken);
        if (syntax is null)
            return null;

        return GetModelForSyntax(data, syntax);
    }

    private static AttributeDataViewModel? GetModelForSyntax(AttributeData data, SyntaxNode syntax)
    {
        return syntax switch
        {
            CSharpSyntaxNode csAttribute => CreateCSharpModel(data, csAttribute),
            _ => null,
        };
    }

    private static AttributeDataViewModel Empty(AttributeData data)
    {
        return new()
        {
            AttributeData = data,
            ConstructorArguments = [],
            NamedArguments = [],
        };
    }

    private static AttributeDataViewModel? CreateCSharpModel(
        AttributeData data, CSharpSyntaxNode csAttribute)
    {
        var attribute = csAttribute as Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax;
        if (attribute is null)
            return null;

        var argumentList = attribute.ArgumentList;
        if (argumentList is null)
            return Empty(data);

        var arguments = argumentList.Arguments;
        if (arguments is [])
            return Empty(data);

        return ConstructFromArguments(data, arguments);
    }

    private static AttributeDataViewModel ConstructFromArguments<TSyntax>(
        AttributeData data, SeparatedSyntaxList<TSyntax> arguments)
        where TSyntax : SyntaxNode
    {
        ImmutableArray<MappedAttributeArgument> regularArguments = [];
        ImmutableArray<MappedAttributeArgument> namedArguments = [];

        int argumentListOffset = 0;
        if (data.ConstructorArguments is not [] and var constructorArgumentsData)
        {
            var regularArgumentsBuilder = ImmutableArray.CreateBuilder<MappedAttributeArgument>(
                constructorArgumentsData.Length);

            // We always have a constructor if we have constructor arguments
            // We expect a potential breaking API change that returns a null
            // constructor with a non-empty argument list, in which case we show the param order
            var constructor = data.AttributeConstructor;
            var constructorParameters = constructor?.Parameters;
            var mappingKind =
                constructorParameters is not null
                ? AttributeArgumentNameMappingKind.Parameter
                : AttributeArgumentNameMappingKind.ParameterIndex;
            for (int i = 0; i < constructorArgumentsData.Length; i++)
            {
                var argumentSyntax = arguments.TryGetAt(i);
                if (argumentSyntax is null)
                {
                    break;
                }

                // TODO: Completely refactor that shit
                // TODO: Also discover that this is not a named parameter
                var argumentName = SyntaxFactsEx.GetArgumentName(argumentSyntax);
                var value = constructorArgumentsData[i];
                var parameter = constructorParameters?[i];
                var displayName = argumentName ?? parameter?.Name ?? i.ToString();
                regularArgumentsBuilder.Add(new(
                    argumentSyntax,
                    displayName,
                    value,
                    mappingKind));

                argumentListOffset = i;
            }
            regularArguments = regularArgumentsBuilder.ToImmutable();
        }

        if (data.NamedArguments is not [] and var namedArgumentsData)
        {
            var namedArgumentsBuilder = ImmutableArray.CreateBuilder<MappedAttributeArgument>(
                namedArgumentsData.Length);

            int offset = data.ConstructorArguments.Length;

            for (int i = 0; i < namedArgumentsData.Length; i++)
            {
                var argumentSyntax = arguments.TryGetAt(i + offset);
                if (argumentSyntax is null)
                {
                    break;
                }

                var kvp = namedArgumentsData[i];
                var name = kvp.Key;
                var value = kvp.Value;
                namedArgumentsBuilder.Add(new(
                    argumentSyntax,
                    name,
                    value,
                    AttributeArgumentNameMappingKind.Named));
            }
            namedArguments = namedArgumentsBuilder.ToImmutable();
        }

        return new()
        {
            AttributeData = data,
            ConstructorArguments = regularArguments,
            NamedArguments = namedArguments,
        };
    }

    public sealed record class MappedAttributeArgument(
        SyntaxNode ArgumentSyntax,
        string Name,
        TypedConstant Value,
        AttributeArgumentNameMappingKind MappingKind)
        ;

    public enum AttributeArgumentNameMappingKind
    {
        None,
        Parameter,
        ParameterIndex,
        Named,
    }
}
