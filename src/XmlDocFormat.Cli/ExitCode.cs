namespace XmlDocFormat.Cli;

public readonly record struct ExitCode(int Value)
{
    public const int Success = 0;
    public const int GenericFailure = -1;
    public const int OperationCancelled = -2;
    public const int CommandParseException = -3;

    public bool IsSuccess => Value >= 0;
    public bool IsFailure => Value < 0;

    public static implicit operator int(ExitCode exitCode) => exitCode.Value;
    public static implicit operator ExitCode(int value) => new(value);
}
