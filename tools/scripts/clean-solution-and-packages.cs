#!/usr/bin/dotnet run
#:project ../../src/XmlDocFormat.Cli/XmlDocFormat.Cli.csproj
#:project ../../src/XmlDocFormat.Core/XmlDocFormat.Core.csproj
#:package Garyon@*

using XmlDocFormat.Core;
using XmlDocFormat.Cli;
using Garyon.Extensions;
using System.Diagnostics;

var tokenSource = CommandCancellationHelpers.CreateCancelKeyTokenSource();
var cancellationToken = tokenSource.Token;

Console.WriteLine("Cleaning the entire solution");
var solutionDirectory = SolutionHelpers.GetSolutionRoot();

CliCommand.BaseRunResult runResult;

try
{
    var dotnetCleanProcessInfo = new ProcessStartInfo
    {
        CreateNoWindow = true,
        UseShellExecute = true,
    };

    runResult = await CliCommand.Run(
        "dotnet clean",
        solutionDirectory,
        dotnetCleanProcessInfo,
        cancellationToken.WithTimeout(1000)
    );

    if (!runResult.IsProcessSuccess)
    {
        return runResult.ExitCode ?? -14785;
    }
}
catch (TaskCanceledException)
{
    Console.WriteLine("The clean operation failed to finish before timeout");
}

Console.WriteLine("Deleting all NuGet packages of the internal generators");

runResult = await CliCommand.Run(
    "dotnet nuget locals global-packages -l",
    solutionDirectory,
    cancellationToken
);

if (!runResult.IsProcessSuccess)
{
    return runResult.ExitCode ?? -14786;
}

var globalPackageListingOutput = runResult.ProcessResult!.GetOutputString();
Console.WriteLine(globalPackageListingOutput);

globalPackageListingOutput.AsSpan()
    .SplitOnce(
        ':',
        out var packageKeyName,
        out var packageSourcePath);

var packageSourceDirectory = new DirectoryInfo(packageSourcePath.Trim().ToString());

const string internalGeneratorsPackageName = "XmlDocFormat.InternalGenerators";

var internalGeneratorsDirectory = packageSourceDirectory
    .Subdirectory($"{internalGeneratorsPackageName}");

internalGeneratorsDirectory.TryDelete(true);

if (internalGeneratorsDirectory.Exists)
{
    Console.WriteLine($"Failed to delete the directory {internalGeneratorsDirectory.FullName}");
    return -14787;
}

return 0;
