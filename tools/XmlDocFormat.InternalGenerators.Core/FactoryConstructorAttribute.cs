namespace XmlDocFormat.InternalGenerators.Core;

/// <summary>
/// Denotes that the constructor, or the type's constructors, should
/// have a factory method generated that uses it, with the same
/// signature.
/// </summary>
/// <remarks>
/// When targeting a type, all the constructors defined in the type
/// are included for generation of the factory methods. In this case,
/// attributes on individual constructors within the type are ignored.
/// <br/>
/// All constructors, regardless of accessibility, are valid to generate
/// factory methods for. The generated factory methods will have the same
/// accessibility as the underlying constructor.
/// </remarks>
[AttributeUsage(ValidTargets, AllowMultiple = false, Inherited = false)]
public sealed class FactoryConstructorAttribute : Attribute
{
    private const AttributeTargets ValidTargets
        = AttributeTargets.Class
        | AttributeTargets.Struct
        | AttributeTargets.Constructor
        ;
}
