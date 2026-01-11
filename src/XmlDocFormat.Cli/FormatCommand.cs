using Garyon.Extensions;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO.Abstractions;
using System.Text;
using UtfUnknown;
using XmlDocFormat.Core;

namespace XmlDocFormat.Cli;

public class FormatCommand(
    IAnsiConsole ansiConsole,
    IFileSystem fileSystem)
    : BaseCliAsyncCommand<FormatCommand.Settings, FormatCommand.ExecutionResult>(ansiConsole)
{
    protected override async Task<ExecutionResult> ExecuteReal(
        CommandContext context,
        Settings settings,
        CancellationToken cancellationToken)
    {
        try
        {
            return await ExecuteCore(context, settings, cancellationToken);
        }
        catch
        {
            return ExecutionResult.GenericFailure;
        }
    }

    private async Task<ExecutionResult> ExecuteCore(
        CommandContext context,
        Settings settings,
        CancellationToken cancellationToken)
    {
        var validationResult = await ValidateAsync(context, settings, cancellationToken);
        if (validationResult is not ExecutionResult.Success)
        {
            return validationResult;
        }

        var options = settings.GetFormatterOptions();
        var formatter = new XmlDocFormatter(options);

        var mappings = settings.GetTargetFileMappings(fileSystem);
        if (mappings is [])
        {
            AnsiConsole.WriteLine($"No files found to format in {settings.FilePath}.");
            return ExecutionResult.NoFilesAffected;
        }

        var parallelism = settings.GetSelectedParallelism();
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = parallelism,
            CancellationToken = cancellationToken,
        };
        await Parallel.ForEachAsync(mappings, parallelOptions, FormatFile);

        if (settings.UsesDirectoryPath)
        {
            AnsiConsole.WriteLine($"Formatted {mappings.Length} *.cs files in '{settings.FilePath}'.");
        }
        else
        {
            AnsiConsole.WriteLine($"Formatted '{settings.FilePath}' successfully.");
        }

        return ExecutionResult.Success;

        async ValueTask FormatFile(FileMapping mapping, CancellationToken cancellationToken)
        {
            var bytes = await fileSystem.File.ReadAllBytesAsync(mapping.Source, cancellationToken);
            var detectionResult = CharsetDetectionHelpers.DetectFromBytesEx(bytes);
            var encoding = detectionResult.Detected.Encoding ?? Encoding.Utf8WithoutBom;
            var text = encoding.GetString(bytes);
            var formatted = formatter.Format(text);
            var formattedBytes = encoding.GetBytes(formatted);
            var targetFile = fileSystem.FileInfo.New(mapping.Target);
            targetFile.Directory!.CreateSafe();
            await fileSystem.File.WriteAllBytesAsync(
                mapping.Target, formattedBytes, cancellationToken);
        }
    }

    private async Task<ExecutionResult> ValidateAsync(
        CommandContext context,
        Settings settings,
        CancellationToken cancellationToken)
    {
        var isDirectory = settings.UsesDirectoryPath;
        IFileSystemEntity fileSystemEntity = isDirectory ? fileSystem.Directory : fileSystem.File;

        if (!fileSystemEntity.EntityExists(settings.FilePath))
        {
            AnsiConsole.WriteLine(
                $"The specified source path '{settings.FilePath}' does not exist.");
            return ExecutionResult.SourcePathDoesNotExist;
        }

        if (settings.OutputFilePath is not null and var outputPath)
        {
            try
            {
                fileSystemEntity.CreateEnsuringDirectory(outputPath);
            }
            catch
            {
                AnsiConsole.WriteLine(
                    $"""
                    The specified output path '{outputPath}' is not writable.
                    Ensure you have permissions to write to that path and maybe execute
                    the tool with elevated permissions.
                    """);
                return ExecutionResult.OutputPathNotWritable;
            }
        }

        return ExecutionResult.Success;
    }

    public partial class Settings : BaseCliCommandSettings
    {
        [CommandArgument(0, "<file-path>")]
        [Description("""
            The path of the file to format XML documentation comments in.
            When the -d or -r flag is specified, this path is treated as a
            directory path, which formats XML documentation comments in
            all *.cs files in the directory, and optionally its subdirectories.
            """)]
        public required string FilePath { get; init; }

        [CommandOption("-o|--output")]
        [Description("The path of the output. When not specified, the tool overwrites the files.")]
        [DefaultValue(null)]
        public string? OutputFilePath { get; init; }

        [CommandOption("-l|--line-length")]
        [Description("""
            The max length of every line, which determines at which point to break
            XML documentation comment lines. This ONLY affects XML documentation;
            regular code will not be affected, regardless of its line length.
            """)]
        [DefaultValue(XmlDocFormatOptions.DefaultLineLength)]
        public int LineLength { get; init; } = XmlDocFormatOptions.DefaultLineLength;

        [CommandOption("-d|--directory")]
        [Description("""
            When specified, treats the file path as a directory, which formats all
            *.cs files contained in the specified directory, but not recursively.
            """)]
        public bool IsDirectory { get; init; }

        [CommandOption("-r|--directory-recursive")]
        [Description("""
            When specified, treats the file path as a directory, which formats all
            *.cs files contained in the specified directory and all its subdirectories.
            """)]
        public bool DirectoryRecursive { get; init; }

        [CommandOption("-p|--parallelism")]
        [Description("""
            The maximum parallelism to use across formatting multiple files.
            Defaults to 0, which means the tool will automatically determine the
            optimal degree of parallelism based on processor count.
            """)]
        [DefaultValue(0)]
        public int MaxParallelism { get; init; } = 0;

        public bool UsesDirectoryPath => IsDirectory || DirectoryRecursive;

        public override ValidationResult Validate()
        {
            if (LineLength < XmlDocFormatOptions.MinimumLineLength)
            {
                return ValidationResult.Error(
                    $"The line length must be at least {XmlDocFormatOptions.MinimumLineLength}.");
            }

            return ValidationResult.Success();
        }

        internal XmlDocFormatOptions GetFormatterOptions()
        {
            return new()
            {
                MaxLineLength = LineLength,
            };
        }
    }

    partial class Settings
    {
        public bool HasSameOutput => string.IsNullOrEmpty(OutputFilePath);

        public int GetSelectedParallelism()
        {
            return MaxParallelism <= 0 ? Environment.ProcessorCount : MaxParallelism;
        }

        public ImmutableArray<FileMapping> GetTargetFileMappings(IFileSystem fileSystem)
        {
            if (!UsesDirectoryPath)
            {
                if (fileSystem.Path.GetExtension(FilePath) is not ".cs")
                {
                    return [];
                }

                var targetPath = OutputFilePath ?? FilePath;
                return [new(FilePath, targetPath)];
            }

            var filePaths = GetTargetFilePaths(fileSystem);

            if (HasSameOutput)
            {
                return filePaths
                    .Select(path => new FileMapping(path))
                    .ToImmutableArray();
            }

            return filePaths
                .Select(path => new FileMapping(path, CalculateTarget(path)))
                .ToImmutableArray();

            string CalculateTarget(string sourcePath)
            {
                var relative = fileSystem.Path.GetRelativePath(FilePath, sourcePath);
                var targetPath = fileSystem.Path.Combine(OutputFilePath!, relative);
                return targetPath;
            }
        }

        public ImmutableArray<string> GetTargetFilePaths(IFileSystem fileSystem)
        {
            if (IsDirectory)
            {
                return GetDirectoryFiles(fileSystem);
            }

            if (DirectoryRecursive)
            {
                return GetDirectoryRecursiveFiles(fileSystem);
            }

            return [FilePath];
        }

        private ImmutableArray<string> GetDirectoryFiles(IFileSystem fileSystem)
        {
            return fileSystem.Directory.GetFiles(FilePath).ToImmutableArray();
        }

        private ImmutableArray<string> GetDirectoryRecursiveFiles(IFileSystem fileSystem)
        {
            return fileSystem.Directory
                .GetFiles(FilePath, "*.cs", SearchOption.AllDirectories)
                .ToImmutableArray();
        }
    }

    public readonly record struct FileMapping(string Source, string Target)
    {
        public FileMapping(string sourceAndTarget)
            : this(sourceAndTarget, sourceAndTarget) { }
    }

    public enum ExecutionResult
    {
        Success = ExitCode.Success,
        NoFilesAffected = 1,

        GenericFailure = ExitCode.GenericFailure,
        OperationCancelled = ExitCode.OperationCancelled,

        SourcePathDoesNotExist = -10,
        OutputPathNotWritable = -11,
    }
}
