using Microsoft.CodeAnalysis;
using RoseLynn;
using System.Diagnostics.CodeAnalysis;
using XmlDocFormat.Core;

namespace XmlDocFormat.InternalGenerators;

[RoseLynnUtility]
public static class NullableTypeExtensions
{
    extension(ITypeSymbol type)
    {
        public bool IsNullableStruct()
        {
            return type.SpecialType is SpecialType.System_Nullable_T;
        }

        public bool IsNullableStruct(
            [NotNullWhen(true)]
            out ITypeSymbol? underlying)
        {
            var isNullable = type.IsNullableStruct();
            underlying = default;
            if (isNullable)
            {
                underlying = type.GetTypeArguments().First();
            }
            return isNullable;
        }

        public ITypeSymbol MakeNullable(Compilation compilation)
        {
            if (type is IPointerTypeSymbol or IFunctionPointerTypeSymbol)
            {
                // Pointer types do not support nullable annotations yet
                return type;
            }

            if (type.IsValueType)
            {
                var nullable = compilation.GetSpecialType(SpecialType.System_Nullable_T);
                return nullable.Construct([type]);
            }

            return type.WithNullableAnnotation(NullableAnnotation.Annotated);
        }

        public ITypeSymbol MakeNotNullable()
        {
            if (type is IPointerTypeSymbol or IFunctionPointerTypeSymbol)
            {
                // Pointer types do not support nullable annotations yet
                return type;
            }

            if (type.IsNullableStruct(out var underlyingNullableType))
            {
                return underlyingNullableType;
            }

            return type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        }
    }
}
