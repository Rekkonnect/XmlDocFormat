using System.Reflection;

namespace XmlDocFormat.Core.Tests;

public class FieldInfoArgumentDisplayFormatter : BaseTypeArgumentDisplayFormatter<FieldInfo>
{
    public override string FormatValue(FieldInfo value)
    {
        return $"{value.DeclaringType!.Name}.{value.Name}";
    }
}
