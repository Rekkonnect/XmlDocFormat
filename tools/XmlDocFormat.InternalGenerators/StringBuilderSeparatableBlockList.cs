using System.Text;

namespace XmlDocFormat.InternalGenerators;

public sealed class StringBuilderSeparatableBlockList(StringBuilder builder)
    : BaseSeparatableBlockList
{
    private readonly StringBuilder _builder = builder;

    protected override void AppendSeparator()
    {
        _builder.AppendLine();
    }
}
