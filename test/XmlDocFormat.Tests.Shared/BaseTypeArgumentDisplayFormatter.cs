namespace XmlDocFormat.Tests.Shared;

public abstract class BaseTypeArgumentDisplayFormatter<T>
    : ArgumentDisplayFormatter
{
    public sealed override bool CanHandle(object? value)
    {
        return value is T;
    }

    public sealed override string FormatValue(object? value)
    {
        var castValue = (T)value!;
        return FormatValue(castValue);
    }

    public abstract string FormatValue(T value);
}
