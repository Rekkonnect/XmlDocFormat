using Garyon.Extensions;
using Garyon.Objects;
using System.Diagnostics;

namespace XmlDocFormat.Core;

/// <summary>
/// Represents a CLI command with capabilities to run that command
/// and gather the results after its execution.
/// </summary>
/// <remarks>
/// This is a minified implementation of a CLI command runner.
/// It may contain bugs, unsupported edge cases and inconsistent
/// behavior with how the commands would be executed in the
/// terminal. The point of developing this has been enhancing the
/// experience of writing C# scripts that interact with PowerShell.
/// </remarks>
[GaryonUtility]
public sealed class CliCommand(string command, string? workingDirectory = null)
{
    private readonly string _command = command;
    private readonly string? _workingDirectory = workingDirectory;

    public CliCommand(string command, DirectoryInfo? workingDirectory)
        : this(command, workingDirectory?.FullName) { }

    public ProcessStartInfo ConstructProcessStartInfo()
    {
        _command.AsSpan().SplitOnce(' ', out var left, out var right);
        var fileName = left.ToString();
        var arguments = right.ToString();

        return new()
        {
            FileName = fileName,
            Arguments = arguments,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = _workingDirectory,
        };
    }

    public async Task<BaseRunResult> Run(CancellationToken cancellationToken)
    {
        var processStartInfo = ConstructProcessStartInfo();
        var process = Process.Start(processStartInfo);
        if (process is null)
        {
            return NoProcessRunResult.Shared;
        }

        await process.WaitForExitAsync(cancellationToken);

        return new ProcessRunResult(process);
    }

    public static async Task<BaseRunResult> Run(
        string command,
        DirectoryInfo? workingDirectory,
        CancellationToken cancellationToken)
    {
        return await Run(command, workingDirectory?.FullName, cancellationToken);
    }

    public static async Task<BaseRunResult> Run(
        string command,
        string? workingDirectory,
        CancellationToken cancellationToken)
    {
        var cli = new CliCommand(command, workingDirectory);
        return await cli.Run(cancellationToken);
    }

    public abstract record BaseRunResult
    {
        public bool IsNoProcess => this is NoProcessRunResult;
        public bool IsProcess => this is ProcessRunResult;
        public bool IsProcessSuccess => this is ProcessRunResult { IsSuccess: true };

        public ProcessRunResult? ProcessResult => this as ProcessRunResult;

        public int? ExitCode => ProcessResult?.ExitCode;
    }

    public sealed record ProcessRunResult(Process Process)
        : BaseRunResult
    {
        public bool IsSuccess => ExitCode is 0;
        public bool IsFailure => ExitCode is not 0;

        public new int ExitCode => Process.ExitCode;

        public string GetOutputString()
        {
            return Process.StandardOutput.ReadToEnd();
        }

        public string GetErrorString()
        {
            return Process.StandardError.ReadToEnd();
        }
    }

    public sealed record NoProcessRunResult
        : BaseRunResult, ISharedInstance;
}
