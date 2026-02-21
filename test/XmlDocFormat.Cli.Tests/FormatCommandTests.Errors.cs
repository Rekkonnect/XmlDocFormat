using System.IO.Abstractions.TestingHelpers;

namespace XmlDocFormat.Cli.Tests;

public partial class FormatCommandTests : BaseCliTests
{
    [Test]
    public async Task InexistentFile()
    {
        const string goodFilePath = @"Q:/cs/file1.cs";
        const string badFilePath = @"Q:/cs/file2.cs";

#pragma warning disable CS0162 // Unreachable code detected
        if (goodFilePath == badFilePath)
        {
            Assert.Fail("Invalid constants specified");
        }
#pragma warning restore CS0162 // Unreachable code detected

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [goodFilePath] = new("random string"),
            });
        var context = CreateTestContext(fileSystem);

        var result = await context.RunAsync(["format", badFilePath, "-l", "40", "-p", "1"]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.SourcePathDoesNotExist);
    }

    [Test]
    public async Task InexistentDirectory()
    {
        const string filePath = @"Q:/cs/file1.cs";

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [filePath] = new("random string"),
            });
        var context = CreateTestContext(fileSystem);

        var result = await context.RunAsync(["format", filePath, "-d", "-l", "40", "-p", "1"]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.SourcePathDoesNotExist);
    }

    [Test]
    public async Task InexistentDirectoryRecursive()
    {
        const string filePath = @"Q:/cs/file1.cs";

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [filePath] = new("random string"),
            });
        var context = CreateTestContext(fileSystem);

        var result = await context.RunAsync(["format", filePath, "-r", "-l", "40", "-p", "1"]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.SourcePathDoesNotExist);
    }

    [Test]
    public async Task InexistentFileFromDirectoryPath()
    {
        const string filePath = @"Q:/cs/file1.cs";

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [filePath] = new("random string"),
            });
        var context = CreateTestContext(fileSystem);

        var result = await context.RunAsync(["format", filePath, "-r", "-l", "40", "-p", "1"]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.SourcePathDoesNotExist);
    }

    [Test]
    public async Task InvalidOutputPathNotWritable()
    {
        const string filePath = @"Q:/cs/file1.cs";

        var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                [filePath] = new("random string"),
            });
        var context = CreateTestContext(fileSystem);

        var result = await context.RunAsync(
        [
            "format", filePath,
            "-o", "-:/Invalid/Path to: \0nowhere.\0",
            "-l", "40",
            "-p", "1",
        ]);

        await Assert.That(result.ExitCode)
            .IsEqualTo((int)FormatCommand.ExecutionResult.OutputPathNotWritable);
    }
}
