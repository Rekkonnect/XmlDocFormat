using Microsoft.CodeAnalysis;
using System.Runtime.InteropServices.ComTypes;
using XmlDocFormat.Core;

namespace XmlDocFormat.InternalGenerators;

[RoseLynnUtility]
public static class AttributeDataExtensions
{
    public static bool HasNoArguments(this AttributeData attribute)
    {
        return attribute.ConstructorArguments is []
            && attribute.NamedArguments is []
            ;
    }

    public static async Task<AttributeDataViewModel?> ViewModelAsync(
        this AttributeData attribute,
        CancellationToken cancellationToken)
    {
        return await AttributeDataViewModel.CreateAsync(attribute, cancellationToken);
    }

    public static AttributeDataViewModel? ViewModel(
        this AttributeData attribute,
        CancellationToken cancellationToken)
    {
        return AttributeDataViewModel.Create(attribute, cancellationToken);
    }
}
