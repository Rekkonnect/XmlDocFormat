using Garyon.Extensions;
using Garyon.Objects;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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
public sealed class CliCommand(string command, DirectoryPath? workingDirectory = null)
{
    private readonly string _command = command;
    private readonly DirectoryPath? _workingDirectory = workingDirectory;

    public ProcessStartInfo ConstructProcessStartInfo(
        ProcessStartInfo? existingInstance = null)
    {
        _command.AsSpan().SplitOnce(' ', out var left, out var right);
        var fileName = left.ToString();
        var arguments = right.ToString();

        existingInstance ??= NewDefaultProcessStartInfo();
        existingInstance.FileName = fileName;
        existingInstance.Arguments = arguments;
        existingInstance.WorkingDirectory = _workingDirectory?.Path;
        return existingInstance;
    }

    public async Task<BaseRunResult> Run(
        CancellationToken cancellationToken)
    {
        return await Run(null, cancellationToken);
    }

    public async Task<BaseRunResult> Run(
        ProcessStartInfo? existingStartInfo,
        CancellationToken cancellationToken)
    {
        var processStartInfo = ConstructProcessStartInfo(existingStartInfo);
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
        DirectoryPath? workingDirectory,
        CancellationToken cancellationToken)
    {
        return await Run(
            command,
            workingDirectory,
            existingStartInfo: null,
            cancellationToken);
    }

    public static async Task<BaseRunResult> Run(
        string command,
        DirectoryPath? workingDirectory,
        ProcessStartInfo? existingStartInfo,
        CancellationToken cancellationToken)
    {
        var cli = new CliCommand(command, workingDirectory);
        return await cli.Run(existingStartInfo, cancellationToken);
    }

    private static ProcessStartInfo NewDefaultProcessStartInfo()
    {
        return new ProcessStartInfo()
        {
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
    }

    public abstract record BaseRunResult
    {
        public bool IsNoProcess => this is NoProcessRunResult;

        [MemberNotNullWhen(false, nameof(ProcessResult))]
        public bool IsProcess => this is ProcessRunResult;

        [MemberNotNullWhen(false, nameof(ProcessResult))]
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
