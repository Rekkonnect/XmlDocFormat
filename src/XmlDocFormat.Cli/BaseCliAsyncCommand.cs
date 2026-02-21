using Garyon.Extensions;
using Garyon.Mechanisms;
using Spectre.Console;
using Spectre.Console.Cli;
using XmlDocFormat.Core;

namespace XmlDocFormat.Cli;

public abstract class BaseCliAsyncCommand<TSettings, TExitCode>(IAnsiConsole ansiConsole)
    : AsyncCommand<TSettings>
    where TSettings : BaseCliCommandSettings
    where TExitCode : unmanaged, Enum
{
    protected IAnsiConsole AnsiConsole { get; } = ansiConsole;

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        TSettings settings,
        CancellationToken cancellationToken)
    {
        CommandCancellationHelpers.AugmentCancellation(ref cancellationToken);

        var simpleProfiler = new SimpleProfiler();
        TExitCode result;
        using (simpleProfiler.Run())
        {
            result = await ExecuteReal(context, settings, cancellationToken).NoContext;
        }

        var profilerResults = simpleProfiler.SnapshotResults!;
        AnsiConsole.Write($"Executed command in {profilerResults.Time.TotalMilliseconds:N2}ms");
        AnsiConsole.Write($" using {profilerResults.Memory / 1024D:N2} kB of memory.");

#pragma warning disable MA0011 // IFormatProvider is missing
        return Convert.ToInt32(result);
#pragma warning restore MA0011 // IFormatProvider is missing
    }

    protected abstract Task<TExitCode> ExecuteReal(
        CommandContext context,
        TSettings settings,
        CancellationToken cancellationToken);
}
