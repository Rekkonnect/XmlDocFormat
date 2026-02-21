using System.Reflection;

namespace XmlDocFormat.Tests.Shared;

public class FieldInfoArgumentDisplayFormatter : BaseTypeArgumentDisplayFormatter<FieldInfo>
{
    public override string FormatValue(FieldInfo value)
    {
        return $"{value.DeclaringType!.Name}.{value.Name}";
    }
}
