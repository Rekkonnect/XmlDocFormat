namespace XmlDocFormat.InternalGenerators.Core;

public sealed class AssertionException(
    string? message = null,
    string argumentExpression = "<none>")
    : Exception($"{message ?? DefaultMessage} (argument: {argumentExpression})")
{
    public const string DefaultMessage = "An assertion failed";

    public string ArgumentExpression { get; } = argumentExpression;
}
