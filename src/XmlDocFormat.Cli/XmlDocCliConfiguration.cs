using Garyon.Extensions;
using Garyon.Objects;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Reflection;

namespace XmlDocFormat.Cli;

public static class XmlDocCliConfiguration
{
    public static void Configure(IConfigurator config)
    {
        // Main app configuration
        config.SetApplicationName("XmlDocFormat");

        var informationalVersion = AppVersionInfo.InformationalVersionForAssembly(
            Assembly.GetExecutingAssembly());

        config.SetApplicationVersion(informationalVersion?.Version ?? "0.0.42");

        // Command setup
        config.AddCommand<FormatCommand>("format")
            .WithDescription("Formats XML documentation comments in *.cs files")
            .WithAlias("fmt")
            .WithAlias("f")
            .WithExample("format", "E:/repos/my-project/Utilities.cs")
            .WithExample("format", "E:/repos/my-project/Utilities.cs", "-l", "60")
            .WithExample("format", "E:/repos/my-project/", "-r", "-l", "60")
            ;

        config.SetExceptionHandler(HandleException);

        // Settings
        config.Settings.CaseSensitivity = CaseSensitivity.None;
        config.Settings.StrictParsing = false;

        // Debug configuration
#if DEBUG
        config.ValidateExamples();
#endif
    }

    private static int HandleException(Exception ex, ITypeResolver? resolver)
    {
        return HandleExceptionExitCode(ex, resolver);
    }

    private static ExitCode HandleExceptionExitCode(
        Exception ex, ITypeResolver? resolver)
    {
        var isCancellation = ex
            is TaskCanceledException
            or OperationCanceledException;

        if (isCancellation)
        {
            return ExitCode.OperationCancelled;
        }

        var isCommandParseException = ex is CommandParseException;
        if (isCommandParseException)
        {
            AnsiConsole.WriteLine(ex.Message);
            return ExitCode.CommandParseException;
        }

        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths);
        return ExitCode.GenericFailure;
    }
}
