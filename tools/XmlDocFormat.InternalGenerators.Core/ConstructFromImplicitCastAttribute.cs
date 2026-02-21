namespace XmlDocFormat.InternalGenerators.Core;

/// <summary>
/// Denotes that the constructor, or the type's constructors, should
/// have an implicit cast operator that converts from the constructor's
/// parameter type to the constructor's type.
/// </summary>
/// <remarks>
/// <para>
/// When targeting a type, all the constructors with exactly one parameter
/// defined in the type are included for generation of the cast methods.
/// In this case, attributes on individual constructors within the type
/// are ignored.
/// </para>
/// <para>
/// All constructors with one parameter, regardless of accessibility,
/// are valid to generate implicit cast methods for. The generated implicit
/// cast methods will have the same accessibility as the underlying
/// constructor.
/// </para>
/// <para>
/// The declared implicit cast operators will convert from the type of the
/// constructor's single parameter to the type containing the constructor.
/// If the parameter is nullable, the cast's result is a non-nullable value
/// of the declared type. However, if the parameter is non-nullable, the
/// cast's result type will be a nullable version of the declared type.
/// <br/>
/// In practical terms, given a constructor like `public A(B b)`, the
/// generated implicit cast operator will be `A?(B? b)`, annotated with
/// `[return: NotNullIfNotNull(nameof(b))]`. However, in the case that the
/// constructor is `public A(B? b)`, the generated implicit cast operator
/// will be `A(B? b)`, where the result will always be a non-null instance
/// of A, since the constructor accepts null values of B.
/// </para>
/// </remarks>
[AttributeUsage(ValidTargets, AllowMultiple = false, Inherited = false)]
public sealed class ConstructFromImplicitCastAttribute : Attribute
{
    private const AttributeTargets ValidTargets
        = AttributeTargets.Class
        | AttributeTargets.Struct
        ;
}
