#!/usr/bin/dotnet run
#:project ../../src/XmlDocFormat.Cli/XmlDocFormat.Cli.csproj
#:project ../../src/XmlDocFormat.Core/XmlDocFormat.Core.csproj
#:package Garyon@*

using XmlDocFormat.Core;
using XmlDocFormat.Core.FileExtensions;
using XmlDocFormat.Cli;
using Garyon.Extensions;
using Garyon.Functions.Windows;

var openBrowser = args.Contains("-i");

var tokenSource = CommandCancellationHelpers.CreateCancelKeyTokenSource();
var cancellationToken = tokenSource.Token;

Console.WriteLine("Running tests with coverage");

var solutionDirectory = SolutionHelpers.GetSolutionRoot();
const string resultSubdirectory = "artifacts/coverage/results/";

var runResult = await CliCommand.Run(
    ConcatCommand([
        "dotnet test --solution ./ --coverage",
        $"--results-directory ./{resultSubdirectory}",
    ]),
    solutionDirectory,
    cancellationToken
);

if (runResult.IsNoProcess)
{
    return -10923;
}

Console.WriteLine("Merging coverage files");

var artifactsOutputDirectory = solutionDirectory.Subdirectory(resultSubdirectory);

const string mergedCoverageFileName = "solution-coverage.cobertura.xml";

runResult = await CliCommand.Run(
    ConcatCommand([
        "dotnet-coverage merge *.coverage --remove-input-files",
        $"-o {mergedCoverageFileName} -f cobertura",
    ]),
    artifactsOutputDirectory,
    cancellationToken
);

if (!runResult.IsProcessSuccess)
{
    return runResult.ExitCode ?? -10924;
}

Console.WriteLine("Generating coverage reports");
const string coverageReportSubdirectoryName = "coverage-report";
var coverageReportDirectory = artifactsOutputDirectory.Subdirectory(coverageReportSubdirectoryName);

runResult = await CliCommand.Run(
    ConcatCommand([
        $"reportgenerator -reports:{mergedCoverageFileName}",
        $"-targetdir:{coverageReportSubdirectoryName}",
        "-reporttypes:\"Html_Dark,Badges\"",
        // For now we exclude generated files because they are not present in
        // the disk, which pollutes the output with tons of errors
        // And the generated files themselves might contain uncovered code that
        // does not matter
        "\"-filefilters:-*.g.cs\"",
    ]),
    artifactsOutputDirectory,
    cancellationToken
);

if (!runResult.IsProcessSuccess)
{
    return runResult.ExitCode ?? -10925;
}

Console.WriteLine("Copying coverage badges to .github directory");

IReadOnlyList<string> syncedFileNames =
[
    "badge_linecoverage.svg",
    "badge_methodcoverage.svg",
];

const string githubImagesDirectoryName = ".github/images/";
var githubImagesDirectory = solutionDirectory.Subdirectory(githubImagesDirectoryName);

SyncFiles(syncedFileNames, coverageReportDirectory, githubImagesDirectory);

if (openBrowser)
{
    Console.WriteLine("Opening coverage report index in browser");
    var indexFile = coverageReportDirectory.File("index.html");
    ProcessUtilities.OpenUrl(indexFile.FullName);
}

return 0;

static void SyncFiles(
    IReadOnlyList<string> fileNames,
    DirectoryInfo source,
    DirectoryInfo target)
{
    // Ignore the command if the directory does not exist
    if (!source.Exists)
    {
        return;
    }

    target.CreateSafe();
    foreach (var file in fileNames)
    {
        SyncFile(file, source, target);
    }
}

static void SyncFile(
    string fileName,
    DirectoryInfo source,
    DirectoryInfo target)
{
    var sourceFile = source.File(fileName);
    var targetFile = target.File(fileName);
    if (!sourceFile.Exists)
    {
        targetFile.Delete();
        return;
    }

    sourceFile.CopyTo(targetFile, true);
}

static string ConcatCommand(ReadOnlySpan<string> parts)
{
    return string.Join(' ', parts);
}
